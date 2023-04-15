using System;
using System.Collections.Generic;

namespace backend.Data;

public partial class MuutoshistoriaKohde
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

	public virtual KohteenTila IdkohteenTilaNavigation { get; set; } = null!;

	public virtual Kayttaja KayttajaIdkayttajaNavigation { get; set; } = null!;

	public virtual Kohde KohdeIdkohdeNavigation { get; set; } = null!;
}
