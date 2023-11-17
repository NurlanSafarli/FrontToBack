using FronyToBack.Models;
using Microsoft.EntityFrameworkCore;

namespace FronyToBack.ViewModels
{
    public class HomeVM
    {
        public List<Slide> Slides { get; set; }
        public List<Product> Products { get; set; }

    }
}
