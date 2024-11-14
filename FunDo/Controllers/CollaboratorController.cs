using DataLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace FunDo.Controllers
{
    [ApiController]
    [Route("api/Collaborator")]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorBL _collabBL;
        private readonly ILogger<CollaboratorController> _logger;

        public CollaboratorController(ICollaboratorBL collabBL, ILogger<CollaboratorController> logger)
        {
            _collabBL = collabBL;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("AddCollaborator/{NoteId}")]
        public async Task<IActionResult> AddCollaborator([FromRoute] int NoteId, string Email)
        {
            _logger.LogInformation("Attempting to add collaborator with email: {Email} to note with ID: {NoteId}", Email, NoteId);
            var apiresponse = await _collabBL.AddCollaboratorAsync(NoteId, Email);
            if (apiresponse.Success)
            {
                _logger.LogInformation("Successfully added collaborator with email: {Email} to note with ID: {NoteId}", Email, NoteId);
                return Ok(apiresponse);
            }
            _logger.LogWarning("Failed to add collaborator with email: {Email} to note with ID: {NoteId}. Reason: {Message}", Email, NoteId, apiresponse.Message);
            return BadRequest(apiresponse);
        }

        [Authorize]
        [HttpDelete("DeleteCollaborator/{NoteId}/{Email}")]
        public async Task<IActionResult> DeleteCollaborator([FromRoute] int NoteId, [FromRoute] string Email)
        {
            _logger.LogInformation("Attempting to delete collaborator with email: {Email} from note with ID: {NoteId}", Email, NoteId);
            var apiresponse = await _collabBL.DeleteCollaboratorAsync(NoteId, Email);
            if (apiresponse.Success)
            {
                _logger.LogInformation("Successfully deleted collaborator with email: {Email} from note with ID: {NoteId}", Email, NoteId);
                return Ok(apiresponse);
            }
            _logger.LogWarning("Failed to delete collaborator with email: {Email} from note with ID: {NoteId}. Reason: {Message}", Email, NoteId, apiresponse.Message);
            return BadRequest(apiresponse);
        }

        [Authorize]
        [HttpGet("GetCollaborators/{NoteId}")]
        public async Task<IActionResult> GetCollaborators([FromRoute] int NoteId)
        {
            _logger.LogInformation("Attempting to get collaborators for note with ID: {NoteId}", NoteId);
            var apiResponse = await _collabBL.GetCollaboratorsAsync(NoteId);
            if (apiResponse.Success)
            {
                _logger.LogInformation("Successfully retrieved collaborators for note with ID: {NoteId}", NoteId);
                return Ok(apiResponse);
            }
            _logger.LogWarning("Failed to retrieve collaborators for note with ID: {NoteId}. Reason: {Message}", NoteId, apiResponse.Message);
            return BadRequest(apiResponse);
        }
    }
}
