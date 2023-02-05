namespace Mango.Areas.Admin.ViewModels.HomeIntro
{
    public class HomeIntroUpdateVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public IFormFile? Photo { get; set; }
        public string? PhotoPath { get; set; }
    }
}
