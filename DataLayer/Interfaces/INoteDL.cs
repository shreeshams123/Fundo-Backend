using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface INoteDL
    {
        Task<NoteResponseDto> CreateNoteInDbAsync(Note note);
        Task<IEnumerable<NoteResponseDto>> GetNoteByIdAsync(int userId);
        Task<NoteResponseDto> UpdateNoteInDb(NoteUpdateDto noteUpdateDto, int NoteId,int UserId);
        Task DeleteNoteInDb(int noteId,int userId);


    }
}
