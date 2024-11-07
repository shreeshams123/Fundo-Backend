using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Models.DTOs;

namespace FunDo.Controllers
{
    [ApiController]
    [Route("api/note")]
    public class NoteController : Controller
    {
        private readonly INoteBL _noteBL;
        public NoteController(INoteBL noteBL)
        {
            _noteBL = noteBL;
        }
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateNote(NoteDto noteDto)
        {
            var newnote = await _noteBL.CreateNoteAsync(noteDto);
            return Ok(newnote);
        }

        [HttpGet("get")]
        [Authorize]
        public async Task<IActionResult> GetAllNotes()
        {
            return Ok(await _noteBL.GetAllNoteAsync());

        }
        [HttpGet("get/{noteId}")]
        [Authorize]
        public async Task<IActionResult> GetNotesById([FromRoute] int noteId)
        {
            return Ok(await _noteBL.GetNoteByIdAsync(noteId));
        }
        [HttpPut("update/{noteId}")]
        [Authorize]
        public async Task<IActionResult> UpdateNote([FromRoute] int noteId, NoteUpdateDto noteUpdateDto)
        {
            var note = await _noteBL.UpdateNoteAsync(noteUpdateDto, noteId);
            return Ok(note);
        }
        [HttpDelete("delete/{noteId}")]
        [Authorize]
        public async Task<IActionResult> DeleteNote([FromRoute] int noteId)
        {
            await _noteBL.DeleteNoteAsync(noteId);
            return Ok();
        }


    }
}
