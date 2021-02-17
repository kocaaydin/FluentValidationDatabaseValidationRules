using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using FluentValidationDatabaseValidationRules.Common.Commands.Products;
using FluentValidationDatabaseValidationRules.Common.Validators;
using FluentValidationDatabaseValidationRules.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FluentValidationDatabaseValidationRules.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(x =>
            {
                x.Filters.Add<ValidateModelAttribute>();
            }).AddFluentValidation(x =>
                        {
                            x.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                            x.RegisterValidatorsFromAssemblyContaining<CreateProductCommand>();
                        });

            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.Configure<ApiBehaviorOptions>(options =>
            {

                options.InvalidModelStateResponseFactory = (context) =>
                {
                    var firstMessage = context.ModelState.Values.Where(v => v.Errors.Count > 0)
                                                              .SelectMany(v => v.Errors)
                                                              .Select(v => new
                                                              {
                                                                  v.ErrorMessage,
                                                                  Message = v.Exception == null ? "" : v.Exception.Message
                                                              })
                                                              .FirstOrDefault();

                    return new BadRequestObjectResult(new
                    {
                        code = "test",
                        message = firstMessage.ErrorMessage, firstMessage.Message
                    });
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
