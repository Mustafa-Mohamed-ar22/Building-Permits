using Building_Permits.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Building_Permits.MVC.ViewModels
{
    public class UpdategroupStage3
	{
		public int PermitId { get; set; }
		public List<PermitStage>? PermitStages { get; set; }

        [Display(Name ="مؤاجعة مهندس منصور")]
        public bool EngineerMansourReview { get; set; }
        [Display(Name = "ارفاق ملف")]
        public IFormFile? EngineerMansourImage { get; set; }
        public bool IsEngineerMansourHasImage { get; set; }
        [Display(Name = "ملاحظات")]
        public string? MasourNotes { get; set; }
        public bool IsMansourHasExistedImage { get; set; }
        public bool IsMansourHasChanges { get; set; }

        [Display(Name = "أمر الدفع النهائى")]
        public bool FinalPayOrder { get; set; }
        [Display(Name = "ارفاق ملف")]
        public IFormFile? FinalPayOrderImage { get; set; }
        public bool IsFinalPagyHasImage { get; set; }
        [Display(Name = "رقم المعاملة")]
        public string? FinalPayTransactionNumber { get; set; }
        [Display(Name = "ملاحظات")]
        public string? FinalPayNotes { get; set; }
        public bool IsFinalPayHasExistedImage { get; set; }
        public bool IsFinalPayHasChanges { get; set; }
	}
}