using System.ComponentModel.DataAnnotations;

namespace FronyToBack.Areas.ProniaAdmin.ViewModels.Category
{
    public class CreateCategoryVM
    {

        public string Name { get; set; }
        [Required]
        public List<Product>? Products { get; set; }
    }
}
