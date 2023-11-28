using FronyToBack.Models;

namespace FronyToBack.Areas.ViewModels.Tag
{
    public class CreateTagVM
    {
        public string Name { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
    }
}
