using System;
using System.Collections.Generic;

namespace backend.Data;

public partial class Kohderyhma
{
	public int Idkohderyhma { get; set; }

	public string Nimi { get; set; } = null!;

	public virtual ICollection<Auditointipohja> Auditointipohjas { get; } = new List<Auditointipohja>();

	public virtual ICollection<Kohde> Kohdes { get; } = new List<Kohde>();
}
