using System.Numerics;

namespace Building_Permits.Core.Entities
{
	public class Client
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string? NationalId { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public Permit? Permit { get; set; }
        public int? PermitId { get; set; }
    }
}
