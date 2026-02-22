using Building_Permits.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Building_Permits.MVC.ViewModels
{
    public class UpdateStageGroup1View
    {
        public List<PermitStage>? permitStages = new List<PermitStage>();
        public int PermitId { get; set; }
        [Display(Name = "استلام العقد والتوكيل")]
       // [Remote(action: "isValidContract2", controller: "Remote",
        //AdditionalFields = "IsContractHasImage,ContractReceiveAndAutharizationTranactionNumber,IsContractHasExistedImage",
        //ErrorMessage = "من فضلك أرفق رقم المعاملة وصورة العقد والتوكيل")]
        public bool ContractReceiveAndAutharization { get; set; }
        [Display(Name = "ملاحظات")]
        public string? ContractReceiveAndAutharizationNotes { get; set; }
        [Display(Name = "ارفاق ملف")]
        public IFormFile? ContractReceiveAndAutharizationImage { get; set; }
        public bool IsContractHasImage { get; set; }
        [Display(Name = "رقم المعاملة")]
   //     [Remote(action:"isValidTN",controller:"Remote",AdditionalFields = "IsContractHasExistedImage,IsContractHasImage", ErrorMessage ="رقم المعاملة يجب أن يكون مميز")]
        public string? ContractReceiveAndAutharizationTranactionNumber { get; set; }
        public bool IsContractHasChanges { get; set; }
        public bool IsContractHasExistedImage { get; set; }


        [Display(Name = "بيان الصلاحية")]
        //[Remote(action: "isValidValidity2",controller:"Remote",AdditionalFields = "EngineerReceive,TechReceive,ValidityReceive,IsValidityHasImage,ValidityTranactionNumber,IsValidityHasExistedImage",
        //ErrorMessage ="بيان الصلاحية يرتبط بموعد اسنلام المهندس والفنى واستلام الصلاحية ")]
        public bool Validity { get; set; } = false;
        [Display(Name = "ملاحظات")]
        public string? ValidityNotes { get; set; }
        [Display(Name = "موعد استلام المهندس")]
        public bool EngineerReceive {  get; set; }
        [Display(Name = "موعد استلام الفنى")]
        public bool TechReceive {  get; set; }
        [Display(Name = "موعد استلام الصلاحية")]
        public bool ValidityReceive {  get; set; }
        [Display(Name = "ارفاق ملف")]
        public IFormFile? ValidityImage {  get; set; }
        public bool IsValidityHasImage {  get; set; }
        [Display(Name = "رقم المعاملة")]
        public string? ValidityTranactionNumber {  get; set; }
        public bool IsValidityHasChanges {  get; set; }
        public bool IsValidityHasExistedImage { get; set; }


        [Display(Name = "الشئون القانونية")]
       // [Remote(action: "isValidLegalAffairs2", controller: "Remote", AdditionalFields = "IsLegalAffairsHasImage,LegalAffairsTranactionNumber,IsLegalHasExistedImage", ErrorMessage = "من فضلك أرفق رقم المعاملة وصورة من إيصال الشئون القانونية")]
        public bool LegalAffairs { get; set; } = false;
        [Display(Name = "ملاحظات")]
        public string? LegalAffairsNotes { get; set; }
        [Display(Name = "ارفاق ملف")]
        public IFormFile? LegalAffairsImage { get; set; }
        public bool IsLegalAffairsHasImage { get; set; }
        //[Remote(action: "isValidTN3", controller: "Remote", AdditionalFields = "IsLegalHasExistedImage,IsLegalAffairsHasImage", ErrorMessage = "رقم المعاملة يجب أن يكون مميز")]
        [Display(Name = "رقم المعاملة")]
        public string? LegalAffairsTranactionNumber { get; set; }
        public bool IsLegalHasChanges { get; set; }
        public bool IsLegalHasExistedImage { get; set; }


        [Display(Name = "الرفع المساحى")]
    //    [Remote(action: "isValidSurveying2", controller: "Remote", AdditionalFields = "IsSurveyingHasImage,SurveyingTranactionNumber,IsSurveyingHasExistedImage", ErrorMessage = "من فضلك أرفق رقم المعاملة وصورة من بيان الرفع المساحى الرقمى")]
        public bool Surveying { get; set; } = false;
        [Display(Name = "ملاحظات")]
        public string? SurveyingNotes { get; set; }
        [Display(Name = "ارفاق ملف")]
        public IFormFile? SurveyingImage { get; set; }
        public bool IsSurveyingHasImage { get; set; }
        [Display(Name = "رقم المعاملة")]
        //[Remote(action: "isValidTN4", controller: "Remote", AdditionalFields = "IsSurveyingHasExistedImage", ErrorMessage = "رقم المعاملة يجب أن يكون مميز")]
        public string? SurveyingTranactionNumber { get; set; }
        public bool IsSurveyingHasChanges { get; set; }
        public bool IsSurveyingHasExistedImage { get; set; }


        [Display(Name = "تقفيل الملف")]
        //[Remote(action: "isvalidCompleteingFile2", controller: "Remote", AdditionalFields = "IsCompletingFileHasImage,CompletingFileTranactionNumber,IsCompletingHasExistedImage", ErrorMessage = "من فضلك أرفق رقم المعاملة وصورة من تثقيل الملف")]
        public bool CompletingFile { get; set; } = false;
        [Display(Name = "ملاحظات")]
        public string? CompletingFileNotes { get; set; }
        [Display(Name = "ارفاق ملف")]
        public IFormFile? CompletingFileImage { get; set; }
        public bool IsCompletingFileHasImage { get; set; }
        [Display(Name = "رقم المعاملة")]
        //[Remote(action: "isValidTN5", controller: "Remote",AdditionalFields = "IsCompletingHasExistedImage", ErrorMessage = "رقم المعاملة يجب أن يكون مميز")]
        public string? CompletingFileTranactionNumber { get; set; }
        public bool IsCompletingHasChanges { get; set; }
        public bool IsCompletingHasExistedImage { get; set; }


        
        [Display(Name = "تقديم الملف")]
      //  [Remote(action: "isValidFinishingFile2", controller: "Remote", AdditionalFields = "IsFinishingFileHasImage,FinishingFileTranactionNumber,IsFinishingHasExistedImage", ErrorMessage = "من فضلك أرفق رقم المعاملة وصورة من إيصال تقديم الملف")]
        public bool FinishingFile { get; set; } = false;
        [Display(Name = "ملاحظات")]
        public string? FinishingFileNotes { get; set; }
        [Display(Name = "ارفاق ملف")]
        public IFormFile? FinishingFileImage { get; set; }
        public bool IsFinishingFileHasImage { get; set; }
        [Display(Name = "رقم المعاملة")]
        //[Remote(action: "isValidTN6", controller: "Remote",AdditionalFields = "IsFinishingHasExistedImage", ErrorMessage = "رقم المعاملة يجب أن يكون مميز")]
        public string? FinishingFileTranactionNumber { get; set; }
        public bool IsFinishingHasChanges { get; set; }
        public bool IsFinishingHasExistedImage { get; set; }
    }
}