using System.ComponentModel.DataAnnotations;

namespace Building_Permits.MVC.ViewModels
{
    public class GroupStages2VM
	{
		[Display(Name ="مراجعة المهندس")]
		public bool EngineerReview { get; set; }
		[Display(Name = "إرفاق ملف")]
		public IFormFile? EngineerReviewImage { get; set; }
		public bool IsEngineerReviewHasImage { get; set; }
		[Display(Name = "ملاحظات")]
		public string? EngineerReviewNotes { get; set; }


		[Display(Name ="مراجعة المهندس شادى")]
		public bool EngineerShadyReview { get; set; }
		[Display(Name = "ارفاق ملف")]
		public IFormFile? EngineerShadyReviewImage { get; set; }
		public bool IsEngineerShadyReviewHasImage { get; set; }
		[Display(Name = "ملاحظات")]
		public string? EngineerShadyReviewNotes { get; set; }



		[Display(Name = "النقابة")]
		public bool Syndicate { get; set; }
		[Display(Name = "ارفاق ملف")]
		public IFormFile? SyndicateImage { get; set; }
		public bool IsSyndicateHasImage { get; set; }
		[Display(Name = "ملاحظات")]
		public string? SyndicateNotes {  get; set; } 
	}
}