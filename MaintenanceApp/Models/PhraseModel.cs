using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MaintenanceApp.Models
{
    public class PhraseModel
    {
        [ValidateNever]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Phrase { get; set; }
        public string Category { get; set; }
        [AllowNull]
        public bool IsChecked { get; set; }
        [ValidateNever]
        public int RequestMaintenanceID { get; set; }
        [ForeignKey("RequestMaintenanceID")]
        public virtual RequestMaintenanceModel RequestMaintenance { get; set; }


        public static List<PhraseModel> PredefinedPhrases()
        {
            var phrases = new List<PhraseModel>
            {
                new PhraseModel{ Category="Request Status", Phrase="Normal", IsChecked=false},
                new PhraseModel{ Category="Request Status", Phrase="Urgent", IsChecked=false},
                new PhraseModel{ Category="Machine", Phrase="Running", IsChecked=false},
                new PhraseModel{Id=4, Category="Machine", Phrase="Idle", IsChecked=false},
                new PhraseModel{Id=5, Category="Request For", Phrase="Mechanical", IsChecked=false},
                new PhraseModel{Id=6, Category="Request For", Phrase="Electrical", IsChecked=false},
                new PhraseModel{Id=7, Category="Request For", Phrase="Electronic", IsChecked=false},
                new PhraseModel{Id=8, Category="Review of Problems", Phrase="Repair at the spot", IsChecked=false},
                new PhraseModel{Id=9, Category="Review of Problems", Phrase="Part Replacement", IsChecked=false},
                new PhraseModel{Id=10, Category="Review of Problems", Phrase="By-pass", IsChecked=false},
                new PhraseModel{Id=11, Category="Review of Problems", Phrase="Adjustment", IsChecked=false},
                new PhraseModel{Id=12, Category="Review of Problems", Phrase="Part to be purchased", IsChecked=false},
                new PhraseModel{Id=13, Category="Review of Problems", Phrase="Problem forwarded to Maintenance Contractor", IsChecked=false},
                new PhraseModel{Id=14, Category="Review of Problems",  Phrase="Part to be fabricated from workshop", IsChecked=false},
                new PhraseModel{Id=15, Category="Review of Problems",  Phrase="Part repair / fabricated from outside", IsChecked=false},
                new PhraseModel{Id=16, Category="Review of Problems",  Phrase="Modifying / New Job", IsChecked=false},
                new PhraseModel{Id=17, Category="Machine Status 2", Phrase="Idle", IsChecked=false},
                new PhraseModel{Id=18, Category="Machine Status 2", Phrase="Running", IsChecked=false},
                new PhraseModel{Id=19, Category="Status of Request", Phrase="Open", IsChecked=false},
                new PhraseModel{Id=20, Category="Status of Request", Phrase="Closed", IsChecked=false}

            };
            return phrases;
        }
    }
}
