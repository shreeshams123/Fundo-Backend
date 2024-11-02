using BusinessLayer.Interfaces;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NoteBL(INoteDL noteDL, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _noteDL = noteDL;
        }
        public async Task<NoteResponseDto> CreateNoteAsync(NoteDto noteDto)
        {
            var userId = GetUserIdFromToken();
            var newnote = new Note
            {
                UserId = userId,
                Title = noteDto.Title,
                Description = noteDto.Description,
                Color = string.IsNullOrWhiteSpace(noteDto.Color) ? null : noteDto.Color
            };
            return await _noteDL.CreateNoteInDbAsync(newnote);
        }
        private int GetUserIdFromToken()
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var userIdClaim = jwtToken?.Claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            if ((userIdClaim != null))
            {
                return int.Parse(userIdClaim);
            }

            throw new Exception("Unauthorized User");
        }
        public async Task<IEnumerable<NoteResponseDto>> GetNoteAsync()
        {
            var userId = GetUserIdFromToken();
            return await _noteDL.GetNoteByIdAsync(userId);
        }
        public async Task<NoteResponseDto> UpdateNoteAsync(NoteUpdateDto noteUpdateDto, int NoteId)
        {
            var userId = GetUserIdFromToken();
            return await _noteDL.UpdateNoteInDb(noteUpdateDto, NoteId, userId);
        }
        public async Task DeleteNoteAsync(int NoteId)
        {
            var userId = GetUserIdFromToken();
            await _noteDL.DeleteNoteInDb(NoteId,userId);
        }
    }
}

    

