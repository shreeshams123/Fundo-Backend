using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;

namespace FunDo.Controllers
{
    [ApiController]
    [Route("note")]
    public class NoteController : Controller
    {
        private readonly INoteBL _noteBL;
        public NoteController(INoteBL noteBL)
        {
            _noteBL = noteBL;
        }
        [Authorize]
        [HttpPost("add note")]
        public async Task<IActionResult> CreateNote(NoteDto noteDto)
        {
            var newnote = await _noteBL.CreateNoteAsync(noteDto);
            if (newnote != null)
            {
                Console.WriteLine("Created note successfully");
                return Ok(newnote);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet("get notes")]
        [Authorize]
        public async Task<IActionResult> GetNotes()
        {
            return Ok(await _noteBL.GetNoteAsync());

        }
        [HttpPut("update notes/{noteId}")]
        [Authorize]
        public async Task<IActionResult> UpdateNote([FromRoute] int noteId, NoteUpdateDto noteUpdateDto)
        {
            var note = await _noteBL.UpdateNoteAsync(noteUpdateDto, noteId);
            return Ok(note);
        }
        [HttpDelete("delete note/{noteId}")]
        [Authorize]
        public async Task<IActionResult> DeleteNote([FromRoute] int noteId)
        {
            await _noteBL.DeleteNoteAsync(noteId);
            return Ok();
        }


    }
}
