namespace Mango.Areas.Admin.ViewModels.MenPromote
{
    public class MenPromoteUpdateVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string? PhotoPath { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
