using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;

namespace FunDo.Controllers
{
    [ApiController]
    [Route("api/note")]
    public class NoteController : Controller
    {
        private readonly INoteBL _noteBL;
        private readonly ILogger<NoteController> _logger;
        public NoteController(INoteBL noteBL, ILogger<NoteController> logger)
        {
            _noteBL = noteBL;
            _logger = logger;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateNote(NoteDto noteDto)
        {
            _logger.LogInformation("Attempt to create a note");
            var apiResponse = await _noteBL.CreateNoteAsync(noteDto);
            if (apiResponse.Success)
            {
                _logger.LogInformation("Created notes successfully");
                return Ok(apiResponse);
            }
            _logger.LogWarning("Note creation failed");
            return BadRequest(apiResponse);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllNotes()
        {
            _logger.LogInformation("Attempt to get all notes from Database");
            var apiResponse=await _noteBL.GetAllNoteAsync();
            if (apiResponse.Success)
            {
                _logger.LogInformation("Notes retrieved successfully");
                return Ok(apiResponse);
            }
            _logger.LogInformation("Retrieving notes failed");
            return BadRequest(apiResponse);

        }
        [HttpGet("get/{noteId}")]
        [Authorize]
        public async Task<IActionResult> GetNotesById([FromRoute] int noteId)
        {
            _logger.LogInformation("Started retrieving notes by Id");
            var apiResponse = await _noteBL.GetNoteByIdAsync(noteId);
            if (apiResponse.Success)
            {
                return Ok(apiResponse);
            }
            return BadRequest(apiResponse);
        }


        [HttpPatch("update/{noteId}")]
        [Authorize]
        public async Task<IActionResult> UpdateNote([FromRoute] int noteId, NoteUpdateDto noteUpdateDto)
        {
            _logger.LogInformation("Updating note with ID: {NoteId}", noteId);
            var apiResponse = await _noteBL.UpdateNoteAsync(noteUpdateDto, noteId);
            if (apiResponse.Success)
            {
                return Ok(apiResponse);
            }
            return BadRequest(apiResponse);
        }
        [HttpDelete("delete/{noteId}")]
        [Authorize]
        public async Task<IActionResult> DeleteNote([FromRoute] int noteId)
        {
            _logger.LogInformation("Received request to delete note with ID: {NoteId}", noteId);
            var apiresponse = await _noteBL.DeleteNoteAsync(noteId);
            if (apiresponse.Success)
            {
                _logger.LogInformation("Successfully deleted note with ID: {NoteId}", noteId);
                return Ok(apiresponse);
            }
            _logger.LogWarning("Failed to delete note with ID: {NoteId}. Reason: {Message}", noteId, apiresponse.Message);
            return BadRequest(apiresponse);
        }
        [HttpPatch("archive/{noteId}")]
        [Authorize]
        public async Task<IActionResult> ToggleArchiveNote([FromRoute] int noteId, bool isArchive)
        {
            _logger.LogInformation("Received request to toggle archive status for note with ID: {NoteId}", noteId);
            var apiresponse = await _noteBL.ToggleArchiveAsync(noteId, isArchive);
            if (apiresponse.Success)
            {
                _logger.LogInformation("Successfully toggled archive status for note with ID: {NoteId}", noteId);
                return Ok(apiresponse);
            }
            _logger.LogWarning("Failed to toggle archive status for note with ID: {NoteId}. Reason: {Message}", noteId, apiresponse.Message);
            return BadRequest(apiresponse);
        }
        [HttpPatch("trash/{noteId}")]
        [Authorize]
        public async Task<IActionResult> ToggleTrashNote([FromRoute] int noteId, bool isTrash)
        {
            _logger.LogInformation("Received request to toggle trash status for note with ID: {NoteId}", noteId);
            var apiresponse = await _noteBL.ToggleTrashAsync(noteId, isTrash);
            if (apiresponse.Success)
            {
                _logger.LogInformation("Successfully toggled trash status for note with ID: {NoteId}", noteId);
                return Ok(apiresponse);
            }
            _logger.LogWarning("Failed to toggle trash status for note with ID: {NoteId}. Reason: {Message}", noteId, apiresponse.Message);
            return BadRequest(apiresponse);
        }



    }
}
