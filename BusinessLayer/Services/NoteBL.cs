using BusinessLayer.Interfaces;
using BusinessLayer.Utilities;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BusinessLayer.Services
{
    public class NoteBL : INoteBL
    {
        private readonly INoteDL _noteDL;
        private readonly TokenHelper _tokenHelper;
        private readonly ILogger<NoteBL> _logger;
        private readonly IDistributedCache _distributedCache;
        public NoteBL(INoteDL noteDL, TokenHelper tokenHelper,ILogger<NoteBL> logger, IDistributedCache distributedCache)
        {
            _noteDL = noteDL;
            _tokenHelper = tokenHelper;
            _logger = logger;
            _distributedCache = distributedCache;
        }
        public async Task<ApiResponse<NoteResponseDto>> CreateNoteAsync(NoteDto noteDto)
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

        public async Task<ApiResponse<IEnumerable<NoteResponseDto>>> GetAllNoteAsync()
        {
           
            var userId = _tokenHelper.GetUserIdFromToken();
            _logger.LogInformation($"Fetching all notes for user with ID: {userId}", userId);
            return await _noteDL.GetAllNotesInDb(userId);
            
            
        }

        public async Task<ApiResponse<NoteResponseDto>> GetNoteByIdAsync(int noteId)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            _logger.LogInformation("Attempting to retrieve note with ID: {NoteId} for user ID: {UserId}", noteId, userId);
            return await _noteDL.GetNoteByIdInDb(noteId, userId);
        }

        public async Task<ApiResponse<NoteResponseDto>> UpdateNoteAsync(NoteUpdateDto noteUpdateDto, int noteId)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            _logger.LogInformation($"User with ID: {userId} is attempting to update note with ID: {noteId}", userId, noteId);
            return await _noteDL.UpdateNoteInDb(noteUpdateDto, noteId, userId);
        }
        public async Task<ApiResponse<string>> DeleteNoteAsync(int noteId)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            _logger.LogInformation($"Initiating deletion of note with ID: {noteId} for user ID: {userId}", noteId, userId);
            return await _noteDL.DeleteNoteInDb(noteId, userId);
        }
        public async Task<ApiResponse<string>> ToggleArchiveAsync(int noteId, bool isArchive)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            _logger.LogInformation("Initiating toggle of archive status for note with ID: {NoteId} for user ID: {UserId}", noteId, userId);
            return await _noteDL.ToggleArchive(noteId, userId, isArchive);
        }
        public async Task<ApiResponse<string>> ToggleTrashAsync(int noteId, bool isTrash)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            _logger.LogInformation("Initiating toggle of trash status for note with ID: {NoteId} for user ID: {UserId}", noteId, userId);
            return await _noteDL.ToggleTrash(noteId, userId, isTrash);
        }
    }
}

    

