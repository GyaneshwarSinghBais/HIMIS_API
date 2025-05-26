using HIMIS_API.Data;
using HIMIS_API.Services.ProgressServices.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebSiteController : ControllerBase
    {

        private readonly DbContextWeb _context;
        public WebSiteController(DbContextWeb context)
        {
            _context = context;
        }


        //[HttpGet("DistFACwiseStockPostionNew")]
        //public async Task<ActionResult<IEnumerable<DistFACwiseStockPostionDTO>>> DistFACwiseStockPostionNew(string disid, string coll_cmho, string mcatid, string EDLNedl, string mitemid, string userid)
        //{


        //}
    }
}
