using FluentValidationDatabaseValidationRules.Common.Commands.Products;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FluentValidationDatabaseValidationRules.API.Controllers
{
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        [HttpPost, Route("")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateProductCommand command)
        {
            return Ok();
        }
    }
}
