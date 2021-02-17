using FluentValidation.Validators;
using System;

namespace FluentValidationDatabaseValidationRules.Common.Validators
{
    public class DecimalPropertyValidator : PropertyValidator
    {
        private readonly decimal _maxValue;

       
        public DecimalPropertyValidator(decimal maxValue) : base($"Maksimum değer {maxValue - 1} olmaldır")
        {
            _maxValue = maxValue;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (decimal.TryParse(context.PropertyValue.ToString(), out decimal value))
                return Math.Round(value, 3) < _maxValue;

            return false;
        }
    }
}