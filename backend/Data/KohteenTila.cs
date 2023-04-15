using System;
using System.Collections.Generic;

namespace backend.Data;

public partial class KohteenTila
{
	public int IdkohteenTila { get; set; }

	public string Kuvaus { get; set; } = null!;

	public virtual ICollection<Kohde> Kohdes { get; } = new List<Kohde>();

	public virtual ICollection<MuutoshistoriaKohde> MuutoshistoriaKohdes { get; } = new List<MuutoshistoriaKohde>();
}
