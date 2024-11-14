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
    public interface ICollaboratorDL
    {
        Task<ApiResponse<UserDto>> AddCollaboratorToDb(int noteId, string email, int userId);
        Task<ApiResponse<string>> DeleteCollaboratorInDb(int noteId, string email, int userId);
        Task<ApiResponse<IEnumerable<UserDto>>> GetCollaborators(int NoteId, int ownerId);
    }
}
