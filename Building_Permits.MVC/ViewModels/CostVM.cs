using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Building_Permits.MVC.ViewModels
{
	public class CostVM
	{
		[Required(ErrorMessage ="من فضلك أدخل قيمة المبلغ المصروف")]
		[Display(Name =	"التكلفة")]
		[Remote(action: "isValidAmountCost", controller:"Remote")]
		public decimal Amount { get; set; }
		[Required(ErrorMessage = "من فضلك أدخل فى ماذا تم صرف المبلغ")]
		[Display(Name ="الوصف")]
		public string AmountFor { get; set; }
		public int PermitId { get; set; }
        [Display(Name = "العميل")]
        public string ClientName { get; set; }
		public int? CostId { get; set; }
	}
    public class GeneralCostVM
    {
        [Required(ErrorMessage = "من فضلك أدخل قيمة المبلغ المصروف")]
        [Display(Name = "التكلفة")]
        [Remote(action: "isValidAmountCost", controller: "Remote")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "من فضلك أدخل فى ماذا تم صرف المبلغ")]
        [Display(Name = "الوصف")]
        public string AmountFor { get; set; }
        public int? Id { get; set; }
    }
}