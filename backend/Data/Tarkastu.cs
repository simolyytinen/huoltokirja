using System;
using System.Collections.Generic;

namespace backend.Data;

public partial class Tarkastu
{
	public int Idtarkastus { get; set; }

	public DateTime Aikaleima { get; set; }

	public string Syy { get; set; } = null!;

	public string Havainnot { get; set; } = null!;

	public int TilanMuutos { get; set; }

	public int Idkayttaja { get; set; }

	public int Idkohde { get; set; }

	public virtual Kayttaja IdkayttajaNavigation { get; set; } = null!;

	public virtual Kohde IdkohdeNavigation { get; set; } = null!;

	public virtual ICollection<Liite> Liites { get; } = new List<Liite>();
}
