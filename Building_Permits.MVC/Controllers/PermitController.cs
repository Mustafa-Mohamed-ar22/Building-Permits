using Building_Permits.Core.Entities;
using Building_Permits.Core.Enums;
using Building_Permits.Core.Repo;
using Building_Permits.Infrastructure;
using Building_Permits.Infrastructure.Services;
using Building_Permits.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IO;
namespace Building_Permits.MVC.Controllers
{
	[Authorize]
	public class PermitController : Controller
	{
		private readonly IPermitStageRepo permitStageService;
		private readonly IPermitRepo permitService;
		private readonly IWebHostEnvironment _env;
		private readonly IStageRepo stageService;
        private readonly IClientRepo clientService;
		private readonly ICostRepo costService;
		private readonly AppDbContext context;
		private readonly IExpenseRepo expenseService;

		public PermitController(IPermitStageRepo permitStageService, IPermitRepo permitService, IWebHostEnvironment _env, IStageRepo stageService,IClientRepo clientService,ICostRepo costService,AppDbContext context,IExpenseRepo expenseService)
		{
			this.permitStageService = permitStageService;
			this.permitService = permitService;
			this._env = _env;
			this.stageService = stageService;
            this.clientService = clientService;
			this.costService = costService;
			this.context = context;
			this.expenseService = expenseService;
		}
        public IActionResult FinalDelete(int id)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
					Permit permit = permitService.GetById(id);
                    Client client = clientService.GetByPermitId(id);
                    var costs = costService.GetPermitCosts(id);
                    var expenses = expenseService.GetAllExpenses().Where(x => x.PermitId == id).ToList();
                    var notifications = context.Notifications.Where(x => x.PermitId == id).ToList();
                    var permitStages = context.PermitStages.Where(x => x.PermitId == id).ToList();

                    foreach (var item in costs)
                    {
                        costService.Delete(item.Id);
                    }
					permit.ExpenseId = null;
                    foreach (var item in expenses)
                    {
                        expenseService.DeleteExpense(item.Id);
                    }

                    foreach (var item in notifications)
                    {
                        context.Notifications.Remove(item);
                    }

                    foreach (var item in permitStages)
                    {
                        context.PermitStages.Remove(item);
                    }

                    if (client != null)
                    {
                        client.PermitId = null;
						clientService.DeleteClient(client.Id);
                        context.SaveChanges(); 
                    }
					permitService.DeletePermit(id);
                    context.SaveChanges();

                    transaction.Commit();

