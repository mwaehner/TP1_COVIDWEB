using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TP1_ARQWEB.Areas.Identity.Data;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace TP1_ARQWEB.Models
{
    public class InfectionDischarge
    {
        public class DischargedAfterDiagnosisAttribute : ValidationAttribute
        {
            private string GetErrorMessage()
            {
                return "Alta debe ser posterior a la fecha de diagnostico.";
            }

            protected override ValidationResult IsValid(object currentValue, ValidationContext validationContext)
            {


                var currentType = validationContext.ObjectInstance.GetType();
                var value = validationContext.ObjectInstance;


                var property = currentType.GetProperty("DiagnosisDate");
                value = property.GetValue(value, null);



                var releaseYear = ((DateTime)currentValue);

                if ((DateTime)value >= releaseYear)
                {
                    return new ValidationResult(GetErrorMessage());
                }

                return ValidationResult.Success;





            }
        }







        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Fecha de alta")]
        public DateTime DischargedDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Fecha de diagnóstico")]
        public DateTime DiagnosisDate { get; set; }


        [Required]
        public int InfectionReportId { get; set; }

    }
}
