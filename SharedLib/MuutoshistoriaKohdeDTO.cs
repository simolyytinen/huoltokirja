using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
    public class MuutoshistoriaKohdeDTO
    {
        public int IdmuutoshistoriaKohde { get; set; }

        public string Nimi { get; set; } = null!;

        public string Kuvaus { get; set; } = null!;

        public string Sijainti { get; set; } = null!;

        public string Tunnus { get; set; } = null!;

        public DateTime Muokattu { get; set; }

        public int KohdeIdkohde { get; set; }

        public int KayttajaIdkayttaja { get; set; }

        public int IdkohteenTila { get; set; }
    }
}
