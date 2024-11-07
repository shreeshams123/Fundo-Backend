using DataLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
namespace FunDo.Controllers
{
    [ApiController]
    [Route("api/Collaborator")]
    public class CollaboratorController:ControllerBase
    {
        private readonly ICollaboratorBL _collabBL;
        public CollaboratorController(ICollaboratorBL collabBL)
        {
            _collabBL = collabBL;
        }
        [Authorize]
        [HttpPost("AddCollaborator/{NoteId}")]
        public async Task<IActionResult> AddCollaborator([FromRoute]int NoteId,string Email)
        {
            var newCollaborator=await _collabBL.AddCollaboratorAsync(NoteId, Email);
            if (newCollaborator != null)
            {
                return Ok(new { Message = "Collaborator added successfully", Collaborator = newCollaborator });
            }
            return BadRequest("Failed to add collaborator");
        }
        [Authorize]
        [HttpDelete("DeleteCollaborator/{NoteId}/{Email}")]
        public async Task<IActionResult> DeleteCollaborator([FromRoute]int NoteId, [FromRoute]string Email)
        {
            await _collabBL.DeleteCollaboratorAsync(NoteId, Email);
            return Ok();
        }
        [Authorize]
        [HttpGet("GetCollaborators/{NoteId}")]
        public async Task<IActionResult> GetCollaborators([FromRoute]int NoteId)
        {
            var result=await _collabBL.GetCollaboratorsAsync(NoteId);
            return Ok(result);
        }
    }
}
