using Microsoft.AspNetCore.Identity;

namespace Building_Permits.Core.Entities
{
	public class ApplicationUser : IdentityUser
	{
		public List<Notification> Notifications { get; set; }
		public List<Permit> Permits { get; set; }
		public bool isActive { get; set; }
		public DateTime? LastLogin { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string FullName { get; set; }
	}
}