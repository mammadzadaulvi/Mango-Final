namespace Mango.Areas.Admin.ViewModels.TeenIntro
{
    public class TeenIntroCreateVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public IFormFile Photo { get; set; }
    }
}
