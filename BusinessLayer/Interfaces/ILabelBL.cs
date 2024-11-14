using Models;
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
        Task<ApiResponse<LabelDto>> AddLabelsToDbAsync(LabelRequestDto labelRequestDto);
        Task<ApiResponse<string>> UpdateLabelToNotesAsync(int noteId, List<int> labelIds);
        Task<ApiResponse<IEnumerable<LabelCheckListDto>>> GetLabelChecklistForNote(int noteId);
        Task<ApiResponse<IEnumerable<LabelDto>>> GetAllLabelsAsync();
        Task<ApiResponse<string>> DeleteLabelAsync(int Id);
    }
}
