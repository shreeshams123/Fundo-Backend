using BusinessLayer.Interfaces;
using BusinessLayer.Utilities;
using DataLayer.Interfaces;
using Models;
using Models.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class CollaboratorBL : ICollaboratorBL
    {
        private readonly ICollaboratorDL _collabrepo;
        private readonly TokenHelper _tokenHelper;
        private readonly ILogger<CollaboratorBL> _logger;

        public CollaboratorBL(ICollaboratorDL collabrepo, TokenHelper tokenHelper, ILogger<CollaboratorBL> logger)
        {
            _collabrepo = collabrepo;
            _tokenHelper = tokenHelper;
            _logger = logger;
        }

        public async Task<ApiResponse<UserDto>> AddCollaboratorAsync(int NoteId, string Email)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            _logger.LogInformation("Attempting to add collaborator with email: {Email} to note with ID: {NoteId} by user with ID: {UserId}", Email, NoteId, userId);
            var apiResponse = await _collabrepo.AddCollaboratorToDb(NoteId, Email, userId);
            if (apiResponse.Success)
            {
                _logger.LogInformation("Successfully added collaborator with email: {Email} to note with ID: {NoteId}", Email, NoteId);
            }
            else
            {
                _logger.LogWarning("Failed to add collaborator with email: {Email} to note with ID: {NoteId}. Reason: {Message}", Email, NoteId, apiResponse.Message);
            }
            return apiResponse;
        }

        public async Task<ApiResponse<string>> DeleteCollaboratorAsync(int NoteId, string Email)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            _logger.LogInformation("Attempting to delete collaborator with email: {Email} from note with ID: {NoteId} by user with ID: {UserId}", Email, NoteId, userId);
            var apiResponse = await _collabrepo.DeleteCollaboratorInDb(NoteId, Email, userId);
            if (apiResponse.Success)
            {
                _logger.LogInformation("Successfully deleted collaborator with email: {Email} from note with ID: {NoteId}", Email, NoteId);
            }
            else
            {
                _logger.LogWarning("Failed to delete collaborator with email: {Email} from note with ID: {NoteId}. Reason: {Message}", Email, NoteId, apiResponse.Message);
            }
            return apiResponse;
        }

        public async Task<ApiResponse<IEnumerable<UserDto>>> GetCollaboratorsAsync(int NoteId)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            _logger.LogInformation("Attempting to retrieve collaborators for note with ID: {NoteId} by user with ID: {UserId}", NoteId, userId);
            var apiResponse = await _collabrepo.GetCollaborators(NoteId, userId);
            if (apiResponse.Success)
            {
                _logger.LogInformation("Successfully retrieved collaborators for note with ID: {NoteId}", NoteId);
            }
            else
            {
                _logger.LogWarning("Failed to retrieve collaborators for note with ID: {NoteId}. Reason: {Message}", NoteId, apiResponse.Message);
            }
            return apiResponse;
        }
    }
}
