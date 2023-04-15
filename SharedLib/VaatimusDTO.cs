using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
	public class VaatimusDTO
	{
		public int Idvaatimus { get; set; }

		public string? Kuvaus { get; set; }

		public string? Pakollisuus { get; set; }

		[Required(ErrorMessage = "Merkitse vaatimus, joko 'täytetty' tai ei 'täytetty'")]
		public int? Taytetty { get; set; }

		public int Idauditointi { get; set; }

		public string? AuditointiSelite { get; set; }	
	}
}
