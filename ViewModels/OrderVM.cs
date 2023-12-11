using FronyToBack.Models;

namespace FronyToBack.ViewModels
{
    public class OrderVM
    {
        public string Address { get; set; }
        public List<BasketItem>? BasketItems { get; set; }

    }
}
