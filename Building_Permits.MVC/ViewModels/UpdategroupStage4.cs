using Building_Permits.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Building_Permits.MVC.ViewModels
{
    public class UpdategroupStage4
    {
        public int PermitId { get; set; }
        public List<PermitStage>? PermitStages { get; set; }

        [Display(Name ="طباعة الرخصة")]
        public bool Printing { get; set; }
        [Display(Name ="إرفاق ملف")]
        public IFormFile? PrintingImage { get; set; }
        public bool IsPrinitingHasImage { get; set; }
        public bool IsPrintingHasExistedImage { get; set; }
        public bool IsPrintingHasChanges { get; set; }
        public bool IsPermitReceived { get; set; }
    }
}