using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace backend.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class KohderyhmaController : ControllerBase
    {

        private readonly ILogger<DbHuoltokirjaContext> _logger;
        private readonly DbHuoltokirjaContext _db;

        public KohderyhmaController(ILogger<DbHuoltokirjaContext> logger, DbHuoltokirjaContext db)
        {
            _logger = logger;
            _db = db;
        }

        // Kaikki kohderyhmät
        [HttpGet("/kohderyhma/all")]
        public async Task<ActionResult<IEnumerable<KohderyhmaDTO>>> GetAll()
        {

            return await _db.Kohderyhmas.OrderBy(a => a.Idkohderyhma).Select(a => Helpers.KohderyhmaToDTO(a)).ToListAsync();

        }
    }
}