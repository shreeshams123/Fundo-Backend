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
        Task<UserDto> AddCollaboratorToDb(int NoteId, string Email,int userId);
        Task DeleteCollaboratorInDb(int NoteId, string Email, int ownerId);
        Task<IEnumerable<UserDto>> GetCollaborators(int NoteId, int ownerId);
    }
}
