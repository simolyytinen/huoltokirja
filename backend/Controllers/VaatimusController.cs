using backend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLib;
using System.Security.Claims;

namespace backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VaatimusController : ControllerBase
	{
		private readonly ILogger<DbHuoltokirjaContext> _logger;
		private readonly DbHuoltokirjaContext _db;
		private readonly IConfiguration _conf;

		public VaatimusController(ILogger<DbHuoltokirjaContext> logger, DbHuoltokirjaContext db, IConfiguration conf)
		{
			_logger = logger;
			_db = db;
			_conf = conf;
		}

		#region VAATIMUS
		[HttpGet("/vaatimus/all")]
		public async Task<IEnumerable<VaatimusDTO>> GetVaatimus()
		{
			return await _db.Vaatimus
				.Include(v=>v.IdauditointiNavigation)
				.Select(v=> Helpers.VaatimusToDTO(v)).ToListAsync();
		}

		[HttpPost("/vaatimus/add")]
		public async Task<ActionResult<Vaatimu>> AddVaatimus(VaatimusDTO req)
		{
			//var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (req == null)
			{
				return BadRequest("tiedot tyhjät");
			}
			else
			{
				Vaatimu v = new()
				{
					Kuvaus = req.Kuvaus,
					Pakollisuus = req.Pakollisuus,
					Taytetty = req.Taytetty,
					Idauditointi = req.Idauditointi,
				};
				_db.Vaatimus.Add(v);
				await _db.SaveChangesAsync();

				return Ok(v);

			}
		}
		#endregion

		#region VAATIMUSPOHJA
		[HttpGet("/vaatimuspohja/all")]
		public async Task<IEnumerable<VaatimuspohjaDTO>> GetVaatimuspohjat()
		{
			return await _db.Vaatimuspohjas
				.Include(v=>v.IdauditointipohjaNavigation)
				.Select(v=>Helpers.VaatimuspohjaToDTO(v)).ToListAsync();	
		}

		[HttpGet("/vaatimuspohja/{id}")]
		public async Task<ActionResult<VaatimuspohjaDTO>> GetSingle(int? id)
		{
			if (id == null) return BadRequest("ei id:tä");

			return await _db.Vaatimuspohjas.Where(v => v.Idvaatimuspohja == id)
				.Include(v=>v.IdauditointipohjaNavigation)
				.Select(v=>Helpers.VaatimuspohjaToDTO(v))
				.FirstOrDefaultAsync();
		}

		[HttpPost("/vaatimuspohja/add")]
		public async Task<ActionResult<Vaatimuspohja>> AddVaatimusPohja(VaatimuspohjaDTO req)
		{
			if(req == null)
			{
				return BadRequest("ei tietoja");
			}
			else
			{
				Vaatimuspohja v = new()
				{
					Kuvaus= req.Kuvaus,	
					Pakollisuus= req.Pakollisuus,
					Idauditointipohja= req.Idauditointipohja,
				};
				_db.Vaatimuspohjas.Add(v);	
				await _db.SaveChangesAsync();
				return Ok(v);
			}
		}

		[HttpPut("/vaatimuspohja/edit")]
		public async Task<IActionResult> EditVaatimuspohja(VaatimuspohjaDTO req)
		{
			var v = await _db.Vaatimuspohjas.Where(i => i.Idvaatimuspohja == req.Idvaatimuspohja).FirstOrDefaultAsync();
			
			if(req == null || v == null)
			{
				return BadRequest("ei tietoja");
			}

			v.Idvaatimuspohja = req.Idvaatimuspohja;
			v.Kuvaus = req.Kuvaus;
			v.Pakollisuus = req.Pakollisuus;
			v.Idauditointipohja = v.Idauditointipohja;

			_db.Vaatimuspohjas.Update(v);
			await _db.SaveChangesAsync();

			return Ok(v);

		}

		[HttpDelete("/vaatimuspohja/delete/{id}")]
		public async Task<IActionResult> DeleteVaatimuspohja(int? id)
		{
			if(id != null)
			{
				var v = await _db.Vaatimuspohjas.Where(i => i.Idvaatimuspohja == id).FirstOrDefaultAsync();
				if(v != null)
				{
					_db.Vaatimuspohjas.Remove(v);
					await _db.SaveChangesAsync();
					return NoContent();
				}
				return BadRequest();
			}
			
				return BadRequest();
			

		}

		#endregion
	}
}
