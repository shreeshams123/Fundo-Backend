using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ILabelBL
    {
        Task<LabelDto> AddLabelsToDbAsync(LabelRequestDto labelRequestDto);
        Task UpdateLabelToNotesAsync(int noteId, List<int> labelIds);
        Task<List<LabelCheckListDto>> GetLabelChecklistForNote(int noteId);
        Task<List<LabelDto>> GetAllLabelsAsync();
        Task DeleteLabelAsync(int Id);
    }
}
