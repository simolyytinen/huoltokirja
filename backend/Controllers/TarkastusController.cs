using System.Security.Claims;
using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace backend.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class TarkastusController : ControllerBase
    {
        private readonly ILogger<DbHuoltokirjaContext> _logger;
        private readonly DbHuoltokirjaContext _db;

        public TarkastusController(ILogger<DbHuoltokirjaContext> logger, DbHuoltokirjaContext db)
        {
            _logger = logger;
            _db = db;

        }


        // Kaikki tarkastukset
        [HttpGet("/tarkastus/kaikki")]
        [HttpGet("/tarkastus/kaikki/{sortOrder?}")]
        public async Task<ActionResult<IEnumerable<TarkastusDTO>>> HaeKaikki(string? sortOrder)
        {

            if (sortOrder == null)
            {
                return await _db.Tarkastus
                .Include(t => t.IdkayttajaNavigation)
                .Include(t => t.IdkohdeNavigation)
                .Include(t => t.Liites)
                .Select(t => Helpers.TarkastusToDTO(t)).ToListAsync();
            }
            else if (sortOrder.Equals("asc"))
            {
                return await _db.Tarkastus
                .Include(t => t.IdkayttajaNavigation)
                .Include(t => t.IdkohdeNavigation)
                .Include(t => t.Liites)
                .OrderBy(t => t.Aikaleima)
                .Select(t => Helpers.TarkastusToDTO(t)).ToListAsync();
            }
            else if (sortOrder.Equals("desc"))
            {
                return await _db.Tarkastus
                .Include(t => t.IdkayttajaNavigation)
                .Include(t => t.IdkohdeNavigation)
                .Include(t => t.Liites)
                .OrderByDescending(t => t.Aikaleima)
                .Select(t => Helpers.TarkastusToDTO(t)).ToListAsync();
            }
            else return BadRequest();

        }

        // Kaikki kohteen tarkastukset
        [HttpGet("/tarkastus/kohde/{id}")]
        [HttpGet("/tarkastus/kohde/{id}/{sortOrder?}")]
        public async Task<ActionResult<IEnumerable<TarkastusDTO>>> HaeKaikkiKohteenTarkastukset(int id, string? sortOrder)
        {
            if (sortOrder == null)
            {
                return await _db.Tarkastus
                .Include(t => t.IdkayttajaNavigation)
                .Include(t => t.IdkohdeNavigation)
                .Include(t => t.Liites)
                .Where(t => t.Idkohde == id)
                .Select(t => Helpers.TarkastusToDTO(t))
                .ToListAsync();
            }
            else if (sortOrder.Equals("asc"))
            {
                return await _db.Tarkastus
                .Include(t => t.IdkayttajaNavigation)
                .Include(t => t.IdkohdeNavigation)
                .Include(t => t.Liites)
                .Where(t => t.Idkohde == id)
                .OrderBy(t => t.Aikaleima)
                .Select(t => Helpers.TarkastusToDTO(t))
                .ToListAsync();
            }
            else if (sortOrder.Equals("desc"))
            {
                return await _db.Tarkastus
                .Include(t => t.IdkayttajaNavigation)
                .Include(t => t.IdkohdeNavigation)
                .Include(t => t.Liites)
                .Where(t => t.Idkohde == id)
                .OrderByDescending(t => t.Aikaleima)
                .Select(t => Helpers.TarkastusToDTO(t))
                .ToListAsync();
            }
            else return BadRequest();
        }

        // Haetaan yksittäinen tarkastus

        [HttpGet("/tarkastus/{id}")]
        public async Task<IActionResult> HaeYksiTarkastus(int id)
        {

            var t = await _db.Tarkastus
                .Include(t => t.IdkayttajaNavigation)
                .Include(t => t.IdkohdeNavigation)
                .Include(t => t.Liites)
                .Where(t => t.Idtarkastus == id).FirstOrDefaultAsync();

            if (t == null)
            {
                return NotFound();
            }
            else return Ok(Helpers.TarkastusToDTO(t));
        }
        
        // Luodaan uusi tarkastus
        [HttpPost("/tarkastus"), Authorize]
        public async Task<IActionResult> LisaaTarkastus([FromBody] TarkastusDTO t)
        {

            // Tarkistetaan käyttäjä
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
            {
                return BadRequest("Käyttäjää ei löydy");
            }
            var kayttaja = await _db.Kayttajas.Where(i => i.Idkayttaja == int.Parse(id)).FirstOrDefaultAsync();

            if (kayttaja == null)
            {
                return BadRequest("Käyttäjää ei löydy");
            }

            // Luodaan uusi tarkastus
            if (false == ModelState.IsValid)
            {
                return BadRequest();
            }

            t.Idkayttaja = int.Parse(id);

            Tarkastu newTarkastus = Helpers.DTOtoTarkastu(t);

            var response = _db.Tarkastus.Add(newTarkastus);
            await _db.SaveChangesAsync();

            // Muokataan kohteen tila
            var tilanMuutos = t.TilanMuutos;

            

            if (tilanMuutos != 0)
            {
                var kohde = await _db.Kohdes.FindAsync(t.Idkohde);

                if (kohde.IdkohteenTila != tilanMuutos)
                {
                    kohde.IdkohteenTila = tilanMuutos;
                    _db.Kohdes.Update(kohde);
                    await _db.SaveChangesAsync();

                    // Tallennetaan myös muutoshistoria

                    MuutoshistoriaKohde muutos = new MuutoshistoriaKohde
                    {
                        Nimi = kohde.Nimi,
                        Kuvaus = kohde.Kuvaus,
                        Sijainti = kohde.Sijainti,
                        Tunnus = kohde.Tunnus,
                        Muokattu = newTarkastus.Aikaleima,
                        KohdeIdkohde = kohde.Idkohde,
                        KayttajaIdkayttaja = newTarkastus.Idkayttaja,
                        IdkohteenTila = kohde.IdkohteenTila
                    };

                    _db.MuutoshistoriaKohdes.Add(muutos);
                    await _db.SaveChangesAsync();
                }
            }

            

            // Päivitetään liitteet oikeaan tauluun
            if (t.Liitteet != null)
            {
                foreach (var item in t.Liitteet)
                {
                    Liite uusiLiite = new Liite
                    {
                        Sijainti = item.Location,
                        Idtarkastus = response.Entity.Idtarkastus
                    };

                    _db.Liites.Add(uusiLiite);
                }

                await _db.SaveChangesAsync();
            }

            return Ok();
        }

        // Poistetaan tarkastus
        [HttpDelete("/tarkastus/{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> PoistaTarkastus(int id)
        {

            var t = await _db.Tarkastus
                .Include(t => t.IdkayttajaNavigation)
                .Include(t => t.IdkohdeNavigation)
                .Include(t => t.Liites)
                .Where(t => t.Idtarkastus == id).FirstOrDefaultAsync();

            if (t == null)
            {
                return NotFound();
            }

            if (t.Liites is not null)
            {
                foreach (var liite in t.Liites)
                {
                    Liite l = await _db.Liites.FindAsync(liite.Idliite);
                    if (l is not null) _db.Liites.Remove(l);
                }
            }

            _db.Tarkastus.Remove(t);
            await _db.SaveChangesAsync();
            return Ok("Poistettu");
        }

        // Muokataan tarkastusta
        [HttpPut("/tarkastus/muokkaa/{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> EditTarkastus(TarkastusDTO item)
        {
            var k = await _db.Tarkastus
                .Include(t => t.Liites)
                .Where(t => t.Idtarkastus == item.Idtarkastus).FirstOrDefaultAsync();

            if (k is null)
            {
                return NotFound();
            }

            // Ainoastaan havaintoja ja syytä voi muokata
            k.Havainnot = item.Havainnot;
            k.Syy = item.Syy;


            _db.Tarkastus.Update(k);
            await _db.SaveChangesAsync();

            // päivitetään liitteet

            if (item.Liitteet is not null)
            {
                bool loytyyko = false;

                foreach (var liite in item.Liitteet)
                {
                    loytyyko = false;

                    var tallennettu = k.Liites.Where(l => l.Sijainti == liite.Location).SingleOrDefault();

                    if (tallennettu is not null)
                    {
                        loytyyko = true;
                    }

                    if (!loytyyko)
                    {
                        Liite l = new Liite { Sijainti = liite.Location, Idtarkastus = item.Idtarkastus };
                        _db.Liites.Add(l);
                    }

                }
                await _db.SaveChangesAsync();
            }

            return Ok(k);
        }

    }
}
