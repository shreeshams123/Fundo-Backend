using Models;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface INoteBL
    {
        Task<ApiResponse<NoteResponseDto>> CreateNoteAsync(NoteDto noteDto);
        Task<ApiResponse<IEnumerable<NoteResponseDto>>> GetAllNoteAsync();
        Task<ApiResponse<NoteResponseDto>> GetNoteByIdAsync(int noteId);
        Task<ApiResponse<NoteResponseDto>> UpdateNoteAsync(NoteUpdateDto noteUpdateDto, int NoteId);
        Task<ApiResponse<string>> DeleteNoteAsync(int NoteId);
        Task<ApiResponse<string>> ToggleArchiveAsync(int NoteId, bool isArchive);
         Task<ApiResponse<string>> ToggleTrashAsync(int NoteId, bool isTrash);
    }
}
