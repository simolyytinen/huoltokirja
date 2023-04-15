using System;
using System.Collections.Generic;

namespace backend.Data;

public partial class Kayttaja
{
	public int Idkayttaja { get; set; }

	public string Nimi { get; set; } = null!;

	public string Kayttajatunnus { get; set; } = null!;

	public string Salasana { get; set; } = null!;

	public DateTime Luotu { get; set; }

	public DateTime? ViimeisinKirjautuminen { get; set; }

	public string Rooli { get; set; } = null!;

	public int Poistettu { get; set; }

	public virtual ICollection<Auditointipohja> Auditointipohjas { get; } = new List<Auditointipohja>();

	public virtual ICollection<Auditointi> Auditointis { get; } = new List<Auditointi>();

	public virtual ICollection<Kohde> Kohdes { get; } = new List<Kohde>();

	public virtual ICollection<MuutoshistoriaKohde> MuutoshistoriaKohdes { get; } = new List<MuutoshistoriaKohde>();

	public virtual ICollection<Tarkastu> Tarkastus { get; } = new List<Tarkastu>();
}
