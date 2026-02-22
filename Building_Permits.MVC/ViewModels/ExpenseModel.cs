using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Building_Permits.MVC.ViewModels
{
	public class ExpenseModel
	{
		public int PermitId { get; set; }
		[Required]
		[Display(Name ="المبلغ المتفق عليه")]
		[Remote(action:"IsValidAgreed",controller:"Remote", AdditionalFields = "Received", ErrorMessage ="يحب أن يكون المبلغ المتفق عليه أكبر من القيمة المستلمة")]
		public decimal AgreementAmount {  get; set; }
		[Display(Name ="الواصل")]
		[Remote(action: "IsValidReceived", controller: "Remote", AdditionalFields = "AgreementAmount", ErrorMessage = "يحب أن يكون القيمة المستلمة أقل من المبلغ المتفق عليه ")]

		public decimal Received {  get; set; }
		[Display(Name ="العميل")]
		public string? PermitOwner { get; set; }
	}
}
