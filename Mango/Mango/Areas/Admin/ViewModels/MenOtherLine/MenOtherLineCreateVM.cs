namespace Mango.Areas.Admin.ViewModels.MenOtherLine
{
    public class MenOtherLineCreateVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public IFormFile Photo { get; set; }
    }
}
