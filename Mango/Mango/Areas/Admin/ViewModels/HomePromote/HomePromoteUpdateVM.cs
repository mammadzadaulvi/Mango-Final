namespace Mango.Areas.Admin.ViewModels.HomePromote
{
    public class HomePromoteUpdateVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string? PhotoPath { get; set; }
        public IFormFile? Photo { get; set; }

    }
}
