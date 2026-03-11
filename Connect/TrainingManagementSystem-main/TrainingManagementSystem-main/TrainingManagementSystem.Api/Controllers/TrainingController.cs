using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingManagementSystem.Api.DTO;
using TrainingManagementSystem.Api.Services.TrainingService;

namespace TrainingManagementSystem.Api.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingController(ITrainingService _trainingService) : ControllerBase
    {
        [HttpGet("trainings")]
        public async Task<ActionResult> GetAllTrainings(CancellationToken token) 
        {
            //get all trainings
            var result = await _trainingService.GetAllTrainingsAsync(token);

            if (result.IsFailure)
                return BadRequest(result.Message);

            return Ok(result.Value);  
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateTraining([FromForm]CreateTrainingRequest request, CancellationToken token) 
        {
            var result = await _trainingService.CreateTrainingAsync(request, token);

            if (result.IsFailure)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        //implement delete
        [HttpDelete("delete/{trainingId}")]
        public async Task<ActionResult> Delete([FromRoute]Guid trainingId, CancellationToken token) 
        {
            var result = await _trainingService.DeleteAsync(trainingId,token);

            if (result.IsFailure)
                return NotFound(result.Message);

            return Ok(result.Message);
        }

        //implement update
        [HttpPut("update/{trainingId}")]
        public async Task<ActionResult> Update( 
                Guid trainingId,
                [FromForm]UpdateTrainingRequest request, 
                CancellationToken token) 
        {
            var result = await _trainingService.UpdateTrainingAsync(trainingId, request, token);

            if (result.IsFailure)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        //march 02
        //implement get by id
        [HttpGet("{trainingId}")]
        public async Task<ActionResult> GetById(Guid trainingId, CancellationToken token) 
        {
            var result = await _trainingService.GetByIdAsync(trainingId, token);

            if(result.IsFailure)
                return NotFound(result.Message);

            return Ok(result.Value);
        }

        [HttpPost("{trainingId}/attendees")]
        public async Task<ActionResult> AddAttendee(
    [FromRoute] Guid trainingId,
    [FromForm] AddAttendeeRequest request,
    CancellationToken token)
        {
            var result = await _trainingService.AddAttendeeAsync(trainingId, request, token);
            if (result.IsFailure)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpPost("{trainingId}/materials")]
        public async Task<ActionResult> AddMaterials(
            [FromRoute] Guid trainingId,
            [FromForm] AddMaterialRequest request,
            CancellationToken token)
        {
            var result = await _trainingService.AddMaterialAsync(trainingId, request, token);
            if (result.IsFailure)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

    }
}
