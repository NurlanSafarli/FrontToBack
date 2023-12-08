using FronyToBack.Models;

namespace FronyToBack.Areas.ProniaAdmin.ViewModels.Size
{
    public class CreateSizeVM
    {
        public string Name { get; set; }
        public List<ProductSize>? ProductSizes { get; set; }
    }
}
