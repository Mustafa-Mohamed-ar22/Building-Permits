using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Building_Permits.MVC.ViewModels
{
	public class fulfilVM
	{
		public int PermitId { get; set; }
		[Display(Name = "الاسم")]
		public string ClientName { get; set; }
		[Required(ErrorMessage ="من فضلك أدخل السبب")]
		[Display(Name ="السبب")]
		public string Reason { get; set; }
		[Required(ErrorMessage ="تاريخ التذكير مطلوب")]
		[Display(Name ="تاريخ التذكير")]
		[Remote(action:"isValidRemeberDate" , controller:"Remote")]
		public DateTime RememberDate { get; set; }
	}
}