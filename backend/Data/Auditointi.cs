using System;
using System.Collections.Generic;

namespace backend.Data;

public partial class Auditointi
{
	public int Idauditointi { get; set; }

	public DateTime Luotu { get; set; }

	public string? Selite { get; set; }

	public int? Lopputulos { get; set; }

	public int Idkohde { get; set; }

	public int Idkayttaja { get; set; }

	public virtual Kayttaja IdkayttajaNavigation { get; set; } = null!;

	public virtual Kohde IdkohdeNavigation { get; set; } = null!;

	public virtual ICollection<Vaatimu> Vaatimus { get; } = new List<Vaatimu>();
}
