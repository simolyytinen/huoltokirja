using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace backend.Data;

public partial class Liite
{
	public int Idliite { get; set; }

	public string? Sijainti { get; set; }

	public int Idtarkastus { get; set; }

    [JsonIgnore]
    public virtual Tarkastu IdtarkastusNavigation { get; set; } = null!;
}
