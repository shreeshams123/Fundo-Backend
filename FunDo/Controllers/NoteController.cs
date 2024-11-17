using BusinessLayer.Interfaces;
using BusinessLayer.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using Models.DTOs;
using System.Text.Json;

namespace FunDo.Controllers
{
    [ApiController]
    [Route("api/notes")]
    public class NoteController : Controller
    {
        private readonly INoteBL _noteBL;
        private readonly ILogger<NoteController> _logger;
        private readonly IDistributedCache _distributedCache;
        public NoteController(INoteBL noteBL, ILogger<NoteController> logger, IDistributedCache distributedCache)
        {
            _noteBL = noteBL;
            _logger = logger;
            _distributedCache = distributedCache;
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

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var cachenotes = await _distributedCache.GetStringAsync($"user:{userId}notes");
            _logger.LogInformation($"Attempt to fetch notes from cache for user {userId}", userId);
            if (!string.IsNullOrEmpty(cachenotes))
            {
                _logger.LogInformation("Fetched notes from cache successfully");
                _logger.LogInformation("Deserializing the notes");
                var notesfromcache = JsonSerializer.Deserialize<IEnumerable<NoteResponseDto>>(cachenotes);

                var apiresponse= new ApiResponse<IEnumerable<NoteResponseDto>>
                {
                    Success = true,
                    Message = "Retreived notes successfully",
                    Data = notesfromcache
                };
                return Ok(apiresponse);
            }
            _logger.LogWarning("Failed to fetch notes from cache");
            var apiResponse=await _noteBL.GetAllNoteAsync();
            if (apiResponse.Success)
            {
                _logger.LogInformation("Notes retrieved successfully");
                return Ok(apiResponse);
            }
            _logger.LogInformation("Retrieving notes failed");
            return BadRequest(apiResponse);

        }
        [HttpGet("{noteId}")]
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


        [HttpPatch("{noteId}")]
        [Authorize]
        public async Task<IActionResult> UpdateNote([FromRoute] int noteId, NoteUpdateDto noteUpdateDto)
        {
            _logger.LogInformation($"Updating note with ID: {noteId}");
            var apiResponse = await _noteBL.UpdateNoteAsync(noteUpdateDto, noteId);
            if (apiResponse.Success)
            {
                return Ok(apiResponse);
            }
            return BadRequest(apiResponse);
        }
        [HttpDelete("{noteId}")]
        [Authorize]
        public async Task<IActionResult> DeleteNote([FromRoute] int noteId)
        {
            _logger.LogInformation($"Received request to delete note with ID: {noteId}");
            var apiresponse = await _noteBL.DeleteNoteAsync(noteId);
            if (apiresponse.Success)
            {
                _logger.LogInformation($"Successfully deleted note with ID: {noteId}");
                return Ok(apiresponse);
            }
            _logger.LogWarning($"Failed to delete note with ID: {noteId}");
            return BadRequest(apiresponse);
        }
        [HttpPatch("archive/{noteId}")]
        [Authorize]
        public async Task<IActionResult> ToggleArchiveNote([FromRoute] int noteId, bool isArchive)
        {
            _logger.LogInformation($"Received request to toggle archive status for note with ID: {noteId}");
            var apiresponse = await _noteBL.ToggleArchiveAsync(noteId, isArchive);
            if (apiresponse.Success)
            {
                _logger.LogInformation($"Successfully toggled archive status for note with ID: {noteId}");
                return Ok(apiresponse);
            }
            _logger.LogWarning($"Failed to toggle archive status for note with ID: {noteId}");
            return BadRequest(apiresponse);
        }
        [HttpPatch("trash/{noteId}")]
        [Authorize]
        public async Task<IActionResult> ToggleTrashNote([FromRoute] int noteId, bool isTrash)
        {
            _logger.LogInformation($"Received request to toggle trash status for note with ID: {noteId}");
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
