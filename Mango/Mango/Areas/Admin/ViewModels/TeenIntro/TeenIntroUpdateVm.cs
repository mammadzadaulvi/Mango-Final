namespace Mango.Areas.Admin.ViewModels.TeenIntro
{
    public class TeenIntroUpdateVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public IFormFile? Photo { get; set; }
        public string? PhotoPath { get; set; }
    }
}
