using System;
using System.ComponentModel.DataAnnotations;

namespace SharedLib
{
	public class TarkastusDTO
	{
        public int Idtarkastus { get; set; }

        public DateTime Aikaleima { get; set; }

        [Required(ErrorMessage = "Pakollinen kenttä")]
        public string Syy { get; set; } = null!;

        [Required(ErrorMessage = "Pakollinen kenttä")]
        public string Havainnot { get; set; } = null!;

        [Required(ErrorMessage = "Pakollinen kenttä")]
        [Range(0,3)]
        public int TilanMuutos { get; set; }

        [Required]
        public int Idkayttaja { get; set; }
        public string? KayttajanNimi { get; set; }

        [Required(ErrorMessage = "Pakollinen kenttä")]
        public int Idkohde { get; set; }
        public string? KohteenNimi { get; set; }

        public List<UploadResult>? Liitteet { get; set; }
    }
}

