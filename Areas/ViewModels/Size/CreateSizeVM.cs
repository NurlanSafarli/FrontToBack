using FronyToBack.Models;

namespace FronyToBack.Areas.ViewModels.Size
{
    public class CreateSizeVM
    {
        public string Name { get; set; }
        public List<ProductSize>? ProductSizes { get; set; }
    }
}
