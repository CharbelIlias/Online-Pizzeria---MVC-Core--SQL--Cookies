using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASPInlämningTvå_KarbelIlias.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Fyll i Användarnamn.")]
        [StringLength(20, ErrorMessage = "Användarnamnet är för långt, max 20 bokstäver.")]
        public string AnvandarNamn { get; set; }

        [Required(ErrorMessage = "Fyll i Lösenord.")]
        [StringLength(20, ErrorMessage = "Lösenordet är för långt, max 20 bokstäver.")]
        public string Losenord { get; set; }
    }
}
