using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluentValidationDatabaseValidationRules.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
      
        public string Name { get; set; }
 
        public decimal Number { get; set; }
    }
}
