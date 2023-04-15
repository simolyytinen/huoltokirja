using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SharedLib;
using System;
using System.Security.Claims;

namespace backend.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class KohdeController : ControllerBase
    {

        private readonly ILogger<DbHuoltokirjaContext> _logger;
        private readonly DbHuoltokirjaContext _db;

        public KohdeController(ILogger<DbHuoltokirjaContext> logger, DbHuoltokirjaContext db)
        {
            _logger = logger;
            _db = db;
        }

		//kohteet auditoinnille kohderyhmän mukaan
		[HttpGet("/kohde/ryhma/{ryhmaid}")]
		public async Task<ActionResult<IEnumerable<HuoltokohdeDTO>>> GetAll(int? ryhmaid)
		{

			return await _db.Kohdes.Where(a => a.Idkohderyhma == ryhmaid).Select(a => Helpers.KohdeToDTO(a)).ToListAsync();

		}

		// Kaikki kohteet
		[HttpGet("/kohde/all")]
        public async Task<ActionResult<IEnumerable<HuoltokohdeDTO>>> GetAll()
        {

            return await _db.Kohdes.OrderBy(a => a.Idkohde).Select(a => Helpers.KohdeToDTO(a)).ToListAsync();

        }

        // Kaikki paitsi poistetut kohteet
        [HttpGet("/kohde/all/tila1or2")]
        public async Task<ActionResult<IEnumerable<HuoltokohdeDTO>>> GetAll1Or2()
        {

            return await _db.Kohdes.Where(a => a.IdkohteenTila < 3).OrderBy(a => a.Idkohde).Select(a => Helpers.KohdeToDTO(a)).ToListAsync();

        }

        // Luodaan uusi kohde
        [HttpPost("/kohde"), Authorize]
        public async Task<IActionResult> LisaaKohde([FromBody] HuoltokohdeDTO t)
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


            if (false == ModelState.IsValid)
            {
                return BadRequest();
            }

            t.Idkayttaja = int.Parse(id);

            Kohde newKohde = Helpers.DTOtoKohde(t);

            _db.Kohdes.Add(newKohde);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // haetaan yksi kohde
        [HttpGet("/kohde/{id}")]
        public async Task<IActionResult> GetOne(int id)
        {

            var a = await _db.Kohdes.Where(a => a.Idkohde == id).FirstOrDefaultAsync();

            if (a == null)
            {
                return NotFound();
            }

            else
            {
                return Ok(Helpers.KohdeToDTO(a));
            }
        }

        // muokataan kohteen tietoja
        [HttpPut("/kohde/{id}")]
        public async Task<IActionResult> Update(int id, HuoltokohdeDTO model)
        {
            var kohde = await _db.Kohdes.FindAsync(id);
            if (null == kohde)
            {
                return NotFound();
            }
            kohde.Idkohde = id;
            kohde.Nimi = model.Nimi;
            kohde.Kuvaus = model.Kuvaus;
            kohde.Sijainti = model.Sijainti;
            kohde.Tyyppi= model.Tyyppi;
            kohde.Malli = model.Malli;
            kohde.Tunnus = model.Tunnus;
            kohde.IdkohteenTila = model.IdkohteenTila;
            kohde.Luotu = model.Luotu;
            kohde.Idkayttaja = model.Idkayttaja;
            kohde.Idkohderyhma = model.Idkohderyhma;

            await _db.SaveChangesAsync();

            return Ok();
        }

        // kohteen poisto
        [HttpDelete("/kohde/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {

            var kohde = await _db.Kohdes.Where(i => i.Idkohde == id).FirstOrDefaultAsync();

            if (kohde != null)
            {
                _db.Kohdes.Remove(kohde);
                await _db.SaveChangesAsync();
                return NoContent();
            }
            return NotFound("kohdetta ei löydy");

        }

        // tänne sorttausta ja filtteröintiä
        [HttpGet("/kohde/sortbynimi")]
        public async Task<ActionResult<IEnumerable<HuoltokohdeDTO>>> SortByNimi()
        {

            return await _db.Kohdes.Where(a => a.IdkohteenTila < 3).OrderBy(a => a.Nimi).Select(a => Helpers.KohdeToDTO(a)).ToListAsync();

        }

        [HttpGet("/kohde/sortbysijainti")]
        public async Task<ActionResult<IEnumerable<HuoltokohdeDTO>>> SortBySijainti()
        {

            return await _db.Kohdes.Where(a => a.IdkohteenTila < 3).OrderBy(a => a.Sijainti).Select(a => Helpers.KohdeToDTO(a)).ToListAsync();

        }

        [HttpGet("/kohde/sortbytyyppi")]
        public async Task<ActionResult<IEnumerable<HuoltokohdeDTO>>> SortByTyyppi()
        {

            return await _db.Kohdes.Where(a => a.IdkohteenTila < 3).OrderBy(a => a.Tyyppi).Select(a => Helpers.KohdeToDTO(a)).ToListAsync();

        }

        [HttpGet("/kohde/sortbytila")]
        public async Task<ActionResult<IEnumerable<HuoltokohdeDTO>>> SortByTila()
        {

            return await _db.Kohdes.Where(a => a.IdkohteenTila < 3).OrderBy(a => a.IdkohteenTila).Select(a => Helpers.KohdeToDTO(a)).ToListAsync();

        }

        [HttpGet("/kohde/sortbyluotu")]
        public async Task<ActionResult<IEnumerable<HuoltokohdeDTO>>> SortByLuotu()
        {

            return await _db.Kohdes.Where(a => a.IdkohteenTila < 3).OrderBy(a => a.Luotu).Select(a => Helpers.KohdeToDTO(a)).ToListAsync();

        }

        [HttpGet("/kohde/sortbyluoja")]
        public async Task<ActionResult<IEnumerable<HuoltokohdeDTO>>> SortByLuoja()
        {

            return await _db.Kohdes.Where(a => a.IdkohteenTila < 3).OrderBy(a => a.Idkayttaja).Select(a => Helpers.KohdeToDTO(a)).ToListAsync();

        }

        [HttpGet("/kohde/sortbykohderyhma")]
        public async Task<ActionResult<IEnumerable<HuoltokohdeDTO>>> SortByKohderyhma()
        {

            return await _db.Kohdes.Where(a => a.IdkohteenTila < 3).OrderBy(a => a.Idkohderyhma).Select(a => Helpers.KohdeToDTO(a)).ToListAsync();

        }

        [HttpGet("/kohde/filterbykaytossa")]
        public async Task<ActionResult<IEnumerable<HuoltokohdeDTO>>> Filter1()
        {

            return await _db.Kohdes.Where(a => a.IdkohteenTila == 1).Select(a => Helpers.KohdeToDTO(a)).ToListAsync();
 

        }

        [HttpGet("/kohde/filterbyepakunnossa")]
        public async Task<ActionResult<IEnumerable<HuoltokohdeDTO>>> Filter2()
        {

            return await _db.Kohdes.Where(a => a.IdkohteenTila == 2).Select(a => Helpers.KohdeToDTO(a)).ToListAsync();


        }
        [HttpGet("/kohde/filterbypoistettu")]
        public async Task<ActionResult<IEnumerable<HuoltokohdeDTO>>> Filter3()
        {

            return await _db.Kohdes.Where(a => a.IdkohteenTila == 3).Select(a => Helpers.KohdeToDTO(a)).ToListAsync();


        }


    }
}

