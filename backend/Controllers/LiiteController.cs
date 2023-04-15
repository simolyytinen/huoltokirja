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
    public class LiiteController : ControllerBase
    {
        private readonly ILogger<DbHuoltokirjaContext> _logger;
        private readonly DbHuoltokirjaContext _db;

        public LiiteController(ILogger<DbHuoltokirjaContext> logger, DbHuoltokirjaContext db)
        {
            _logger = logger;
            _db = db;

        }


        // Haetaan kaikki
        [HttpGet("/liite/kaikki")]
        public async Task<ActionResult<IEnumerable<Liite>>> HaeKaikkiLiitteet()
        {
            return await _db.Liites.ToListAsync();
        }

        // Poistetaan liite
        [HttpPost("/liite/poista")]
        public async Task<IActionResult> PoistaLiite([FromBody] UploadResult u)
        {

            var liite = await _db.Liites
                .Where(l => l.Sijainti.Equals(u.Location))
                .FirstOrDefaultAsync();

            if (liite == null)
            {
                return NotFound();
            }

            _db.Liites.Remove(liite);
            await _db.SaveChangesAsync();
            return Ok("Poistettu");
        }



    }
}
