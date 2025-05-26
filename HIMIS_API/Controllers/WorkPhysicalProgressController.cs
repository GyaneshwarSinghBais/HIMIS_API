using HIMIS_API.Models;
using HIMIS_API.Services.MongoServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkPhysicalProgressController : ControllerBase
    {
        private readonly WorkPhysicalProgressService _service;

        public WorkPhysicalProgressController(WorkPhysicalProgressService service)
        {
            _service = service;
        }

        [HttpGet("GetBySR/{sr}")]
        public async Task<IActionResult> GetBySR(string sr)
        {
            var result = await _service.GetBySRAsync(sr);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Mongo_WorkPhysicalProgressModel document)
        {
            await _service.CreateAsync(document);
            return Ok(document);
        }
       

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(ObjectId id, [FromBody] Mongo_WorkPhysicalProgressModel updatedDocument)
        //{
        //    var existingDocument = await _service.GetByIdAsync(id);

        //    if (existingDocument == null)
        //    {
        //        return NotFound();
        //    }

        //    updatedDocument.Id = existingDocument.Id;
        //    await _service.UpdateAsync(id, updatedDocument);

        //    return Ok(updatedDocument);
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(ObjectId id)
        {
            var existingDocument = await _service.GetByIdAsync(id);

            if (existingDocument == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ObjectId format");
            }

            var document = await _service.GetByIdAsync(objectId);

            if (document == null)
            {
                return NotFound();
            }

            return Ok(document);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var documents = await _service.GetAllAsync();
            return Ok(documents);
        }


        [HttpGet("GetImageBinary")]
        public  string GetImageBinary(string sr, string imgName)
        {
            var documents =  _service.GetByIDmTypeAsync(sr, imgName);
            return documents;
        }
    }
}
