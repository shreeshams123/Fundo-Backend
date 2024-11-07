using BusinessLayer.Interfaces;
using BusinessLayer.Utilities;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class NoteBL : INoteBL
    {
        private readonly INoteDL _noteDL;
        private readonly TokenHelper _tokenHelper;
        public NoteBL(INoteDL noteDL, TokenHelper tokenHelper)
        {
            _noteDL = noteDL;
            _tokenHelper = tokenHelper;
            
        }
        public async Task<NoteResponseDto> CreateNoteAsync(NoteDto noteDto)
        {
            if (noteDto == null)
                throw new ArgumentNullException(nameof(noteDto));

            var userId = _tokenHelper.GetUserIdFromToken();
            var newnote = new Note
            {
                UserId = userId,
                Title = noteDto.Title,
                Description = noteDto.Description,
                Color = string.IsNullOrWhiteSpace(noteDto.Color) ? null : noteDto.Color
            };
            return await _noteDL.CreateNoteInDbAsync(newnote,userId);
        }
                   
        public async Task<IEnumerable<NoteResponseDto>> GetAllNoteAsync()
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            return await _noteDL.GetAllNotesInDb(userId);
        }

        public async Task<NoteResponseDto> GetNoteByIdAsync(int noteId)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            return await _noteDL.GetNoteByIdInDb(noteId, userId);
        }
 
        public async Task<NoteResponseDto> UpdateNoteAsync(NoteUpdateDto noteUpdateDto, int NoteId)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            return await _noteDL.UpdateNoteInDb(noteUpdateDto, NoteId, userId);
        }
        public async Task DeleteNoteAsync(int NoteId)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            await _noteDL.DeleteNoteInDb(NoteId,userId);
        }
    }
}

    

