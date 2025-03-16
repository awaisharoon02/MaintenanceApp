using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MaintenanceApp.Models
{
    public class RequestMaintenanceModel
    {
        [ValidateNever]
        public int Id { get; set; }
        public DateOnly RequestDate { get; set; }
        public string Department { get; set; }
        public string RequestedBy { get; set; }
        public string ReceivedBy { get; set; }
        public TimeOnly ReceivedTime { get; set; }
        public string Machine { get; set; }
        public string Description { get; set; }
        public string Proposal { get; set; }
        
        public string? DescriptionAction { get; set; }
        
        public TimeOnly? MachineRunning { get; set; }
        
        public string? RequestOpen { get; set; }
        [AllowNull]
        public string? ActionTakenBy { get; set; }
        [AllowNull]
        public DateOnly? ActionTakenDate { get; set; }
        [AllowNull]
        public string? ActionApprovedBy { get; set; }
        [AllowNull]
        public DateOnly? ActionApprovedDate { get; set; }
        [AllowNull]
        public string? ActionAcknowledgedBy { get; set; }
        [AllowNull]
        public DateOnly? ActionAcknowledgedDate { get; set; }
        [AllowNull]
        public List<PhraseModel> PredefindPhrases = PhraseModel.PredefinedPhrases();

        [ValidateNever]
        public List<PhraseModel> phraseModel { get; set; } = new List<PhraseModel>();

        [NotMapped]
        public string RequestStatus { get; set; }
        [NotMapped]
        public string MachineState { get; set; }
        [NotMapped]
        public List<string> RequestFor {  get; set; } = new List<string>();
        [NotMapped]
        public List<string> ReviewOfProblem { get; set; } = new List<string>();
        [NotMapped]
        public string MachineState2 { get; set; }
        [NotMapped]
        public string StatufOfRequest {  get; set; }
        public string FormStatus { get; set; }
        [AllowNull]
        public string? ReviewRemarks { get; set; }
       
    }
}