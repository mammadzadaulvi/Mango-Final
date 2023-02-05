namespace Mango.Areas.Admin.ViewModels.MenPromote
{
    public class MenPromoteCreateVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public IFormFile Photo { get; set; }
    }
}
