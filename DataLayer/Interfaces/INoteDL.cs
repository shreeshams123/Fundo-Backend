using Models;
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
        Task<ApiResponse<NoteResponseDto>> CreateNoteInDbAsync(Note note, int userId);
        Task<ApiResponse<IEnumerable<NoteResponseDto>>> GetAllNotesInDb(int userId);
        Task<ApiResponse<NoteResponseDto>> GetNoteByIdInDb(int NoteId, int userId);
        Task<ApiResponse<NoteResponseDto>> UpdateNoteInDb(NoteUpdateDto noteUpdateDto, int noteId, int userId);
        Task<ApiResponse<string>> DeleteNoteInDb(int noteId, int userId);
        Task<ApiResponse<string>> ToggleArchive(int noteId, int userId, bool isArchive);
        Task<ApiResponse<string>> ToggleTrash(int noteId, int userId, bool isTrash);

    }
}
