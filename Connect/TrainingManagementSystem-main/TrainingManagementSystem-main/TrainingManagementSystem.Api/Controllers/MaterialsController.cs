using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingManagementSystem.Api.Data;
using TrainingManagementSystem.Api.DTO;
using TrainingManagementSystem.Api.Services.MaterialService;

namespace TrainingManagementSystem.Api.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialsController(IMaterialService _service, AppDbContext _context) : ControllerBase
    {
        //March 02 (refactor this by separating concerns)
        //add endpoint for viewing 
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var material = await _context.Materials.FindAsync(id);

            if (material == null || material.Disabled)
                return NotFound();

            if (material.IsExternalLink)
                return Redirect(material.ExternalUrl!);

            var bytes = await System.IO.File.ReadAllBytesAsync(material.StoragePath);

            return File(bytes, material.MimeType, material.OriginalFileName);
        }

        //redundant since training fetch can include training materials?
        //[HttpGet("getall")]
        //public async Task<IActionResult> GetTrainingMaterials(Guid trainingId, CancellationToken token) 
        //{
        //    var result = await _service.GetTrainingMaterialsAsync(trainingId, token);

        //    if (result.IsFailure)
        //        return BadRequest(result.Message);

        //    return Ok(result.Value);
        //}

        //[HttpPost("upload")]
        //public async Task<IActionResult> Upload(
        //    [FromForm] UploadMaterialRequest request,
        //    CancellationToken token)
        //{
        //    var result = await _service.UploadMaterialAsync(request, token);

        //    if (result.IsFailure)
        //        return BadRequest(result.Message);

        //    return Ok("Material uploaded");
        //}

        //[HttpPost("external")]
        //public async Task<IActionResult> AddExternal(
        //    AddExternalMaterialRequest request,
        //    CancellationToken token)
        //{
        //    var result =  await _service.AddExternalMaterialAsync(request, token);

        //    if(result.IsFailure)
        //        return BadRequest(result.Message);

        //    return Ok("External material added");
        //}

        //[HttpDelete("delete/{materialId}")]
        //public async Task<IActionResult> Delete(Guid materialId, CancellationToken token)
        //{
        //    var result = await _service.DeleteMaterialAsync(materialId, token);

        //    if (result.IsFailure)
        //        return BadRequest(result.Message);

        //    return Ok("Material removed");
        //}

    }
}
