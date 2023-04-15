using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SharedLib;
using Microsoft.AspNetCore.Authorization;
using Azure;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuditointipohjaController : ControllerBase
    {
        private readonly ILogger<DbHuoltokirjaContext> _logger;
        private readonly DbHuoltokirjaContext _db;
        private readonly IConfiguration _conf;


        public AuditointipohjaController(ILogger<DbHuoltokirjaContext> logger, DbHuoltokirjaContext db, IConfiguration conf)
        {
            _logger = logger;
            _db = db;
            _conf = conf;
		}

		[HttpGet("/auditointipohja/all/{sort}")]
        public async Task<IEnumerable<AuditointipohjaDTO>> Get(string? sort)
        {
			if (sort == "desc")
			{
				return await _db.Auditointipohjas.OrderByDescending(a => a.Luontiaika)
								.Include(k => k.IdkayttajaNavigation)
								.Include(k => k.IdkohderyhmaNavigation)
								.Include(k => k.Vaatimuspohjas)
								.Select(k => Helpers.AuditointipohjaToDTO(k)).ToListAsync();
			}
			else if (sort == "asc")
			{
				return await _db.Auditointipohjas.OrderBy(a => a.Luontiaika)
								.Include(k => k.IdkayttajaNavigation)
								.Include(k => k.IdkohderyhmaNavigation)
								.Include(k => k.Vaatimuspohjas)
								.Select(k => Helpers.AuditointipohjaToDTO(k)).ToListAsync();
			}
			else if (sort == "nimi")
			{
				return await _db.Auditointipohjas.OrderBy(a => a.IdkayttajaNavigation.Nimi)
								.Include(k => k.IdkayttajaNavigation)
								.Include(k => k.IdkohderyhmaNavigation)
								.Include(k => k.Vaatimuspohjas)
								.Select(k => Helpers.AuditointipohjaToDTO(k)).ToListAsync();
			}
			else if (sort == "nimidesc")
			{
				return await _db.Auditointipohjas.OrderByDescending(a => a.IdkayttajaNavigation.Nimi)
								.Include(k => k.IdkayttajaNavigation)
								.Include(k => k.IdkohderyhmaNavigation)
								.Include(k => k.Vaatimuspohjas)
								.Select(k => Helpers.AuditointipohjaToDTO(k)).ToListAsync();
			}
            else

            {
                return await _db.Auditointipohjas
                                .Include(k => k.IdkayttajaNavigation)
                                .Include(k => k.IdkohderyhmaNavigation)
                                .Include(k => k.Vaatimuspohjas)
                                .Select(k => Helpers.AuditointipohjaToDTO(k)).ToListAsync();
            }


        }

		[HttpGet("/auditointipohja/{id}")]
		public async Task<ActionResult<AuditointipohjaDTO>> GetSinge(int? id)
		{
            if (id == null) return BadRequest("ei id:tä");

			var auditointipohja = await _db.Auditointipohjas.Where(i=> i.Idauditointipohja == id)
				.Include(k => k.IdkayttajaNavigation)
				.Include(k => k.IdkohderyhmaNavigation)
                .Include(k => k.Vaatimuspohjas)
                .Select(k => Helpers.AuditointipohjaToDTO(k))
				.FirstOrDefaultAsync();

			if (auditointipohja is null) return NotFound();

            return Ok(auditointipohja);

		}


		[HttpPost("/auditointipohja/add"), Authorize]
        public async Task<ActionResult<AuditointipohjaDTO>> Add(AuditointipohjaDTO req)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (req == null || id == null)
            {
                return BadRequest("tiedot tyhjät");
            }
            else
            {
                Auditointipohja a = new()
                {
                    Selite = req.Selite,
                    Luontiaika = DateTime.Now,
                    Idkayttaja = int.Parse(id)
                };

				if (req.Idkohderyhma != 0) a.Idkohderyhma = req.Idkohderyhma;

                var response = _db.Auditointipohjas.Add(a);
                await _db.SaveChangesAsync();

				// Lisätään auditointipohjaan liittyvät vaatimukset
				if (req.Vaatimuspohjat is not null)
				{
					foreach (var vaatimus in req.Vaatimuspohjat)
					{
						Vaatimuspohja v = new Vaatimuspohja
						{
							Kuvaus = vaatimus.Kuvaus,
							Pakollisuus = vaatimus.Pakollisuus,
							Idauditointipohja = response.Entity.Idauditointipohja
						};

						_db.Vaatimuspohjas.Add(v);
					}
					await _db.SaveChangesAsync();
				}
                
                return Ok(a);   

            }
        }

		[HttpPut("/auditointipohja/edit"), Authorize]
		public async Task<IActionResult> EditAuditointipohja(AuditointipohjaDTO req)
		{
			var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var v = await _db.Auditointipohjas
				.Include(i => i.Vaatimuspohjas)
				.Where(i => i.Idauditointipohja == req.Idauditointipohja).FirstOrDefaultAsync();

			if (req == null || v == null)
			{
				return BadRequest("ei tietoja");
			}

            v.Idauditointipohja = v.Idauditointipohja;
			v.Selite = req.Selite;
            v.Luontiaika = DateTime.Now;
			v.Idkayttaja = int.Parse(id);
            v.Idkohderyhma = req.Idkohderyhma;

			_db.Auditointipohjas.Update(v);
			await _db.SaveChangesAsync();

			// päivitetään myös pohjaan liittyvät vaatimukset
			if (req.Vaatimuspohjat is not null)
			{
				// poistetaan ne vaatimukset joita ei enää kutsun mukana tulleessa oliossa ollut
				if (v.Vaatimuspohjas is not null && v.Vaatimuspohjas.Count != 0)
				{
					foreach (var item in v.Vaatimuspohjas)
					{

						if (item.Idvaatimuspohja != 0 && req.Vaatimuspohjat.Where(x => x.Idvaatimuspohja == item.Idvaatimuspohja).FirstOrDefault() == null)
						{
							var response = _db.Vaatimuspohjas.Remove(item);
						}
					}
				}

                // lisätään uudet eli ne vaatimukset joilla ei ole vielä id:tä
                foreach (var item in req.Vaatimuspohjat)
				{
					if (item.Idvaatimuspohja == 0)
					{
						Vaatimuspohja vaatimus = new Vaatimuspohja
						{
							Kuvaus = item.Kuvaus,
							Pakollisuus = item.Pakollisuus,
							Idauditointipohja = v.Idauditointipohja
						};

						var response = _db.Vaatimuspohjas.Add(vaatimus);
					}
				}

				
				await _db.SaveChangesAsync();
			}
            

             return Ok(v);

		}

		[HttpDelete("/auditointipohja/{id}"), Authorize(Roles = "admin")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return BadRequest("id:tä ei annettu");
			}

			var a = await _db.Auditointipohjas
				.Include(i => i.Vaatimuspohjas)
				.Where(i => i.Idauditointipohja == id).FirstOrDefaultAsync();

			if (a == null)
			{
				return NotFound("tietoja ei löydy");
			}

			if (a.Vaatimuspohjas is not null)
			{
				foreach (var item in a.Vaatimuspohjas)
				{
					_db.Vaatimuspohjas.Remove(item);
				}
				await _db.SaveChangesAsync();
			}

			_db.Auditointipohjas.Remove(a);
			await _db.SaveChangesAsync();
            return NoContent(); 
		}


	}
}
