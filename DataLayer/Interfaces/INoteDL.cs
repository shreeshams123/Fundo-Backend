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
        Task<NoteResponseDto> CreateNoteInDbAsync(Note note,int userId);
        Task<IEnumerable<NoteResponseDto>> GetAllNotesInDb(int userId);
        Task<NoteResponseDto> GetNoteByIdInDb(int NoteId, int UserId);
        Task<NoteResponseDto> UpdateNoteInDb(NoteUpdateDto noteUpdateDto, int NoteId,int UserId);
        Task DeleteNoteInDb(int noteId,int userId);
        Task toggleArchive(int noteId, int userId, bool isArchive);
        Task toggleTrash(int noteId, int userId, bool isTrash);

    }
}
