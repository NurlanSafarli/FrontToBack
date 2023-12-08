namespace FronyToBack.Areas.ProniaAdmin.ViewModels.Slide
{
    public class CreateSlideVM
    {
        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }

        public IFormFile Photo { get; set; }
    }
}
