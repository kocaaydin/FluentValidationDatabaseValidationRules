using FluentValidation;
using FluentValidationDatabaseValidationRules.Common.Commands.Products;
using FluentValidationDatabaseValidationRules.Core.Entities;
using FluentValidationDatabaseValidationRules.Data;

namespace FluentValidationDatabaseValidationRules.Common.Validators.Products
{
    public class CreateProductCommandValidator : BaseValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator(ApplicationContext applicationContext) : base(applicationContext)
        {
            RuleFor(x => x.Name).NotEmpty();

            RuleFor(x => x.Number).NotNull();

            SetDatabaseValidationRules<Product>();
        }
    }
}
