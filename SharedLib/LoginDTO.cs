using System;
using System.ComponentModel.DataAnnotations;

namespace SharedLib
{
	public class LoginDTO
	{
		[Required]
        public string Kayttajatunnus { get; set; }
		[Required]
		public string Salasana { get; set; }
	}
}

