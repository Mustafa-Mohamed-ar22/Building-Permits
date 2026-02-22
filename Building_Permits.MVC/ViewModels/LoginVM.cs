
using System.ComponentModel.DataAnnotations;

namespace Building_Permits.MVC.ViewModels
{
    public class LoginVM
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Username { get; set; }

        public bool rememberMe { get; set; }
    }
}
