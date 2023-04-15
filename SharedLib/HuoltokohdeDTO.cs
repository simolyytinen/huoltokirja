using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
    public class HuoltokohdeDTO
    {
        public int Idkohde { get; set; }

        [Required(ErrorMessage = "Pakollinen kenttä")]
        public string Nimi { get; set; } = null!;

        [Required(ErrorMessage = "Pakollinen kenttä")]
        public string Kuvaus { get; set; } = null!;

        [Required(ErrorMessage = "Pakollinen kenttä")]
        public string Sijainti { get; set; } = null!;

        [Required(ErrorMessage = "Pakollinen kenttä")]
        public string Tyyppi { get; set; } = null!;

        [Required(ErrorMessage = "Pakollinen kenttä")]
        public string Malli { get; set; } = null!;

        [Required(ErrorMessage = "Pakollinen kenttä")]
        public string Tunnus { get; set; } = null!;

        [Required(ErrorMessage = "Pakollinen kenttä")]
        public int IdkohteenTila { get; set; }

        public DateTime Luotu { get; set; }

        public int Idkayttaja { get; set; }

        [Required(ErrorMessage = "Pakollinen kenttä")]
        public int Idkohderyhma { get; set; }

    }
}
