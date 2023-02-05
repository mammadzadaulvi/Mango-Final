namespace Mango.Areas.Admin.ViewModels.KidOtherLine
{
    public class KidOtherLineUpdateVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string? PhotoPath { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
