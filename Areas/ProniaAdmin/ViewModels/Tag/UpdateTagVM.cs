using FronyToBack.Models;

namespace FronyToBack.Areas.ProniaAdmin.ViewModels.Tag
{
    public class UpdateTagVM
    {
        public string Name { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
    }
}
