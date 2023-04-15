using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SharedLib
{
	public class VaatimuspohjaDTO
	{
		public int Idvaatimuspohja { get; set; }

		[Required]
		public string? Kuvaus { get; set; }

		[Required]
		public string? Pakollisuus { get; set; }

		public int Idauditointipohja { get; set; }

		public string? AuditointipohjaSelite { get; set; }

	}
}
