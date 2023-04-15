using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Data;

public partial class Auditointipohja
{
	public int Idauditointipohja { get; set; }

	public string? Selite { get; set; }

	public DateTime Luontiaika { get; set; }

	public int Idkayttaja { get; set; }

	public int? Idkohderyhma { get; set; }

	public virtual Kayttaja IdkayttajaNavigation { get; set; } = null!;

	public virtual Kohderyhma? IdkohderyhmaNavigation { get; set; }

	public virtual ICollection<Vaatimuspohja> Vaatimuspohjas { get; } = new List<Vaatimuspohja>();
}
