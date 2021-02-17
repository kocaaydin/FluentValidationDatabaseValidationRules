using FluentValidation;
using FluentValidationDatabaseValidationRules.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace FluentValidationDatabaseValidationRules.Common.Validators
{
    public abstract class BaseValidator<TModel> : AbstractValidator<TModel> 
    {
        private readonly ApplicationContext _applicationContext;

        public BaseValidator(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        protected virtual void SetDatabaseValidationRules<TEntity>()
        {
            SetStringPropertiesMaxLength<TEntity>();
            SetDecimalMaxValue<TEntity>();
        }

        protected virtual void SetStringPropertiesMaxLength<TEntity>(  params string[] filterPropertyNames)
        { 
            var entityType = _applicationContext.Model.FindEntityType(typeof(TEntity));

            var entityProperties = entityType.GetProperties().Where(x => x.ClrType == typeof(string) && !string.IsNullOrEmpty(x.Name) && x.GetMaxLength().HasValue)
                                                             .Select(property => new { property.Name, MaxValue = property.GetMaxLength() ?? 0 });

            var maxLengthExpressions = entityProperties.Select(property => new
            {
                MaxLength = property.MaxValue,
                Expression = DynamicExpressionParser.ParseLambda<TModel, string>(null, false, property.Name)
            }).ToList();

            foreach (var expression in maxLengthExpressions)
            {
                RuleFor(expression.Expression).Length(0, expression.MaxLength);
            }
        }
     
        protected virtual void SetDecimalMaxValue<TEntity>()
        {
            
            var modelPropertyNames = typeof(TModel).GetProperties()
                                                   .Where(property => property.PropertyType == typeof(decimal))
                                                   .Select(property => property.Name).ToList();

            var entityTypeFullName = typeof(TEntity).FullName;

          
            var entityType = _applicationContext.Model.FindEntityType(typeof(TEntity));

            var properties = entityType.GetProperties().Where(property => property.ClrType == typeof(decimal)).Select(property =>
            {
                var precisionAndScale = property.GetRelationalTypeMapping().StoreType.Replace("decimal(", string.Empty).Replace(")", "").Split(",");

                decimal? value = null;
                decimal? maxValueForValidator = null;

                if (precisionAndScale.Count() == 2)
                {
                    value =(decimal?)(Math.Pow(10, (Convert.ToInt32(precisionAndScale[0]) - Convert.ToInt32(precisionAndScale[1]))) - (Math.Pow(10, -Convert.ToInt32(precisionAndScale[1]))));
                    maxValueForValidator = (decimal?)(Math.Pow(10, (Convert.ToInt32(precisionAndScale[0]) - Convert.ToInt32(precisionAndScale[1]))));
                }

                return new
                {
                    property.Name,
                    MaxValue = value,
                    MaxValueForValidator = maxValueForValidator
                };
            });

            var maxValueExpressions = properties.Where(x=>x.MaxValue.HasValue).Select(property => new
            {
                MaxValue = property.MaxValue,
                MaxValueForValidator = property.MaxValueForValidator,
                Expression = DynamicExpressionParser.ParseLambda<TModel, decimal>(null, false, property.Name)
            }).ToList();


            foreach (var expression in maxValueExpressions)
            {
                RuleFor(expression.Expression).SetValidator(new DecimalPropertyValidator(expression.MaxValueForValidator ?? 0)).WithMessage($"Maksimum değer {expression.MaxValue} olmaldır");
            }
        }
    }
}
