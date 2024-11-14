using Models;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ICollaboratorBL
    {
        Task<ApiResponse<UserDto>> AddCollaboratorAsync(int NoteId, string Email);
        Task<ApiResponse<string>> DeleteCollaboratorAsync(int NoteId, string Email);
        Task<ApiResponse<IEnumerable<UserDto>>> GetCollaboratorsAsync(int NoteId);
    }
}
