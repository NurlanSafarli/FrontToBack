using FronyToBack.Models;

namespace FronyToBack.Areas.ProniaAdmin.ViewModels.Color
{
    public class UpdateColorVM
    {
        public string Name { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
    }
}
