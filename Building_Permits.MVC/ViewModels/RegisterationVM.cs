using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Building_Permits.MVC.ViewModels
{
    public class RegisterationVM
    {
        [Required(ErrorMessage ="من فضلك أدخل الاسم")]
        [Display(Name ="Full Name ")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "من فضلك أدخل الهاتف")]
		public string Phone { get; set; }
        [Required(ErrorMessage = "من فضلك أدخل اسم المستخدم")]
        [Remote(action:"IsValidUserName" , controller:"Remote")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}