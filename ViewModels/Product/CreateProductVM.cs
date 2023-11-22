using FronyToBack.Models;
using System.ComponentModel.DataAnnotations;

namespace FronyToBack.ViewModels
{
    public class CreateProductVM
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public int CategoryId { get; set; }
        [Required]

        public List<int> TagIds { get; set; }
    }
}
