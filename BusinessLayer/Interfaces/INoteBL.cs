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
        Task<NoteResponseDto> CreateNoteAsync(NoteDto noteDto);
        Task<IEnumerable<NoteResponseDto>> GetNoteAsync();
        Task<NoteResponseDto> UpdateNoteAsync(NoteUpdateDto noteUpdateDto, int NoteId);
        Task DeleteNoteAsync(int NoteId);
    }
}
