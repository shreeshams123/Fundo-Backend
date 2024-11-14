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
    public interface ILabelDL
    {
        Task<ApiResponse<LabelDto>> AddLabelsToDb(LabelRequestDto label);
        Task<ApiResponse<string>> UpdateNoteLabel(int noteId, int userId, List<int> LabelIds);
        Task<ApiResponse<IEnumerable<LabelDto>>> GetAllLabelsFromDb();
        Task<ApiResponse<IEnumerable<int>>> GetLabelsForNoteFromDb(int noteId, int userId);
        Task<ApiResponse<string>> DeleteLabelsFromDb(int Id);
    }
}
