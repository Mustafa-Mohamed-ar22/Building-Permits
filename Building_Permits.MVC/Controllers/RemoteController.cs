using Building_Permits.Core.Entities;
using Building_Permits.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Building_Permits.MVC.Controllers
{
    public class RemoteController : Controller
    {
        private AppDbContext _context;
        public RemoteController(AppDbContext _context)
        {
            this._context = _context;
        }
        #region ExistedImageAddition

        public ActionResult isValidValidity2(bool Validity, bool ValidityReceive, bool TechReceive,
            bool EngineerReceive, bool IsValidityHasImage, string? ValidityTranactionNumber,
            bool IsValidityHasExistedImage, int PermitId)
        {
            if (IsValidityHasExistedImage && IsValidityHasImage && ValidityTranactionNumber is not null && TechReceive && EngineerReceive && ValidityReceive && Validity)
                return Json(true);
            if (!Validity && ValidityTranactionNumber is null && !IsValidityHasImage && !TechReceive && !EngineerReceive && !ValidityReceive)
                return Json(true);
            else
            {
                if (IsValidityHasExistedImage)
                {
                    if (EngineerReceive && ValidityReceive && TechReceive && Validity)
                    {
                        if (ValidityTranactionNumber != null)
                        {
                            var existing = _context.PermitStages
                                .FirstOrDefault(x => x.TransactionNumber == ValidityTranactionNumber);

                            if (existing == null || existing.StageId == 3)
                            {
                                return Json(true);
                            }
                            else
                            {
                                return Json("رقم المعاملة مستخدم في سجل آخر");
                            }
                        }
                        else return Json("من فضلك أرفق رقم المعاملة");
                    }
                    else return Json(false);
                }
                else
                {
                    if (Validity && EngineerReceive && ValidityReceive && TechReceive && IsValidityHasImage && ValidityTranactionNumber != null)
                    {
                        var existing = _context.PermitStages
                            .Any(x => x.TransactionNumber == ValidityTranactionNumber);
                        if (existing)
                        {
                            return Json("رقم المعاملة مستخدم في سجل آخر");
                        }
                        else
                        {
                            return Json(true);
                        }
                    }
                    else return Json("من فضلك إذا تم الانتهاء من البند أرفق البيانات المطلوبة من رقم معاملة وصورة بيان الصلاحية");
                }
            }

        }
        public ActionResult isValidContract2(bool ContractReceiveAndAutharization, bool IsContractHasImage, string? ContractReceiveAndAutharizationTranactionNumber, bool IsContractHasExistedImage)
        {
            if (IsContractHasExistedImage && IsContractHasImage && ContractReceiveAndAutharizationTranactionNumber != null
                && ContractReceiveAndAutharization)
            {
                return Json(true);
            }
            if (IsContractHasExistedImage)
            {
                if (ContractReceiveAndAutharization && ContractReceiveAndAutharizationTranactionNumber != null)
                {
                    var existed = _context.PermitStages.FirstOrDefault(x => x.TransactionNumber == ContractReceiveAndAutharizationTranactionNumber);
                    if (existed == null || existed.StageId == 2)
                    {
                        return Json(true);
                    }
                    else return Json("رقم المعاملة مستخدم فى سجل آخر");

                }
                else return Json("من فضلك إذا تم إنجاز البند من فضلك اضفط على الصندوق وأدخل رقم المعاملة");
            }
            else
            {
                if (!ContractReceiveAndAutharization && !IsContractHasImage && ContractReceiveAndAutharizationTranactionNumber is null)
                {
                    return Json(true);
                }
                if (ContractReceiveAndAutharization && IsContractHasImage && ContractReceiveAndAutharizationTranactionNumber is not null)
                {
                    var existing = _context.PermitStages
                        .Any(x => x.TransactionNumber == ContractReceiveAndAutharizationTranactionNumber);
                    if (existing)
                    {
                        return Json("يجب أن يكون رقم المعاملة رقم مميز");
                    }

                    return Json(true);
                }
                else if (ContractReceiveAndAutharization && IsContractHasImage && ContractReceiveAndAutharizationTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة");
                }

                else if (ContractReceiveAndAutharization && !IsContractHasImage && ContractReceiveAndAutharizationTranactionNumber is not null)
                {
                    return Json("من فضلك أرفق صورة من العقد والتوكيل");
                }

                else if (ContractReceiveAndAutharization && !IsContractHasImage && ContractReceiveAndAutharizationTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة وصورة من العقد والتوكيل");
                }
                else if (!ContractReceiveAndAutharization && IsContractHasImage && ContractReceiveAndAutharizationTranactionNumber is not null)
                {
                    return Json("إذا تم إنجاز البند وبياناته مرفقة، من فضلك اضغط على الصندوق");
                }
                else if (!ContractReceiveAndAutharization && IsContractHasImage && ContractReceiveAndAutharizationTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة أو قم بإلغاء تحديد الصورة");
                }
                else if (!ContractReceiveAndAutharization && !IsContractHasImage && ContractReceiveAndAutharizationTranactionNumber is not null)
                {
                    return Json("من فضلك قم بحذف رقم المعاملة أو اضغط على الصندوق وأرفق الصورة");
                }
                else
                {
                    return Json("حالة غير صحيحة، يرجى مراجعة البيانات المدخلة");
                }

            }
        }

        public ActionResult isValidLegalAffairs2(bool LegalAffairs, bool IsLegalAffairsHasImage, string? LegalAffairsTranactionNumber, bool IsLegalHasExistedImage, int StageId)
        {
            if (IsLegalAffairsHasImage && IsLegalHasExistedImage && LegalAffairsTranactionNumber is not null && LegalAffairs)
                return Json(true);
            if (IsLegalHasExistedImage)
            {
                if (LegalAffairs)
                {
                    if (LegalAffairsTranactionNumber != null)
                    {
                        var existing = _context.PermitStages
                        .FirstOrDefault(x => x.TransactionNumber == LegalAffairsTranactionNumber);

                        if (existing == null || existing.StageId == StageId)
                        {
                            return Json(true);
                        }
                        else
                        {
                            return Json("رقم المعاملة مستخدم في سجل آخر");
                        }
                    }
                    else
                    {
                        return Json("من فضلك أرفق رقم المعاملة");
                    }
                }
                else return Json("إذا تم إنجاز البند من فضلك اضفط على الصندوق");
            }
            else
            {
                if (!LegalAffairs && !IsLegalAffairsHasImage && LegalAffairsTranactionNumber is null)
                    return Json(true);
                if (LegalAffairs && IsLegalAffairsHasImage && LegalAffairsTranactionNumber is not null)
                {
                    var existing = _context.PermitStages
                        .Any(x => x.TransactionNumber == LegalAffairsTranactionNumber);
                    if (existing)
                    {
                        return Json("يجب أن يكون رقم المعاملة رقم مميز");
                    }
                    return Json(true);
                }
                else if (LegalAffairs && IsLegalAffairsHasImage && LegalAffairsTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة");
                }

                else if (LegalAffairs && !IsLegalAffairsHasImage && LegalAffairsTranactionNumber is not null)
                {
                    return Json("من فضلك أرفق صورة من إيصال الشئون القانونية");
                }

                else if (LegalAffairs && !IsLegalAffairsHasImage && LegalAffairsTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة وصورة من إيصال الشئون القانونية");
                }
                else if (!LegalAffairs && IsLegalAffairsHasImage && LegalAffairsTranactionNumber is not null)
                {
                    return Json("إذا تم إنجاز البند وبياناته مرفقة، من فضلك اضغط على الصندوق");
                }
                else if (!LegalAffairs && IsLegalAffairsHasImage && LegalAffairsTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة أو قم بإلغاء تحديد الصورة");
                }
                else if (!LegalAffairs && !IsLegalAffairsHasImage && LegalAffairsTranactionNumber is not null)
                {
                    return Json("من فضلك قم بحذف رقم المعاملة أو اضغط على الصندوق وأرفق الصورة");
                }
                else
                {
                    return Json("حالة غير صحيحة، يرجى مراجعة البيانات المدخلة");
                }
            }
        }

        public ActionResult isValidSurveying2(bool Surveying, bool IsSurveyingHasImage, string? SurveyingTranactionNumber, bool IsSurveyingHasExistedImage)
        {
            if (Surveying && IsSurveyingHasExistedImage && IsSurveyingHasImage && SurveyingTranactionNumber is not null) return Json(true);
            if (IsSurveyingHasExistedImage)
            {
                if (Surveying)
                {
                    if (SurveyingTranactionNumber != null)
                    {
                        var existing = _context.PermitStages
                        .FirstOrDefault(x => x.TransactionNumber == SurveyingTranactionNumber);

                        if (existing == null || existing.StageId == 5)
                        {
                            return Json(true);
                        }
                        else
                        {
                            return Json("رقم المعاملة مستخدم في سجل آخر");
                        }
                    }
                    else
                    {
                        return Json("من فضلك أرفق رقم المعاملة");
                    }
                }
                else return Json("إذا تم إنجاز البند من فضلك اضفط على الصندوق");
            }
            else
            {
                if (!Surveying && !IsSurveyingHasImage && SurveyingTranactionNumber is null)
                    return Json(true);
                if (Surveying && IsSurveyingHasImage && SurveyingTranactionNumber is not null)
                {
                    var existing = _context.PermitStages
                        .Any(x => x.TransactionNumber == SurveyingTranactionNumber);
                    if (existing)
                    {
                        return Json("يجب أن يكون رقم المعاملة رقم مميز");
                    }
                    return Json(true);
                }
                else if (Surveying && IsSurveyingHasImage && SurveyingTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة");
                }

                else if (Surveying && !IsSurveyingHasImage && SurveyingTranactionNumber is not null)
                {
                    return Json("من فضلك أرفق صورة من بيان الرفع المساحى");
                }

                else if (Surveying && !IsSurveyingHasImage && SurveyingTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة وصورة من إبيان الرفع المساحى");
                }
                else if (!Surveying && IsSurveyingHasImage && SurveyingTranactionNumber is not null)
                {
                    return Json("إذا تم إنجاز البند وبياناته مرفقة، من فضلك اضغط على الصندوق");
                }
                else if (!Surveying && IsSurveyingHasImage && SurveyingTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة أو قم بإلغاء تحديد الصورة");
                }
                else if (!Surveying && !IsSurveyingHasImage && SurveyingTranactionNumber is not null)
                {
                    return Json("من فضلك قم بحذف رقم المعاملة أو اضغط على الصندوق وأرفق الصورة");
                }
                else
                {
                    return Json("حالة غير صحيحة، يرجى مراجعة البيانات المدخلة");
                }
            }
        }
        public ActionResult isvalidCompleteingFile2(bool CompletingFile, bool IsCompletingFileHasImage, string? CompletingFileTranactionNumber, bool IsCompletingHasExistedImage)
        {

            if (IsCompletingFileHasImage && IsCompletingHasExistedImage && CompletingFileTranactionNumber is not null && CompletingFile)
                return Json(true);
            if (IsCompletingHasExistedImage)
            {
                if (CompletingFile)
                {
                    if (CompletingFileTranactionNumber != null)
                    {
                        var existing = _context.PermitStages
                        .FirstOrDefault(x => x.TransactionNumber == CompletingFileTranactionNumber);

                        if (existing == null || existing.StageId == 6)
                        {
                            return Json(true);
                        }
                        else
                        {
                            return Json("رقم المعاملة مستخدم في سجل آخر");
                        }
                    }
                    else
                    {
                        return Json("من فضلك أرفق رقم المعاملة");
                    }
                }
                else return Json("إذا تم إنجاز البند من فضلك اضفط على الصندوق");
            }
            else
            {
                if (!CompletingFile && !IsCompletingFileHasImage && CompletingFileTranactionNumber is null)
                    return Json(true);
                if (CompletingFile && IsCompletingFileHasImage && CompletingFileTranactionNumber is not null)
                {
                    var existing = _context.PermitStages
                        .Any(x => x.TransactionNumber == CompletingFileTranactionNumber);
                    if (existing)
                    {
                        return Json("يجب أن يكون رقم المعاملة رقم مميز");
                    }
                    return Json(true);
                }
                else if (CompletingFile && IsCompletingFileHasImage && CompletingFileTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة");
                }

                else if (CompletingFile && !IsCompletingFileHasImage && CompletingFileTranactionNumber is not null)
                {
                    return Json("من فضلك أرفق صورة من تقفيل الملف");
                }

                else if (CompletingFile && !IsCompletingFileHasImage && CompletingFileTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة وصورة من تقفيل الملف");
                }
                else if (!CompletingFile && IsCompletingFileHasImage && CompletingFileTranactionNumber is not null)
                {
                    return Json("إذا تم إنجاز البند وبياناته مرفقة، من فضلك اضغط على الصندوق");
                }
                else if (!CompletingFile && IsCompletingFileHasImage && CompletingFileTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة أو قم بإلغاء تحديد الصورة");
                }
                else if (!CompletingFile && !IsCompletingFileHasImage && CompletingFileTranactionNumber is not null)
                {
                    return Json("من فضلك قم بحذف رقم المعاملة أو اضغط على الصندوق وأرفق الصورة");
                }
                else
                {
                    return Json("حالة غير صحيحة، يرجى مراجعة البيانات المدخلة");
                }
            }
        }
        public ActionResult isValidFinishingFile2(bool FinishingFile, bool IsFinishingFileHasImage, string? FinishingFileTranactionNumber, bool IsFinishingHasExistedImage)
        {
            if (IsFinishingFileHasImage && IsFinishingHasExistedImage && FinishingFileTranactionNumber is not null && FinishingFile)
                return Json(true);
            if (IsFinishingHasExistedImage)
            {
                if (FinishingFile)
                {
                    if (FinishingFileTranactionNumber != null)
                    {
                        var existing = _context.PermitStages
                        .FirstOrDefault(x => x.TransactionNumber == FinishingFileTranactionNumber);

                        if (existing == null || existing.StageId == 7)
                        {
                            return Json(true);
                        }
                        else
                        {
                            return Json("رقم المعاملة مستخدم في سجل آخر");
                        }
                    }
                    else
                    {
                        return Json("من فضلك أرفق رقم المعاملة");
                    }
                }
                else return Json("إذا تم إنجاز البند من فضلك اضفط على الصندوق");
            }
            else
            {
                if (!FinishingFile && !IsFinishingFileHasImage && FinishingFileTranactionNumber is null)
                    return Json(true);
                if (FinishingFile && IsFinishingFileHasImage && FinishingFileTranactionNumber is not null)
                {
                    var existing = _context.PermitStages
                        .Any(x => x.TransactionNumber == FinishingFileTranactionNumber);
                    if (existing)
                    {
                        return Json("يجب أن يكون رقم المعاملة رقم مميز");
                    }
                    return Json(true);
                }
                else if (FinishingFile && IsFinishingFileHasImage && FinishingFileTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة");
                }

                else if (FinishingFile && !IsFinishingFileHasImage && FinishingFileTranactionNumber is not null)
                {
                    return Json("من فضلك أرفق صورة من تقديم الملف");
                }

                else if (FinishingFile && !IsFinishingFileHasImage && FinishingFileTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة وصورة من تقديم الملف");
                }
                else if (!FinishingFile && IsFinishingFileHasImage && FinishingFileTranactionNumber is not null)
                {
                    return Json("إذا تم إنجاز البند وبياناته مرفقة، من فضلك اضغط على الصندوق");
                }
                else if (!FinishingFile && IsFinishingFileHasImage && FinishingFileTranactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة أو قم بإلغاء تحديد الصورة");
                }
                else if (!FinishingFile && !IsFinishingFileHasImage && FinishingFileTranactionNumber is not null)
                {
                    return Json("من فضلك قم بحذف رقم المعاملة أو اضغط على الصندوق وأرفق الصورة");
                }
                else
                {
                    return Json("حالة غير صحيحة، يرجى مراجعة البيانات المدخلة");
                }
            }
        }
        #endregion




        #region ValidValueswithimagesandTN
        public ActionResult isValidValidity(bool Validity, bool? ValidityReceive, bool? TechReceive, bool? EngineerReceive, bool? IsValidityHasImage, string? ValidityTranactionNumber)
        {
            bool isValid =
                (Validity == true && ValidityReceive == true && TechReceive == true && EngineerReceive == true && IsValidityHasImage == true && ValidityTranactionNumber is not null) ||
                (Validity == false && ValidityReceive == false && TechReceive == false && EngineerReceive == false && IsValidityHasImage == false && ValidityTranactionNumber is null);

            return Json(isValid);
        }
        public ActionResult isValidContract(bool ContractReceiveAndAutharization, bool? IsContractHasImage, string? ContractReceiveAndAutharizationTranactionNumber)
        {
            bool isValid = (ContractReceiveAndAutharization == true && IsContractHasImage == true && ContractReceiveAndAutharizationTranactionNumber is not null) ||
                (ContractReceiveAndAutharization == false && IsContractHasImage == true && ContractReceiveAndAutharizationTranactionNumber is null);
            return Json(isValid);
        }


        public ActionResult isValidLegalAffairs(bool LegalAffairs, bool? IsLegalAffairsHasImage, string? LegalAffairsTranactionNumber)
        {
            bool isValid = (LegalAffairs == true && IsLegalAffairsHasImage == true && LegalAffairsTranactionNumber is not null) ||
                (LegalAffairs == false && IsLegalAffairsHasImage == false && LegalAffairsTranactionNumber is null);
            return Json(isValid);
        }

        public ActionResult isValidSurveying(bool Surveying, bool? IsSurveyingHasImage, string? SurveyingTranactionNumber)
        {
            bool isValid = (Surveying == true && IsSurveyingHasImage == true && SurveyingTranactionNumber is not null) ||
                (Surveying == false && IsSurveyingHasImage == false && SurveyingTranactionNumber is null);
            return Json(isValid);
        }
        public ActionResult isvalidCompleteingFile(bool CompletingFile, bool? IsCompletingFileHasImage, string? CompletingFileTranactionNumber)
        {
            bool isValid = (CompletingFile == true && IsCompletingFileHasImage == true && CompletingFileTranactionNumber is not null) ||
                (CompletingFile == false && IsCompletingFileHasImage == false && CompletingFileTranactionNumber is null);
            return Json(isValid);
        }
        public ActionResult isValidFinishingFile(bool FinishingFile, bool? IsFinishingFileHasImage, string? FinishingFileTranactionNumber)
        {
            bool isValid = (FinishingFile == true && IsFinishingFileHasImage == true && FinishingFileTranactionNumber is not null)
                || (FinishingFile == false && IsFinishingFileHasImage == false && FinishingFileTranactionNumber is null);
            return Json(isValid);
        }
        #endregion



        #region TransactionNumber
        public ActionResult isValidTN(string? ContractReceiveAndAutharizationTranactionNumber, bool IsContractHasExistedImage, bool IsContractHasImage)
        {
            if (ContractReceiveAndAutharizationTranactionNumber is null && IsContractHasImage == false)
            {
                return Json(true);
            }
            if (IsContractHasExistedImage)
            {
                if (ContractReceiveAndAutharizationTranactionNumber is null) return Json(false);
                bool result = _context.PermitStages.SingleOrDefault(x => x.TransactionNumber == ContractReceiveAndAutharizationTranactionNumber) is null;
                return Json(result);
            }
            else
            {
                bool res = _context.PermitStages.Any(x => x.TransactionNumber == ContractReceiveAndAutharizationTranactionNumber);
                return Json(!res);
            }
        }
        //public ActionResult isValidTN2(string? ValidityTranactionNumber, bool IsValidityHasExistedImage, bool IsValidityHasImage, int PermitId)
        //{
        //	if (IsValidityHasExistedImage)
        //	{
        //		if (ValidityTranactionNumber != null)
        //		{
        //			var existing = _context.PermitStages
        //				.FirstOrDefault(x => x.TransactionNumber == ValidityTranactionNumber);

        //			if (existing == null || existing.Id == PermitId)
        //			{
        //				return Json(true);
        //			}
        //			else
        //			{
        //				return Json("رقم المعاملة مستخدم في سجل آخر");
        //			}

        //		}
        //		else return Json("من فضلك أرفق رقم المعاملة");
        //	}
        //	else
        //	{
        //		if (IsValidityHasImage)
        //		{
        //			if (ValidityTranactionNumber != null)
        //			{
        //				var existing = _context.PermitStages
        //					.FirstOrDefault(x => x.TransactionNumber == ValidityTranactionNumber);

        //				if (existing != null && existing.Id != PermitId)
        //				{
        //					return Json("يجب أن يكون رقم المعاملة رقم مميز");
        //				}
        //				return Json(true);
        //			}
        //			else return Json("من فضلك أرفق رقم المعاملة");
        //		}
        //	}
        //}


        public ActionResult isValidTN4(string? SurveyingTranactionNumber, bool IsSurveyingHasExistedImage)
        {
            if (SurveyingTranactionNumber == null)
            {
                return Json("من فضلك أدخل رقم المعاملة");
            }
            if (IsSurveyingHasExistedImage && _context.PermitStages.SingleOrDefault(x => x.TransactionNumber == SurveyingTranactionNumber) != null)
            {
                return Json(true);
            }
            bool isValid = _context.PermitStages.Any(x => x.TransactionNumber == SurveyingTranactionNumber);
            return Json(!isValid);
        }
        public ActionResult isValidTN5(string? CompletingFileTranactionNumber, bool IsCompletingHasExistedImage)
        {
            if (CompletingFileTranactionNumber == null)
            {
                return Json("من فضلك أدخل رقم المعاملة");
            }
            if (IsCompletingHasExistedImage && _context.PermitStages.SingleOrDefault(x => x.TransactionNumber == CompletingFileTranactionNumber) != null)
            {
                return Json(true);
            }
            bool isValid = _context.PermitStages.Any(x => x.TransactionNumber == CompletingFileTranactionNumber);
            return Json(!isValid);
        }
        public ActionResult isValidTN6(string? FinishingFileTranactionNumber, bool IsFinishingHasExistedImage)
        {
            if (FinishingFileTranactionNumber == null)
            {
                return Json("من فضلك أدخل رقم المعاملة");
            }
            if (IsFinishingHasExistedImage && _context.PermitStages.SingleOrDefault(x => x.TransactionNumber == FinishingFileTranactionNumber) != null)
            {
                return Json(true);
            }
            bool isValid = _context.PermitStages.Any(x => x.TransactionNumber == FinishingFileTranactionNumber);
            return Json(!isValid);
        }
        #endregion


        #region SecondStagesValidation

        public ActionResult isValidEngineerReview(bool EngineerReview, bool IsEngineerReviewHasImage, bool isEngineerReviewHasExistedImage)
        {
            if (EngineerReview && isEngineerReviewHasExistedImage)
                return Json(true);
            if (isEngineerReviewHasExistedImage)
            {
                if (EngineerReview)
                {
                    if (IsEngineerReviewHasImage)
                    {
                        return Json(true);
                    }
                    else return Json("من فضلك أرفق صورة");
                }
                else return Json("من فضلك إذا تم إنجاز البند اضغط على الصندوق");
            }
            else
            {
                if (!EngineerReview && !IsEngineerReviewHasImage)
                    return Json(true);
                if (EngineerReview && IsEngineerReviewHasImage)
                {
                    return Json(true);
                }
                else if (IsEngineerReviewHasImage) return Json("من فضلك اضفط على الصندوق أو احذف الصورة");
                else return Json("من فضلك أرفق صورة");
            }
        }
        public ActionResult isValidEngineerShadyReview(bool EngineerShadyReview, bool IsEngineerShadyReviewHasImage, bool isEngineerShadyHasExistedImage)
        {
            if (EngineerShadyReview && isEngineerShadyHasExistedImage)
                return Json(true);
            if (isEngineerShadyHasExistedImage)
            {
                if (EngineerShadyReview)
                {
                    if (IsEngineerShadyReviewHasImage)
                    {
                        return Json(true);
                    }
                    else return Json("من فضلك أرفق صورة");
                }
                else return Json("من فضلك إذا تم إنجاز البند اضغط على الصندوق");
            }
            else
            {
                if (!EngineerShadyReview && !IsEngineerShadyReviewHasImage)
                    return Json(true);
                if (EngineerShadyReview && IsEngineerShadyReviewHasImage)
                {
                    return Json(true);
                }
                else if (IsEngineerShadyReviewHasImage) return Json("من فضلك اضفط على الصندوق أو احذف الصورة");
                else return Json("من فضلك أرفق صورة");
            }
        }

        public ActionResult isValidSyndicate(bool Syndicate, bool IsSyndicateHasImage, bool isSyndicateHasExistedImage)
        {
            if (Syndicate && isSyndicateHasExistedImage)
                return Json(true);
            if (isSyndicateHasExistedImage)
            {
                if (Syndicate)
                {
                    if (IsSyndicateHasImage)
                    {
                        return Json(true);
                    }
                    else return Json("من فضلك أرفق صورة");
                }
                else return Json("من فضلك إذا تم إنجاز البند اضغط على الصندوق");
            }
            else
            {
                if (!Syndicate && !IsSyndicateHasImage)
                    return Json(true);
                if (Syndicate && IsSyndicateHasImage)
                {
                    return Json(true);
                }
                else if (IsSyndicateHasImage) return Json("من فضلك اضفط على الصندوق أو احذف الصورة");
                else return Json("من فضلك أرفق صورة");
            }
        }
        #endregion

        #region GroupStage3
        public IActionResult isValidMansourReview(bool EngineerMansourReview, bool IsEngineerMansourHasImage, bool IsMansourHasExistedImage)
        {
            if (EngineerMansourReview && IsMansourHasExistedImage)
                return Json(true);
            if (IsMansourHasExistedImage)
            {
                if (EngineerMansourReview)
                {
                    if (IsEngineerMansourHasImage)
                    {
                        return Json(true);
                    }
                    else return Json("من فضلك أرفق صورة");
                }
                else return Json("من فضلك إذا تم إنجاز البند اضغط على الصندوق");
            }
            else
            {
                if (!EngineerMansourReview && !IsEngineerMansourHasImage)
                    return Json(true);
                if (EngineerMansourReview && IsEngineerMansourHasImage)
                {
                    return Json(true);
                }
                else if (IsEngineerMansourHasImage) return Json("من فضلك اضفط على الصندوق أو احذف الصورة");
                else return Json("من فضلك أرفق صورة");
            }
        }


        public IActionResult isValidPayment(bool FinalPayOrder, bool IsFinalPagyHasImage, string? FinalPayTransactionNumber, bool IsFinalPayHasExistedImage)
        {
            if (IsFinalPagyHasImage && IsFinalPayHasExistedImage && FinalPayTransactionNumber is not null && FinalPayOrder)
                return Json(true);
            if (IsFinalPayHasExistedImage)
            {
                if (FinalPayOrder)
                {
                    if (FinalPayTransactionNumber != null)
                    {
                        var existing = _context.PermitStages
                        .FirstOrDefault(x => x.TransactionNumber == FinalPayTransactionNumber);

                        if (existing == null || existing.StageId == 12)
                        {
                            return Json(true);
                        }
                        else
                        {
                            return Json("رقم المعاملة مستخدم في سجل آخر");
                        }
                    }
                    else
                    {
                        return Json("من فضلك أرفق رقم المعاملة");
                    }
                }
                else return Json("إذا تم إنجاز البند من فضلك اضفط على الصندوق");
            }
            else
            {
                if (!FinalPayOrder && !IsFinalPagyHasImage && FinalPayTransactionNumber is null)
                    return Json(true);
                if (FinalPayOrder && IsFinalPagyHasImage && FinalPayTransactionNumber is not null)
                {
                    var existing = _context.PermitStages
                        .Any(x => x.TransactionNumber == FinalPayTransactionNumber);
                    if (existing)
                    {
                        return Json("يجب أن يكون رقم المعاملة رقم مميز");
                    }
                    return Json(true);
                }
                else if (FinalPayOrder && IsFinalPagyHasImage && FinalPayTransactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة");
                }

                else if (FinalPayOrder && !IsFinalPagyHasImage && FinalPayTransactionNumber is not null)
                {
                    return Json("من فضلك أرفق صورة من أمر الدفع النهائى");
                }

                else if (FinalPayOrder && !IsFinalPagyHasImage && FinalPayTransactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة وصورة من أمر الدفع النهائى");
                }
                else if (!FinalPayOrder && IsFinalPagyHasImage && FinalPayTransactionNumber is not null)
                {
                    return Json("إذا تم إنجاز البند وبياناته مرفقة، من فضلك اضغط على الصندوق");
                }
                else if (!FinalPayOrder && IsFinalPagyHasImage && FinalPayTransactionNumber is null)
                {
                    return Json("من فضلك أرفق رقم المعاملة أو قم بإلغاء تحديد الصورة");
                }
                else if (!FinalPayOrder && !IsFinalPagyHasImage && FinalPayTransactionNumber is not null)
                {
                    return Json("من فضلك قم بحذف رقم المعاملة أو اضغط على الصندوق وأرفق الصورة");
                }
                else
                {
                    return Json("حالة غير صحيحة، يرجى مراجعة البيانات المدخلة");
                }
            }
        }
		#endregion


		#region Expense
        public ActionResult IsValidAgreed(decimal AgreementAmount, decimal Received)
        {
            if (AgreementAmount == 0) return Json("من فضلك أدخل المبلغ المتفق عليه");
            if (Received == 0) return Json("من فضلك أدخل المبلغ الواصل");
            if (Received > AgreementAmount) return Json(false);
            return Json(true);
        }
		public ActionResult IsValidReceived(decimal Received,decimal AgreementAmount)
		{
            if (AgreementAmount == 0) return Json("من فضلك أدخل المبلغ المتفق عليه");
            if (Received == 0) return Json("من فضلك أدخل المبلغ الواصل");
            if (Received > AgreementAmount) return Json(false);
			return Json(true);
		}
		#endregion



		#region LastStage
		public ActionResult isValidPrinting(bool Printing, bool IsPrinitingHasImage, bool IsPrintingHasExistedImage)
        {
            if (Printing && IsPrintingHasExistedImage)
                return Json(true);
            if (IsPrintingHasExistedImage)
            {
                if (Printing)
                {
                    if (IsPrinitingHasImage)
                    {
                        return Json(true);
                    }
                    else return Json("من فضلك أرفق صورة");
                }
                else return Json("من فضلك إذا تم إنجاز البند اضغط على الصندوق");
            }
            else
            {
                if (!Printing && !IsPrinitingHasImage)
                    return Json(true);
                if (Printing && IsPrinitingHasImage)
                {
                    return Json(true);
                }
                else if (IsPrinitingHasImage) return Json("من فضلك اضفط على الصندوق أو احذف الصورة");
                else return Json("من فضلك أرفق صورة");
            }
        }

        public IActionResult isValidReceving(bool IsPermitReceived, bool IsPrinitingHasImage,
            bool IsPrintingHasExistedImage, bool Printing)
        {
            if (!IsPermitReceived && !IsPrinitingHasImage && !Printing)
            {
                return Json(true);
            }
            if (IsPrintingHasExistedImage && Printing && IsPermitReceived)
                return Json(true);
            if (IsPrintingHasExistedImage && Printing && !IsPermitReceived)
                return Json(true);
            if (Printing && IsPrinitingHasImage&&!IsPermitReceived)
            {
                return Json(true);
            }
            if (Printing && IsPrinitingHasImage && IsPermitReceived)
            {
                return Json(true);
            }
            else return Json("يجب الانتهاء من بند طباعة الرخصة بشكل كامل");
        }
        #endregion


        #region clientVM
        public ActionResult isValidName(string FullName)
        {
            bool isValid = FullName.All(x=>Char.IsLetter(x)||x==' ');
            return Json(isValid);
        }
        public ActionResult isValidId(string NationalId)
        {
            bool isValid = NationalId.All(x => Char.IsDigit(x));
            if(isValid)
            {
                if (NationalId.Length != 14)
                    return Json("الرقم القومى يتكون من 14 رقم فقط ");
                bool res = _context.Clients.Any(x => x.NationalId == NationalId);
                if(res)
                {
                    return Json("هذا الرقم القومى موجود بالفعل");
                }return Json(true);
            }
            return Json("الرقم القومى لابد أن يحتوى على أرقام فقط");
        }
        public IActionResult isValidPhone(string Phone,int id)
        {
            bool isValid = Phone.All(x => Char.IsDigit(x));
            if (isValid)
            {
                if (Phone.Length != 11)
                    return Json("رقم التليفون يتكون من 11 رقم فقط ");
                bool res = _context.Clients.Any(x => x.Phone == Phone);
                if (res)
                {
                    return Json("هذا الرقم موجود بالفعل");
                }
                return Json(true);
            }
            return Json("الرقم لابد أن يحتوى على أرقام فقط");
        }

		public ActionResult isValidId2(string NationalId, int id)
		{
			bool isValid = NationalId.All(x => Char.IsDigit(x));
			if (isValid)
			{
				if (NationalId.Length != 14)
					return Json("الرقم القومى يتكون من 14 رقم فقط ");
				var res = _context.Clients.FirstOrDefault(x => x.NationalId == NationalId);
				if (res == null || res.Id == id)
				{
					return Json(true);
				}
				else return Json("هذا الرقم القومى موجود بالفعل");
			}
			return Json("الرقم القومى لابد أن يحتوى على أرقام فقط");
		}
		public IActionResult isValidPhone2(string Phone, int id)
		{
			bool isValid = Phone.All(x => Char.IsDigit(x));
			if (isValid)
			{
				if (Phone.Length != 11)
					return Json("رقم التليفون يتكون من 11 رقم فقط ");
				var res = _context.Clients.FirstOrDefault(x => x.Phone == Phone);
				if (res == null || res.Id == id)
				{
					return Json(true);

				}else return Json("هذا الرقم موجود بالفعل");
			}
			return Json("الرقم لابد أن يحتوى على أرقام فقط");
		}
		#endregion

        public ActionResult isValidRemeberDate(DateTime RememberDate)
        {
            if (RememberDate<= DateTime.Now)
            {
                return Json("تاريخ التذكير يجب أن يكون فى المستقل");
            }
            return Json(true);
        }
        public IActionResult IsValidUserName(string Username)
        {
            if(_context.Users.Any(x=>x.UserName==Username))
            {
                return Json("اسم المستخدم موجود بالفعل .. من فضلك أدخل اسم مستخدم مميز");
            }
            return Json(true);
        }
        public ActionResult isValidAmountCost(decimal Amount)
        {
            if (Amount == 0)
                return Json("من فضلك أدخل القيمة");
            return Json(true);
        }
    }
}