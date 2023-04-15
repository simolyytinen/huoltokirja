using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
	public class VaihdaSalasana
	{
		[Required(ErrorMessage = "Valitse uusi salasana."), StringLength(20, MinimumLength = 5)]
		public string UusiSalasana { get; set; } = string.Empty;
		[Compare("UusiSalasana", ErrorMessage = "Salasanat eivät täsmää.")]
		public string VahvistaSalasana { get; set; } = string.Empty;
	}
}
