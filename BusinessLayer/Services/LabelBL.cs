using BusinessLayer.Interfaces;
using BusinessLayer.Utilities;
using DataLayer.Interfaces;
using Models;
using Models.DTOs;
using Models.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class LabelBL : ILabelBL
    {
        private readonly ILabelDL _labelDL;
        private readonly TokenHelper _tokenHelper;
        private readonly ILogger<LabelBL> _logger;

        public LabelBL(ILabelDL labelDL, TokenHelper tokenHelper, ILogger<LabelBL> logger)
        {
            _labelDL = labelDL;
            _tokenHelper = tokenHelper;
            _logger = logger;
        }

        public async Task<ApiResponse<LabelDto>> AddLabelsToDbAsync(LabelRequestDto labelRequestDto)
        {
            _logger.LogInformation("Adding label to database.");
            var response = await _labelDL.AddLabelsToDb(labelRequestDto);
            if (!response.Success)
            {
                _logger.LogWarning("Failed to add label: {Message}", response.Message);
            }
            else
            {
                _logger.LogInformation("Successfully added label with ID: {LabelId}", response.Data?.Id);
            }
            return response;
        }

        public async Task<ApiResponse<string>> UpdateLabelToNotesAsync(int noteId, List<int> labelIds)
        {
            _logger.LogInformation("Updating labels for note with ID: {NoteId}", noteId);
            var userId = _tokenHelper.GetUserIdFromToken();
            var response = await _labelDL.UpdateNoteLabel(noteId, userId, labelIds);
            if (!response.Success)
            {
                _logger.LogWarning("Failed to update labels for note ID {NoteId}: {Message}", noteId, response.Message);
            }
            return response;
        }

        public async Task<ApiResponse<IEnumerable<LabelDto>>> GetAllLabelsAsync()
        {
            _logger.LogInformation("Retrieving all labels from database.");
            var response = await _labelDL.GetAllLabelsFromDb();
            _logger.LogInformation("Retrieved {LabelCount} labels.", response.Data?.Count() ?? 0);
            return response;
        }

        public async Task<ApiResponse<IEnumerable<LabelCheckListDto>>> GetLabelChecklistForNote(int noteId)
        {
            _logger.LogInformation("Getting label checklist for note with ID: {NoteId}", noteId);
            var userId = _tokenHelper.GetUserIdFromToken();

            var allLabelsResponse = await _labelDL.GetAllLabelsFromDb();
            var noteLabelsResponse = await _labelDL.GetLabelsForNoteFromDb(noteId, userId);

            if (!allLabelsResponse.Success || !noteLabelsResponse.Success)
            {
                _logger.LogWarning("Failed to retrieve labels or note labels for note ID {NoteId}.", noteId);
                return new ApiResponse<IEnumerable<LabelCheckListDto>>
                {
                    Success = false,
                    Message = "Failed to retrieve labels",
                    Data = null
                };
            }

            var labelCheckList = allLabelsResponse.Data.Select(label => new LabelCheckListDto
            {
                LabelId = label.Id,
                LabelName = label.Name,
                IsChecked = noteLabelsResponse.Data.Contains(label.Id)
            });

            _logger.LogInformation("Retrieved label checklist for note ID: {NoteId}", noteId);
            return new ApiResponse<IEnumerable<LabelCheckListDto>>
            {
                Success = true,
                Message = "Retrieved checklist successfully",
                Data = labelCheckList
            };
        }

        public async Task<ApiResponse<string>> DeleteLabelAsync(int id)
        {
            _logger.LogInformation("Deleting label with ID: {LabelId}", id);
            var response = await _labelDL.DeleteLabelsFromDb(id);
            if (response.Success)
            {
                _logger.LogInformation("Successfully deleted label with ID: {LabelId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete label with ID: {LabelId}", id);
            }
            return response;
        }
    }
}
