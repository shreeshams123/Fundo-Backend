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
        Task<UserDto>AddCollaboratorAsync(int NoteId, string Email);
        Task DeleteCollaboratorAsync(int NoteId, string Email);
        Task<IEnumerable<UserDto>> GetCollaboratorsAsync(int NoteId);
    }
}
