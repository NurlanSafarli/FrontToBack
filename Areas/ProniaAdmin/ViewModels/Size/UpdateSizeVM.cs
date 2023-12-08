using FronyToBack.Models;

namespace FronyToBack.Areas.ProniaAdmin.ViewModels.Size
{
    public class UpdateSizeVM
    {
        public string Name { get; set; }
        public List<ProductSize>? ProductSizes { get; set; }
    }
}
