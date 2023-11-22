using System.ComponentModel.DataAnnotations;

namespace FronyToBack.ViewModels
{
    public class UpdateProductVM
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public int CategoryId { get; set; }
        public List<int> TagIds { get; set; }


    }
}
