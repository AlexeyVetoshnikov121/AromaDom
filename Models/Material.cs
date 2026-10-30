using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageMaterials.wpf.Models
{
    public class Material
    {
        public int Id { get; set; }
        public int MaterialTypeId { get; set; }
        public string TypeName { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public decimal QuantityInStock { get; set; }
        public decimal MinQuantity { get; set; }
        public decimal? QuantityInPack { get; set; }
        public decimal RequiredQuantity { get; set; } 
    }
}
