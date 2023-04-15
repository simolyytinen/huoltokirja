using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TilaController : ControllerBase
    {

        private readonly ILogger<DbHuoltokirjaContext> _logger;
        private readonly DbHuoltokirjaContext _db;

        public TilaController(ILogger<DbHuoltokirjaContext> logger, DbHuoltokirjaContext db)
        {
            _logger = logger;
            _db = db;
        }

        // Kaikki kohderyhmät
        [HttpGet("/tila/all")]
        public async Task<ActionResult<IEnumerable<TilaDTO>>> GetAll()
        {

            return await _db.KohteenTilas.OrderBy(a => a.IdkohteenTila).Select(a => Helpers.TilaToDTO(a)).ToListAsync();

        }

        [HttpGet("/tila/all1or2")]
        public async Task<ActionResult<IEnumerable<TilaDTO>>> GetAll1or2()
        {

            return await _db.KohteenTilas.Where(a => a.IdkohteenTila < 3).OrderBy(a => a.IdkohteenTila).Select(a => Helpers.TilaToDTO(a)).ToListAsync();

        }
    }
}