                    TempData["deleteFinal"] = "تم حذف جميع بيانات الرخصة بالكامل";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    TempData["deleteFinal"] = "حدث خطأ أثناء حذف البيانات: " + ex.Message;
                }
            }

            return RedirectToAction("Archieve");
        }
        [HttpGet]
		[Authorize(Roles ="Admin")]
		public IActionResult fulfilments()
		{
			var fulfils = permitService.GetAllPermits().Where(x => x.isPaused == true).Select(x => new fulfilDisplayVM
			{
				clientName= clientService.GetByPermitId(x.Id).FullName,
				PermitId = x.Id,
				pausedAt=x.PausedAt,
				rememberDate=x.FulfilmentDate,
				fulfilReason = x.FulfilmentReason
			});
			return View(fulfils);
		}
		[HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Fulfillment(int id)
		{
			fulfilVM vM = new fulfilVM
			{
				ClientName = clientService.GetByPermitId(id).FullName,
				PermitId = id,
				RememberDate = DateTime.Now
			};
			return View(vM);
		}
		public IActionResult DeleteV1(int id)
		{
			Permit permit = permitService.GetById(id);
			permit.ArchivedAt = DateTime.Now;
			permit.Status = PermitStatus.Archieved;
			var permitsStages = permitStageService.GetAllPermitStages().Where(x => x.PermitId == id);
			foreach (var item in permitsStages)
			{
				item.CompletedAt = DateTime.Now;
			}
			permitStageService.Save();
			TempData["deletePermit"] = "تم حذف الرخصة بنجاح";
			return RedirectToAction("Index", "Client");
		}
		[HttpPost]
		public IActionResult Fulfillment(fulfilVM vM)
		{
			if(ModelState.IsValid)
			{
				var permit= permitService.GetById(vM.PermitId);
				permit.isPaused = true;
				permit.FulfilmentReason = vM.Reason;
				permit.FulfilmentDate = vM.RememberDate;
				permit.PausedAt = DateTime.Now;
                TempData["fulfil"] = $"تم نقل رخصة العميل {vM.ClientName} إلى قسم الاستيفاء بنجاح";
                permitService.Save();
				return RedirectToAction("fulfilments");
			}	
			return View(vM);
		}
        [Authorize(Roles = "Admin")]
        [HttpPost]
		public IActionResult DeleteFormFulfil(int id)
		{
			
			string CleintName = clientService.GetByPermitId(id).FullName;
			var permit = permitService.GetById(id);
			if(permit.isPaused==false)
			{
				TempData["deletefulfil"] = "تم حذف الرخصة بالفعل من قسم الاستيفاء ... ";
				return RedirectToAction("fulfilments");
			}
			permit.isPaused = false;
			permit.FulfilmentDate = null;
			permit.FulfilmentReason = null;
			permit.PausedAt = null;
			permitService.Save();
			var permitsStages = permitStageService.GetAllPermitStages().Where(x => x.PermitId == id && x.CompletedAt == null);
            foreach (var item in permitsStages)
            {
				item.DueAt = DateTime.Now.AddDays(stageService.GetStageById(item.StageId).DefaultDaysLimit);
			}
			permitService.Save();
			TempData["deletefulfil"] = "تم حذف رخصة العميل  من قسم الاستيفاء ... سيتم استكمال الرخصة ومواعيد البنود من الآن";
			return RedirectToAction("fulfilments");
		}
		[HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ExtendFulfil(int id)
		{
			fulfilVM vM = new fulfilVM
			{
				ClientName = clientService.GetByPermitId(id).FullName,
				PermitId = id,
				RememberDate = (DateTime)permitService.GetById(id).FulfilmentDate,
			};
			return View(vM);
		}
		[HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ExtendFulfil(fulfilVM vM)
		{
			if (ModelState.IsValid)
			{
				var permit = permitService.GetById(vM.PermitId);
				permit.isPaused = true;
				permit.FulfilmentReason = vM.Reason;
				permit.FulfilmentDate = vM.RememberDate;
				TempData["extendfulfil"] = $"تم مد استيفاء رخصة العميل {vM.ClientName} حتى تاريخ {vM.RememberDate:dd/MM/yyyy hh:mm tt}";
				permitService.Save();
                return RedirectToAction("fulfilments");
            }
			return View(vM);
		}
		[HttpGet]
		public IActionResult PermitDetails(int id)
		{
			var permitStages = permitStageService.GetAllPermitStages().
				Where(x => x.PermitId == id && x.CompletedAt != null).Select(x=>new permitstageDetails
				{
					stageName = stageService.GetStageById(x.StageId).Name,
					PathPhoto = x.AttachmentPath,
					StageStartedAt = x.StartedAt,
					StageCompletedAt= (DateTime)x.CompletedAt,
					DaysSpendOnthisStage = (x.CompletedAt-x.StartedAt).Value.Days,
					stageId = x.StageId
				}).ToList();
			if(permitStages.Count()==0)
			{
				return View(new PermitDetailsVM
				{
					PermitId = id,
					permitstages = permitStages,
					ClientName = clientService.GetByPermitId(id).FullName,
					DaysOnPermitGenerally = (DateTime.Now - permitService.GetById(id).CreatedAt).Days,
					TotolCosts = costService.GetTotalPermitCosts(id),
					PermitStartedAt = permitService.GetById(id).CreatedAt
				});
			}
			PermitDetailsVM model = new PermitDetailsVM
			{
				PermitId=id,
				permitstages = permitStages,
				ClientName = clientService.GetByPermitId(id).FullName,
				DaysOnPermitGenerally = (permitStages.MaxBy(x => x.StageCompletedAt).StageCompletedAt-permitService.GetById(id).CreatedAt).Days,
				TotolCosts = costService.GetTotalPermitCosts(id),
				PermitStartedAt = permitService.GetById(id).CreatedAt
			};
			return View(model);

		}
		public IActionResult Index()
		{

			return View();
		}
		[HttpGet]
		public IActionResult StageGroup1(int PermitId)
		{
			StageGroup1View view = new StageGroup1View()
			{
				ContractReceiveAndAutharization = false,
				Surveying = false,
				CompletingFile = false,
				FinishingFile = false,
				Validity = false,
				LegalAffairs = false,
				PermitId = PermitId
			};
			return View(view);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> StageGroup1(StageGroup1View group1View)
		{
			if (ModelState.IsValid)
			{
				var PermitStages = permitStageService.GetAllPermitStages().Where(x => x.PermitId == group1View.PermitId).ToList();
				var rootPath = _env.ContentRootPath;//webrootpath
				string fullpath = Path.Combine(rootPath, "Uploads", group1View.PermitId.ToString());
				bool ValidityEntered = false;
				bool ContractEntered = false;
				bool LegalEntered = false;
				bool surveyingEntered = false;
				bool CompletingFileEntered = false;
				bool FinishingFileEntered = false;
				Directory.CreateDirectory(fullpath);
				foreach (var item in PermitStages)
				{
					if (group1View.ContractReceiveAndAutharization && item.StageId == 2 && ContractEntered == false)
					{
						string fieldPath = fullpath + "\\" + group1View.ContractReceiveAndAutharizationImage.Name + $"{Path.GetExtension(group1View.ContractReceiveAndAutharizationImage.FileName)}";
						using (var stream = new FileStream(fieldPath, FileMode.Create))
						{
							await group1View.ContractReceiveAndAutharizationImage.CopyToAsync(stream);
						}
						item.AttachmentPath = fieldPath;
						item.CompletedAt = DateTime.Now;
						item.Notes = group1View.ContractReceiveAndAutharizationNotes;
						item.TransactionNumber = group1View.ContractReceiveAndAutharizationTranactionNumber;
						ContractEntered = true;
					}
					if (group1View.Validity && item.StageId == 3 && ValidityEntered == false)
					{
						string fieldPath = fullpath + "\\" + group1View.ValidityImage.Name + $"{Path.GetExtension(group1View.ValidityImage.FileName)}";
						using (var stream = new FileStream(fieldPath, FileMode.Create))
						{
							await group1View.ValidityImage.CopyToAsync(stream);
						}
						item.AttachmentPath = fieldPath;
						item.CompletedAt = DateTime.Now;
						item.Notes = group1View.ValidityNotes;
						item.TransactionNumber = group1View.ValidityTranactionNumber;
						ValidityEntered = true;
					}

					if (group1View.LegalAffairs && item.StageId == 4 && LegalEntered == false)
					{
						string fieldPath = fullpath + "\\" + group1View.LegalAffairsImage.Name + $"{Path.GetExtension(group1View.LegalAffairsImage.FileName)}";
						using (var stream = new FileStream(fieldPath, FileMode.Create))
						{
							await group1View.LegalAffairsImage.CopyToAsync(stream);
						}
						item.AttachmentPath = fieldPath;
						item.CompletedAt = DateTime.Now;
						item.Notes = group1View.LegalAffairsNotes;
						item.TransactionNumber = group1View.LegalAffairsTranactionNumber;
						LegalEntered = true;
					}
					if (group1View.Surveying && item.StageId == 5 && surveyingEntered == false)
					{
						string fieldPath = fullpath + "\\" + group1View.SurveyingImage.Name + $"{Path.GetExtension(group1View.SurveyingImage.FileName)}";
						using (var stream = new FileStream(fieldPath, FileMode.Create))
						{
							await group1View.SurveyingImage.CopyToAsync(stream);
						}
						item.AttachmentPath = fieldPath;
						item.CompletedAt = DateTime.Now;
						item.Notes = group1View.SurveyingNotes;
						item.TransactionNumber = group1View.SurveyingTranactionNumber;
						surveyingEntered = true;
					}
					if (group1View.CompletingFile && item.StageId == 6 && CompletingFileEntered == false)
					{
						string fieldPath = fullpath + "\\" + group1View.CompletingFileImage.Name + $"{Path.GetExtension(group1View.CompletingFileImage.FileName)}";
						using (var stream = new FileStream(fieldPath, FileMode.Create))
						{
							await group1View.CompletingFileImage.CopyToAsync(stream);
						}
						item.AttachmentPath = fieldPath;
						item.CompletedAt = DateTime.Now;
						item.Notes = group1View.CompletingFileNotes;
						item.TransactionNumber = group1View.CompletingFileTranactionNumber;
						CompletingFileEntered = true;
					}
					if (group1View.FinishingFile && item.StageId == 7 && FinishingFileEntered == false)
					{
						string fieldPath = fullpath + "\\" + group1View.FinishingFileImage.Name + $"{Path.GetExtension(group1View.FinishingFileImage.FileName)}";
						using (var stream = new FileStream(fieldPath, FileMode.Create))
						{
							await group1View.FinishingFileImage.CopyToAsync(stream);
						}
						item.AttachmentPath = fieldPath;
						item.CompletedAt = DateTime.Now;
						item.Notes = group1View.FinishingFileNotes;
						item.TransactionNumber = group1View.FinishingFileTranactionNumber;
						FinishingFileEntered = true;
					}
					permitStageService.Save();
				}
				TempData["StageGroup1"] = "تم حفظ جميع البيانات";

				return RedirectToAction("Index", "Client");
			}
			return View(group1View);
		}
		public IActionResult GetImagePath(int permitId, int stageId)
		{
			var permitStage = permitStageService
				.GetAllPermitStages()
				.FirstOrDefault(x => x.PermitId == permitId && x.StageId == stageId);
			if(permitStage == null || string.IsNullOrEmpty(permitStage.AttachmentPath))
				return NotFound();
            Console.WriteLine(permitStage.AttachmentPath.Substring(86));
            return Content(permitStage.AttachmentPath.Substring(86));
		}
		public IActionResult GetImage(int permitId, int stageId)
		{
			var permitStage = permitStageService
				.GetAllPermitStages()
				.FirstOrDefault(x => x.PermitId == permitId && x.StageId == stageId);

			if (permitStage == null || string.IsNullOrEmpty(permitStage.AttachmentPath))
				return NotFound();

			var filePath = permitStage.AttachmentPath;
			var mimeType = "image/" + Path.GetExtension(filePath).TrimStart('.').ToLower();
			var fileBytes = System.IO.File.ReadAllBytes(filePath);

			return File(fileBytes, mimeType);
		}
        //public IActionResult GetFile(int permitId, int stageId)
        //{
        //	var permitStage = permitStageService
        //		.GetAllPermitStages()
        //		.FirstOrDefault(x => x.PermitId == permitId && x.StageId == stageId);

        //	if (permitStage == null || string.IsNullOrEmpty(permitStage.AttachmentPath))
        //		return NotFound();

        //	var filePath = permitStage.AttachmentPath;

        //	if (!System.IO.File.Exists(filePath))
        //		return NotFound();

        //	var fileName = Path.GetFileName(filePath);
        //	var contentType = "application/octet-stream";

        //	// Try detect content type for inline preview (pdf, jpg, png)
        //	new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);

        //	return File(System.IO.File.ReadAllBytes(filePath), contentType, fileName);
        //}
        public IActionResult GetFile(int permitId, int stageId)
        {
            var permitStage = permitStageService
                .GetAllPermitStages()
                .FirstOrDefault(x => x.PermitId == permitId && x.StageId == stageId);

            if (permitStage == null || string.IsNullOrEmpty(permitStage.AttachmentPath))
                return NotFound();

            string filePath;

            // Check if it's a web path (starts with /) or physical path
            if (permitStage.AttachmentPath.StartsWith("/"))
            {
                // Convert web path to physical path
                // Remove leading slash and replace forward slashes with backslashes
                var relativePath = permitStage.AttachmentPath.TrimStart('/').Replace("/", "\\");
                filePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            }
            else
            {
                // Already a physical path (old data)
                filePath = permitStage.AttachmentPath;
            }

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var fileName = Path.GetFileName(filePath);
            var contentType = "application/octet-stream";

            // Try detect content type for inline preview (pdf, jpg, png)
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);

            return File(System.IO.File.ReadAllBytes(filePath), contentType, fileName);
        }

        [HttpGet]
		public IActionResult UpdateStageGroup1(int id)
		{
			List<PermitStage> permits = permitStageService.GetAllPermitStages().Where(x => x.PermitId == id).ToList();
			UpdateStageGroup1View model = new UpdateStageGroup1View
			{
				PermitId = id,
				permitStages = permits,

				ContractReceiveAndAutharization = permits.FirstOrDefault(x => x.StageId == 2 && x.AttachmentPath != null) != null ? true : false,
				ContractReceiveAndAutharizationNotes = permits.FirstOrDefault(x => x.StageId == 2 && x.AttachmentPath != null) != null ? permits.FirstOrDefault(x => x.StageId == 2).Notes : null,
				ContractReceiveAndAutharizationTranactionNumber = permits.FirstOrDefault(x => x.StageId == 2 && x.AttachmentPath != null) != null ? permits.FirstOrDefault(x => x.StageId == 2).TransactionNumber : null,
				IsContractHasExistedImage = permits.FirstOrDefault(x => x.StageId == 2 && x.AttachmentPath != null) != null ? true : false,

				Validity = permits.FirstOrDefault(x => x.StageId == 3 && x.AttachmentPath != null) != null ? true : false,
				ValidityNotes = permits.FirstOrDefault(x => x.StageId == 3 && x.AttachmentPath != null) != null ? permits.FirstOrDefault(x => x.StageId == 3).Notes : null,
				ValidityTranactionNumber = permits.FirstOrDefault(x => x.StageId == 3 && x.AttachmentPath != null) != null ? permits.FirstOrDefault(x => x.StageId == 3).TransactionNumber : null,
				IsValidityHasExistedImage = permits.FirstOrDefault(x => x.StageId == 3 && x.AttachmentPath != null) != null ? true : false,
				TechReceive = permits.FirstOrDefault(x => x.StageId == 3 && x.AttachmentPath != null) != null ? true : false,
				EngineerReceive = permits.FirstOrDefault(x => x.StageId == 3 && x.AttachmentPath != null) != null ? true : false,
				ValidityReceive = permits.FirstOrDefault(x => x.StageId == 3 && x.AttachmentPath != null) != null ? true : false,

				LegalAffairs = permits.FirstOrDefault(x => x.StageId == 4 && x.AttachmentPath != null) != null ? true : false,
				LegalAffairsNotes = permits.FirstOrDefault(x => x.StageId == 4 && x.AttachmentPath != null) != null ? permits.FirstOrDefault(x => x.StageId == 4).Notes : null,
				LegalAffairsTranactionNumber = permits.FirstOrDefault(x => x.StageId == 4 && x.AttachmentPath != null) != null ? permits.FirstOrDefault(x => x.StageId == 4).TransactionNumber : null,
				IsLegalHasExistedImage = permits.FirstOrDefault(x => x.StageId == 4 && x.AttachmentPath != null) != null ? true : false,


				Surveying = permits.FirstOrDefault(x => x.StageId == 5 && x.AttachmentPath != null) != null ? true : false,
				SurveyingNotes = permits.FirstOrDefault(x => x.StageId == 5 && x.AttachmentPath != null) != null ? permits.FirstOrDefault(x => x.StageId == 5).Notes : null,
				SurveyingTranactionNumber = permits.FirstOrDefault(x => x.StageId == 5 && x.AttachmentPath != null) != null ? permits.FirstOrDefault(x => x.StageId == 5).TransactionNumber : null,
				IsSurveyingHasExistedImage = permits.FirstOrDefault(x => x.StageId == 5 && x.AttachmentPath != null) != null ? true : false,

				CompletingFile = permits.FirstOrDefault(x => x.StageId == 6 && x.AttachmentPath != null) != null ? true : false,
				CompletingFileNotes = permits.FirstOrDefault(x => x.StageId == 6 && x.AttachmentPath != null) != null ? permits.FirstOrDefault(x => x.StageId == 6).Notes : null,
				CompletingFileTranactionNumber = permits.FirstOrDefault(x => x.StageId == 6 && x.AttachmentPath != null) != null ? permits.FirstOrDefault(x => x.StageId == 6).TransactionNumber : null,
				IsCompletingHasExistedImage = permits.FirstOrDefault(x => x.StageId == 6 && x.AttachmentPath != null) != null ? true : false,

				FinishingFile = permits.FirstOrDefault(x => x.StageId == 7 && x.AttachmentPath != null) != null ? true : false,
				FinishingFileNotes = permits.FirstOrDefault(x => x.StageId == 7 && x.AttachmentPath != null) != null ? permits.FirstOrDefault(x => x.StageId == 7).Notes : null,
				FinishingFileTranactionNumber = permits.FirstOrDefault(x => x.StageId == 7 && x.AttachmentPath != null) != null ? permits.FirstOrDefault(x => x.StageId == 7).TransactionNumber : null,
				IsFinishingHasExistedImage = permits.FirstOrDefault(x => x.StageId == 7 && x.AttachmentPath != null) != null ? true : false,

			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> UpdateStageGroup1(UpdateStageGroup1View viewmodel)
		{
			if (ModelState.IsValid)
			{
				var PermitStages = permitStageService.GetAllPermitStages().Where(x => x.PermitId == viewmodel.PermitId).ToList();
				var rootPath = _env.ContentRootPath;//webrootpath
				string fullpath = Path.Combine(rootPath, "Uploads", viewmodel.PermitId.ToString());
				bool ValidityEntered = false;
				bool ContractEntered = false;
				bool LegalEntered = false;
				bool surveyingEntered = false;
				bool CompletingFileEntered = false;
				bool FinishingFileEntered = false;
				Directory.CreateDirectory(fullpath);
				foreach (var item in PermitStages)
				{
					if (viewmodel.IsContractHasChanges && item.StageId == 2 && ContractEntered == false)
					{
						item.AttachmentPath = viewmodel.IsContractHasImage ? await SaveFileAsync(viewmodel.ContractReceiveAndAutharizationImage!, fullpath, item.AttachmentPath) : item.AttachmentPath;
						item.CompletedAt = DateTime.Now;
						item.Notes = viewmodel.ContractReceiveAndAutharizationNotes;
						item.TransactionNumber = viewmodel.ContractReceiveAndAutharizationTranactionNumber;
						ContractEntered = true;
					}
					if (viewmodel.IsValidityHasChanges && item.StageId == 3 && ValidityEntered == false)
					{
						item.AttachmentPath = viewmodel.IsValidityHasImage ? await SaveFileAsync(viewmodel.ValidityImage!, fullpath, item.AttachmentPath) : item.AttachmentPath;
						item.CompletedAt = DateTime.Now;
						item.Notes = viewmodel.ValidityNotes;
						item.TransactionNumber = viewmodel.ValidityTranactionNumber;
						ValidityEntered = true;
					}
					if (viewmodel.IsLegalHasChanges && item.StageId == 4 && LegalEntered == false)
					{
						item.AttachmentPath = viewmodel.IsLegalAffairsHasImage ? await SaveFileAsync(viewmodel.LegalAffairsImage!, fullpath, item.AttachmentPath) : item.AttachmentPath;
						item.CompletedAt = DateTime.Now;
						item.Notes = viewmodel.LegalAffairsNotes;
						item.TransactionNumber = viewmodel.LegalAffairsTranactionNumber;
						LegalEntered = true;
					}
					if (viewmodel.IsSurveyingHasChanges && item.StageId == 5 && surveyingEntered == false)
					{
						item.AttachmentPath = viewmodel.IsSurveyingHasImage ? await SaveFileAsync(viewmodel.SurveyingImage!, fullpath, item.AttachmentPath) : item.AttachmentPath;
						item.CompletedAt = DateTime.Now;
						item.Notes = viewmodel.SurveyingNotes;
						item.TransactionNumber = viewmodel.SurveyingTranactionNumber;
						surveyingEntered = true;
					}
					if (viewmodel.IsCompletingHasChanges && item.StageId == 6 && CompletingFileEntered == false)
					{
						item.AttachmentPath = viewmodel.IsCompletingFileHasImage ? await SaveFileAsync(viewmodel.CompletingFileImage!, fullpath, item.AttachmentPath) : item.AttachmentPath;
						item.CompletedAt = DateTime.Now;
						item.Notes = viewmodel.CompletingFileNotes;
						item.TransactionNumber = viewmodel.CompletingFileTranactionNumber;
						CompletingFileEntered = true;
					}
					if (viewmodel.IsFinishingHasChanges && item.StageId == 7 && FinishingFileEntered == false)
					{
						item.AttachmentPath = viewmodel.IsFinishingFileHasImage ? await SaveFileAsync(viewmodel.FinishingFileImage!, fullpath, item.AttachmentPath) : item.AttachmentPath;
						item.CompletedAt = DateTime.Now;
						item.Notes = viewmodel.FinishingFileNotes;
						item.TransactionNumber = viewmodel.FinishingFileTranactionNumber;
						FinishingFileEntered = true;
					}
				}
				permitStageService.Save();
				TempData["StageGroup1"] = "تم تحديث جميع البيانات";
				bool goToStages2 = false;
				foreach (var item in PermitStages)
				{
					if (item.AttachmentPath is null)
					{
						goToStages2 = true;
					}
				}
				if (!goToStages2)
				{
					Permit permit = permitService.GetById(viewmodel.PermitId);
					permit.Current_Execution_Stage = 2;
					permitService.Save();
					List<Stage> secondgroup = stageService.Get_Second_Stages();
					foreach (var item in secondgroup)
					{
						permitStageService.AddPermitStage(new PermitStage
						{
							StageId = item.Id,
							PermitId = viewmodel.PermitId,
							StartedAt = DateTime.Now,
							DueAt = DateTime.Now.AddDays(item.DefaultDaysLimit),
                            isCheckedForNeglect = false,
                            isCheckedForOverDue = false
                        });
					}
					permitStageService.Save();
					return RedirectToAction("UpdateGroupStages2", new { id = viewmodel.PermitId });
				}
				return RedirectToAction("Index", "Client");
			}
			return View(viewmodel);
		}
		private async Task<string> SaveFileAsync(IFormFile file, string folderPath, string oldFile = "")
		{
			string Name = file.Name + Path.GetExtension(file.FileName);
			string fullPath = Path.Combine(folderPath, Name);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(oldFile);
			if (oldFile != "")
			{
				try
				{
					System.IO.File.Delete(oldFile);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				Console.ForegroundColor = ConsoleColor.White;
			}

			using (var stream = new FileStream(fullPath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}
			return fullPath;
		}
		[HttpGet]
		public IActionResult GroupStages2(int id)
		{
			var Permits = permitStageService.GetAllPermitStages().Where(x => x.PermitId == id && x.StageId >= 8 && x.StageId <= 10)
				.ToList();
			UpdateGroupStages2VM viewmodel = new UpdateGroupStages2VM
			{
				EngineerReview = Permits.FirstOrDefault(x => x.StageId == 8).AttachmentPath != null ? true : false,
				EngineerReviewNotes = Permits.FirstOrDefault(x => x.StageId == 8).Notes,
				isEngineerReviewHasExistedImage = Permits.FirstOrDefault(x => x.StageId == 8).AttachmentPath != null ? true : false,

				EngineerShadyReview = Permits.FirstOrDefault(x => x.StageId == 9).AttachmentPath != null ? true : false,
				EngineerShadyReviewNotes = Permits.FirstOrDefault(x => x.StageId == 9).Notes,
				isEngineerShadyHasExistedImage = Permits.FirstOrDefault(x => x.StageId == 9).AttachmentPath != null ? true : false,

				Syndicate = Permits.FirstOrDefault(x => x.StageId == 10).AttachmentPath != null ? true : false,
				SyndicateNotes = Permits.FirstOrDefault(x => x.StageId == 10).Notes,
				IsSyndicateHasExistedImage = Permits.FirstOrDefault(x => x.StageId == 10).AttachmentPath != null ? true : false,

			};
			return View(viewmodel);
		}

		[HttpGet]
		public IActionResult UpdateGroupStages2(int id)
		{
			var Permits = permitStageService.GetAllPermitStages().Where(x => x.PermitId == id && x.StageId >= 8 && x.StageId <= 10)
				.ToList();
			UpdateGroupStages2VM viewmodel = new UpdateGroupStages2VM
			{
				PermitId = id,
				PermitStages = Permits,
				EngineerReview = Permits.FirstOrDefault(x => x.StageId == 8).AttachmentPath != null ? true : false,
				EngineerReviewNotes = Permits.FirstOrDefault(x => x.StageId == 8).Notes,
				isEngineerReviewHasExistedImage = Permits.FirstOrDefault(x => x.StageId == 8).AttachmentPath != null ? true : false,

				EngineerShadyReview = Permits.FirstOrDefault(x => x.StageId == 9).AttachmentPath != null ? true : false,
				EngineerShadyReviewNotes = Permits.FirstOrDefault(x => x.StageId == 9).Notes,
				isEngineerShadyHasExistedImage = Permits.FirstOrDefault(x => x.StageId == 9).AttachmentPath != null ? true : false,

				Syndicate = Permits.FirstOrDefault(x => x.StageId == 10).AttachmentPath != null ? true : false,
				SyndicateNotes = Permits.FirstOrDefault(x => x.StageId == 10).Notes,
				IsSyndicateHasExistedImage = Permits.FirstOrDefault(x => x.StageId == 10).AttachmentPath != null ? true : false,

			};
			return View(viewmodel);
		}
		[HttpPost]
		public async Task<IActionResult> UpdateGroupStages2(UpdateGroupStages2VM viewmodel)
		{
			if (ModelState.IsValid)
			{
				var PermitStages = permitStageService.GetAllPermitStages().Where(x => x.PermitId == viewmodel.PermitId &&
				x.StageId >= 8 && x.StageId <= 10).ToList();
				var rootPath = _env.ContentRootPath;//webrootpath
				string fullpath = Path.Combine(rootPath, "Uploads", viewmodel.PermitId.ToString());
				bool EngineerEntered = false;
				bool ShadyEntered = false;
				bool SyndicateEntered = false;
				Directory.CreateDirectory(fullpath);
				foreach (var item in PermitStages)
				{
					if (viewmodel.isEngineerReviewHasChanges && item.StageId == 8 && EngineerEntered == false)
					{
						item.AttachmentPath = viewmodel.EngineerReviewImage != null ? await SaveFileAsync(viewmodel.EngineerReviewImage, fullpath, item.AttachmentPath) : await SaveFileAsync(viewmodel.EngineerReviewImage, fullpath);
						item.CompletedAt = DateTime.Now;
						item.Notes = viewmodel.EngineerReviewNotes;
						EngineerEntered = true;
					}
					if (viewmodel.isEngineerShadyHasChanges && item.StageId == 9 && ShadyEntered == false)
					{
						item.AttachmentPath = viewmodel.EngineerShadyReviewImage != null ? await SaveFileAsync(viewmodel.EngineerShadyReviewImage, fullpath, item.AttachmentPath) : await SaveFileAsync(viewmodel.EngineerShadyReviewImage, fullpath);
						item.CompletedAt = DateTime.Now;
						item.Notes = viewmodel.EngineerShadyReviewNotes;
						ShadyEntered = true;
					}
					if (viewmodel.isSyndicateHasChanges && item.StageId == 10 && SyndicateEntered == false)
					{
						item.AttachmentPath = viewmodel.SyndicateImage != null ? await SaveFileAsync(viewmodel.SyndicateImage, fullpath, item.AttachmentPath) : await SaveFileAsync(viewmodel.SyndicateImage, fullpath);
						item.CompletedAt = DateTime.Now;
						item.Notes = viewmodel.SyndicateNotes;
						SyndicateEntered = true;
					}
				}
				permitStageService.Save();
				bool goToStage3 = false;
				foreach (var item in PermitStages)
				{
					if (item.AttachmentPath is null)
						goToStage3 = true;
				}
				if (!goToStage3)
				{
					Permit permit = permitService.GetById(viewmodel.PermitId);
					permit.Current_Execution_Stage = 3;
					permitService.Save();
					List<Stage> secondgroup = stageService.Get_Third_Stages();
					foreach (var item in secondgroup)
					{
						permitStageService.AddPermitStage(new PermitStage
						{
							StageId = item.Id,
							PermitId = viewmodel.PermitId,
							StartedAt = DateTime.Now,
							DueAt = DateTime.Now.AddDays(item.DefaultDaysLimit),
							isCheckedForNeglect = false,
							isCheckedForOverDue = false
						});
					}
					permitStageService.Save();
					return RedirectToAction("UpdateGroupStages3", new { id = viewmodel.PermitId });
				}
				TempData["StageGroup1"] = "تم تحديث جميع البيانات";
				return RedirectToAction("Index", "Client");
			}
			return RedirectToAction("Index", "Client");
		}
		[HttpGet]
		public IActionResult UpdateGroupStages3(int id)
		{
			var permitStages = permitStageService.GetAllPermitStages().Where(x => x.PermitId == id && x.StageId >= 11 && x.StageId <= 12).ToList();
			UpdategroupStage3 viewmodel = new UpdategroupStage3
			{
				PermitId = id,
				PermitStages = permitStages,

				EngineerMansourReview = permitStages.FirstOrDefault(x => x.StageId == 11).AttachmentPath != null ? true : false,
				IsMansourHasExistedImage = permitStages.FirstOrDefault(x => x.StageId == 11).AttachmentPath != null ? true : false,
				MasourNotes = permitStages.FirstOrDefault(x => x.StageId == 11).Notes,

				FinalPayOrder = permitStages.FirstOrDefault(x => x.StageId == 12).AttachmentPath != null ? true : false,
				FinalPayTransactionNumber = permitStages.FirstOrDefault(x => x.StageId == 12).TransactionNumber,
				FinalPayNotes = permitStages.FirstOrDefault(x => x.StageId == 12).Notes,
				IsFinalPayHasExistedImage = permitStages.FirstOrDefault(x => x.StageId == 12).AttachmentPath != null ? true : false,


			};
			return View(viewmodel);
		}
		[HttpPost]
        public async Task<IActionResult> UpdateGroupStages3(UpdategroupStage3 viewmodel)
        {
			if(ModelState.IsValid)
            {
                var PermitStages = permitStageService.GetAllPermitStages().Where(x => x.PermitId == viewmodel.PermitId &&
				x.StageId >= 11 && x.StageId <= 12).ToList();
                var rootPath = _env.ContentRootPath;//webrootpath
                string fullpath = Path.Combine(rootPath, "Uploads", viewmodel.PermitId.ToString());
                bool EngineerEntered = false;
                bool ShadyEntered = false;
                Directory.CreateDirectory(fullpath);
                foreach (var item in PermitStages)
                {
                    if (viewmodel.IsMansourHasChanges && item.StageId == 11 && EngineerEntered == false)
                    {
                        item.AttachmentPath = viewmodel.EngineerMansourReview != null ? await SaveFileAsync(viewmodel.EngineerMansourImage, fullpath, item.AttachmentPath) : item.AttachmentPath;
                        item.CompletedAt = DateTime.Now;
                        item.Notes = viewmodel.MasourNotes;
                        EngineerEntered = true;
                    }
                    if (viewmodel.IsFinalPayHasChanges && item.StageId == 12 && ShadyEntered == false)
                    {
                        item.AttachmentPath = viewmodel.FinalPayOrderImage != null ? await SaveFileAsync(viewmodel.FinalPayOrderImage, fullpath, item.AttachmentPath) : item.AttachmentPath;
                        item.CompletedAt = DateTime.Now;
                        item.Notes = viewmodel.FinalPayNotes;
						item.TransactionNumber = viewmodel.FinalPayTransactionNumber;
                        ShadyEntered = true;
                    }
                }
                permitStageService.Save();
				bool goToStage4 = false;
				foreach (var item in PermitStages)
				{
					if (item.AttachmentPath is null)
						goToStage4 = true;
				}
				if (!goToStage4)
				{
					Permit permit = permitService.GetById(viewmodel.PermitId);
					permit.Current_Execution_Stage = 4;
					permitService.Save();
					List<Stage> secondgroup = stageService.Get_Fourth_Stages();
					foreach (var item in secondgroup)
					{
						permitStageService.AddPermitStage(new PermitStage
						{
							StageId = item.Id,
							PermitId = viewmodel.PermitId,
							StartedAt = DateTime.Now,
							DueAt = DateTime.Now.AddDays(item.DefaultDaysLimit),
                            isCheckedForNeglect = false,
							isCheckedForOverDue = false
                        });
					}
					permitStageService.Save();
					return RedirectToAction("UpdateGroupStages4", new { id = viewmodel.PermitId });
				}
				TempData["StageGroup1"] = "تم تحديث جميع البيانات";
                return RedirectToAction("Index", "Client");
            }
            return View(viewmodel);
        }
		[HttpGet]
		public IActionResult UpdateGroupStages4(int id)
		{
            var permitStages = permitStageService.GetAllPermitStages().Where(x => x.PermitId == id && x.StageId >= 13 && x.StageId <= 14).ToList();
            UpdategroupStage4 viewmodel = new UpdategroupStage4
            {
                PermitId = id,
                PermitStages = permitStages,

				Printing = permitStages.FirstOrDefault(x => x.StageId == 13).AttachmentPath != null ? true : false,
                IsPrintingHasExistedImage = permitStages.FirstOrDefault(x => x.StageId == 13).AttachmentPath != null ? true : false,
				
            };
            return View(viewmodel);
        }
		[HttpPost]
		public async Task<IActionResult> UpdateGroupStages4(UpdategroupStage4 viewmodel)
		{
			if(ModelState.IsValid)
			{
                var PermitStages = permitStageService.GetAllPermitStages().Where(x => x.PermitId == viewmodel.PermitId &&
                x.StageId >= 13 && x.StageId <= 14).ToList();
                var rootPath = _env.ContentRootPath;//webrootpath
                string fullpath = Path.Combine(rootPath, "Uploads", viewmodel.PermitId.ToString());
                bool EngineerEntered = false;
                bool ShadyEntered = false;
                Directory.CreateDirectory(fullpath);
                foreach (var item in PermitStages)
                {
                    if (viewmodel.IsPrintingHasChanges && item.StageId == 13 && EngineerEntered == false)
                    {
                        item.AttachmentPath = viewmodel.PrintingImage != null ? await SaveFileAsync(viewmodel.PrintingImage, fullpath, item.AttachmentPath) : item.AttachmentPath;
                        item.CompletedAt = DateTime.Now;
                        EngineerEntered = true;
                    }
                    if (viewmodel.IsPermitReceived && item.StageId == 14 && ShadyEntered == false)
					{
						item.CompletedAt=DateTime.Now;
						ShadyEntered=true;
					}
                }
                permitStageService.Save();
                bool goToStageArchieve = false;
                foreach (var item in PermitStages)
                {
                    if (item.CompletedAt is null)
                        goToStageArchieve = true;
                }
                if (!goToStageArchieve)
                {
                    Permit permit = permitService.GetById(viewmodel.PermitId);
                    permit.Current_Execution_Stage = 5;
					permit.Status = PermitStatus.Archieved;
					permit.ArchivedAt = DateTime.Now;
                    permitService.Save();
					string Name = clientService.GetAllClients().FirstOrDefault(x => x.PermitId == viewmodel.PermitId).FullName;
                    return RedirectToAction("Index", "Client");
                }
                TempData["StageGroup4"] = "تم إنجاز جميع البنود الخاصة بالرخصة ... تم تحويلها إلى قسم الأرشيف";
                return RedirectToAction("Index", "Client");
            }
			return View(viewmodel);
		}
		[HttpGet]
		public IActionResult Archieve()
		{
			var model=  permitService.GetAllPermits().Where(x => x.Status.ToString() == "Archieved").Select(
				x => new ArchiveModel
				{
					clientName=clientService.GetByPermitId(x.Id).FullName,
					EngineerName=context.Users.FirstOrDefault(y=>y.Id==x.CreatedBy).FullName,
					CreatedAt= x.CreatedAt,
					finishedAt=x.ArchivedAt,
					NumOfDaysSpent=(x.ArchivedAt-x.CreatedAt).Value.Days,
					PermitType = x.PermitType,
					Costs= costService.GetTotalPermitCosts(x.Id),
					AgreedOn=expenseService.GetPermitById(x.Id)!=null?expenseService.GetPermitById(x.Id).AgreementAmount:0,
					Received=expenseService.GetPermitById(x.Id)!=null ? expenseService.GetPermitById(x.Id).ReceivedAmount:0,
					Remaining=expenseService.GetPermitById(x.Id) != null ? expenseService.GetPermitById(x.Id).RemainingAmount : 0,
					id= x.Id
				});
			return View(model);
		}
		[HttpGet]
		public IActionResult searchArchieve(string q)
		{
			var model = permitService.GetAllPermits().Where(x => x.Status.ToString() == "Archieved").Select(
				x => new ArchiveModel
				{
					clientName = clientService.GetByPermitId(x.Id).FullName,
					EngineerName = context.Users.FirstOrDefault(y => y.Id == x.CreatedBy).FullName,
					CreatedAt = x.CreatedAt,
					finishedAt = x.ArchivedAt,
					NumOfDaysSpent = (x.ArchivedAt - x.CreatedAt).Value.Days,
					PermitType = x.PermitType,
					Costs = costService.GetTotalPermitCosts(x.Id),
					AgreedOn = expenseService.GetPermitById(x.Id) != null ? expenseService.GetPermitById(x.Id).AgreementAmount : 0,
					Received = expenseService.GetPermitById(x.Id) != null ? expenseService.GetPermitById(x.Id).ReceivedAmount : 0,
					Remaining = expenseService.GetPermitById(x.Id) != null ? expenseService.GetPermitById(x.Id).RemainingAmount : 0
				}).ToList();
			var result = string.IsNullOrEmpty(q) ? model :
				model.Where(x => x.clientName.ToLower().Contains(q.ToLower()) ||
				x.EngineerName.ToLower().Contains(q.ToLower()) || x.PermitType.ToLower().Contains(q.ToLower())).ToList();
			return PartialView("_ArchieveTable", result);
		}
	}
}