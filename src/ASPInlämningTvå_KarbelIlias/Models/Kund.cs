using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASPInlämningTvå_KarbelIlias.Models
{
    public partial class Kund
    {
        public Kund()
        {
            Bestallning = new HashSet<Bestallning>();
        }
        public int KundId { get; set; }

        [Required]
        [StringLength(100)]
        public string Namn { get; set; }

        [Required]
        [StringLength(50)]
        public string Gatuadress { get; set; }

        [Required]
        [StringLength(20)]
        public string Postnr { get; set; }

        [Required]
        [StringLength(100)]
        public string Postort { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Fyll i Användarnamn.")]
        [StringLength(20, ErrorMessage = "Användarnamnet är för långt, max 20 bokstäver.")]
        public string AnvandarNamn { get; set; }

        [Required(ErrorMessage = "Fyll i Lösenord.")]
        [StringLength(20, ErrorMessage = "Lösenordet är för långt, max 20 bokstäver.")]
        public string Losenord { get; set; }

        public virtual ICollection<Bestallning> Bestallning { get; set; }
    }
}
