﻿namespace Mango.Areas.Admin.ViewModels.KidIntro
{
    public class KidIntroCreateVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public IFormFile Photo { get; set; }
    }
}
