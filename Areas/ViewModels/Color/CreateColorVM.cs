using FronyToBack.Models;

namespace FronyToBack.Areas.ViewModels.Color
{
    public class CreateColorVM
    {

        public string Name { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
    }
}
