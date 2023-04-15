using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedLib;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class AuditointiController : ControllerBase
	{
		private readonly ILogger<DbHuoltokirjaContext> _logger;
		private readonly DbHuoltokirjaContext _db;
		private readonly IConfiguration _conf;

		public AuditointiController(ILogger<DbHuoltokirjaContext> logger, DbHuoltokirjaContext db, IConfiguration conf)
		{
			_logger = logger;
			_db = db;
			_conf = conf;
		}


		[HttpGet("/auditointi/all")]
		public async Task<IEnumerable<AuditointiDTO>> Get()
		{
			return await _db.Auditointis
				.Include(a => a.IdkohdeNavigation)
				.Include(a => a.IdkayttajaNavigation)
				.Include(a => a.Vaatimus)
				.Select(a => Helpers.AuditointiToDTO(a)).ToListAsync();
		}

		[HttpGet("/auditointi/{id}")]
		public async Task<ActionResult<AuditointiDTO>> HaeAuditointi(int id)
		{
			var a = await _db.Auditointis
				.Include(a => a.IdkohdeNavigation)
				.Include(a => a.IdkayttajaNavigation)
				.Include(a => a.Vaatimus)
				.Where(a => a.Idauditointi == id)
				.Select(a => Helpers.AuditointiToDTO(a)).SingleOrDefaultAsync();

			if (a is null)
			{
				return NotFound();
			}

			else return a;

		}

		[HttpPost("/auditointi/add"), Authorize]
		public async Task<ActionResult<Auditointi>> AddAuditointi(AuditointiDTO req)
		{
			int kayttaja = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

			Auditointi a = new()
			{
				Luotu = DateTime.Now,
				Selite = req.Selite,
				Lopputulos = req.Lopputulos,
				Idkohde = req.Idkohde,
				Idkayttaja = kayttaja,

			};

			var result = _db.Auditointis.Add(a);
			await _db.SaveChangesAsync();

			//tarkistetaan/muutetaan auditoinnin kohteen tila
			var kohde = await _db.Kohdes.Where(i => i.Idkohde == a.Idkohde).FirstOrDefaultAsync();
			int tila;

			if (a.Lopputulos == 0)
			{
				tila = 2;
			}
			else
			{
				tila = 1;
			}

			if (kohde.IdkohteenTila != tila)
			{
				kohde.IdkohteenTila = tila;

				_db.Kohdes.Update(kohde);
				await _db.SaveChangesAsync();

				//lisätään tilamuutos kohteen muutoshistoriaan
				MuutoshistoriaKohde mk = new()
				{
					Nimi = kohde.Nimi,
					Kuvaus = kohde.Kuvaus,
					Sijainti = kohde.Sijainti,
					Tunnus = kohde.Tunnus,
					Muokattu = DateTime.Now,
					KohdeIdkohde = kohde.Idkohde,
					KayttajaIdkayttaja = kayttaja,
					IdkohteenTila = tila
				};
				_db.MuutoshistoriaKohdes.Add(mk);
				await _db.SaveChangesAsync();

			}




			//Tallennetaan vaatimukset vaatimus-tauluun
			foreach (var item in req.Vaatimukset)
			{
				Vaatimu v = new()
				{
					Kuvaus = item.Kuvaus,
					Pakollisuus = item.Pakollisuus,
					Taytetty = item.Taytetty,
					Idauditointi = result.Entity.Idauditointi
				};
				_db.Vaatimus.Add(v);
			}
			await _db.SaveChangesAsync();

			return Ok();
		}

		[HttpPut("/auditointi/edit"), Authorize(Roles = "admin")]
		public async Task<IActionResult> MuokkaaAuditointi([FromBody] AuditointiDTO req)
		{
			var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			var kayttaja = await _db.Kayttajas.FindAsync(id);

			if (kayttaja is null)
			{
				return BadRequest("Käyttäjää ei löydy");
			}

			var a = await _db.Auditointis
				.Include(a => a.Vaatimus)
				.Where(a => a.Idauditointi == req.Idauditointi).FirstOrDefaultAsync();

			if (a is null) return NotFound();

			// Muokataan vain selite kenttää
			a.Selite = req.Selite;

			_db.Auditointis.Update(a);
			await _db.SaveChangesAsync();

			return Ok("Muokattu");

		}

		[HttpDelete("/auditointi/{id}"), Authorize(Roles = "admin")]
		public async Task<IActionResult> PoistaAuditointi(int id)
		{
            var a = await _db.Auditointis
                .Include(a => a.Vaatimus)
                .Where(a => a.Idauditointi == id).FirstOrDefaultAsync();

            if (a is null)
            {
                return NotFound();
            }

			// Poistetaan ensin vaatimukset

			if (a.Vaatimus is not null)
			{
				foreach (var vaatimus in a.Vaatimus)
				{
					_db.Vaatimus.Remove(vaatimus);
				}
				await _db.SaveChangesAsync();
			}

			// auditointi
			_db.Auditointis.Remove(a);
			await _db.SaveChangesAsync();

			return Ok("Poistettu");
        }
	}
}
