using BusinessLayer.Interfaces;
using BusinessLayer.Utilities;
using DataLayer.Interfaces;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class CollaboratorBL:ICollaboratorBL
    {
        private readonly ICollaboratorDL _collabrepo;
        private readonly TokenHelper _tokenHelper;
        public CollaboratorBL(ICollaboratorDL collabrepo,TokenHelper tokenHelper)
        {
            _collabrepo = collabrepo;
            _tokenHelper = tokenHelper;
        }

        public async Task<UserDto> AddCollaboratorAsync(int NoteId, string Email)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            return await _collabrepo.AddCollaboratorToDb(NoteId, Email,userId);
        }
        public async Task DeleteCollaboratorAsync(int NoteId,string Email)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            await _collabrepo.DeleteCollaboratorInDb(NoteId,Email,userId);
        }
        public async Task<IEnumerable<UserDto>> GetCollaboratorsAsync(int NoteId)
        {
            var userId= _tokenHelper.GetUserIdFromToken();
            return await _collabrepo.GetCollaborators(NoteId,userId);

        }
    }
}
