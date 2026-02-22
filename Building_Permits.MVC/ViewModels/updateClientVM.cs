using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Building_Permits.MVC.ViewModels
{
    public class updateClientVM
    {

		[Required(ErrorMessage = "من فضلك قم بإدخال الاسم")]
		[Display(Name = "الاسم رباعى")]
		[Remote(action: "isValidName", controller: "Remote", ErrorMessage = "الاسم لا يجب أن يحتوى على أرقام أو أى علامات أخرى")]
		public string FullName { get; set; }
		[Required(ErrorMessage = "من فضلك قم بإدخال الرقم القومى")]
		[Display(Name = "الرقم القومى")]
		[Remote(action: "isValidId2", controller: "Remote", AdditionalFields = "id")]
		public string NationalId { get; set; }

        public int id { get; set; }
		[Required(ErrorMessage = "من فضلك قم بإدخال رقم التليفون")]
		[Display(Name = "رقم التليفون")]
		[Remote(action: "isValidPhone2", controller: "Remote",AdditionalFields = "id")]
		public string Phone { get; set; }
		[Required(ErrorMessage = "من فضلك قم بإدخال العنوان")]
		[Display(Name = "العنوان")]
		public string Address { get; set; }
    }
}
