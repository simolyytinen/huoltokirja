using System;
using System.Collections.Generic;

namespace backend.Data;

public partial class Kohde
{
	public int Idkohde { get; set; }

	public string Nimi { get; set; } = null!;

	public string Kuvaus { get; set; } = null!;

	public string Sijainti { get; set; } = null!;

	public string Tyyppi { get; set; } = null!;

	public string Malli { get; set; } = null!;

	public string Tunnus { get; set; } = null!;

	public DateTime Luotu { get; set; }

	public int Idkayttaja { get; set; }

	public int Idkohderyhma { get; set; }

	public int IdkohteenTila { get; set; }

	public virtual ICollection<Auditointi> Auditointis { get; } = new List<Auditointi>();

	public virtual Kayttaja IdkayttajaNavigation { get; set; } = null!;

	public virtual Kohderyhma IdkohderyhmaNavigation { get; set; } = null!;

	public virtual KohteenTila IdkohteenTilaNavigation { get; set; } = null!;

	public virtual ICollection<MuutoshistoriaKohde> MuutoshistoriaKohdes { get; } = new List<MuutoshistoriaKohde>();

	public virtual ICollection<Tarkastu> Tarkastus { get; } = new List<Tarkastu>();
}
