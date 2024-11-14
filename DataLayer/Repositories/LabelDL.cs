using DataLayer.Data;
using DataLayer.Exceptions;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class LabelDL : ILabelDL
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LabelDL> _logger;

        public LabelDL(ApplicationDbContext context, ILogger<LabelDL> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<LabelDto>> AddLabelsToDb(LabelRequestDto labeldto)
        {
            _logger.LogInformation("Attempting to add label with name: {LabelName}", labeldto.Name);

            var labelExists = await _context.Labels.AnyAsync(u => u.Name == labeldto.Name);
            if (labelExists)
            {
                _logger.LogWarning("Label with name {LabelName} already exists", labeldto.Name);
                return new ApiResponse<LabelDto>
                {
                    Success = false,
                    Message = "Label already exists",
                    Data = null
                };
            }

            var label = new Label { Name = labeldto.Name };
            await _context.Labels.AddAsync(label);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully added label with ID: {LabelId}", label.Id);

            return new ApiResponse<LabelDto>
            {
                Success = true,
                Message = "Added label to Database",
                Data = new LabelDto { Id = label.Id, Name = label.Name }
            };
        }

        public async Task<ApiResponse<string>> UpdateNoteLabel(int noteId, int userId, List<int> LabelIds)
        {
            _logger.LogInformation("Updating labels for Note ID: {NoteId} by User ID: {UserId}", noteId, userId);

            if (LabelIds == null)
            {
                _logger.LogError("LabelIds parameter is null for Note ID: {NoteId}", noteId);
                throw new ArgumentNullException(nameof(LabelIds), "LabelIds cannot be null.");
            }

            var note = await _context.Notes.Include(n => n.Collaborators).Include(n => n.NoteLabels).FirstOrDefaultAsync(n => n.Id == noteId);
            if (note == null)
            {
                _logger.LogWarning("Note with ID: {NoteId} not found", noteId);
                return new ApiResponse<string> { Success = false, Message = "Note not found", Data = null };
            }

            if (userId != note.UserId && !_context.Collaborators.Any(c => c.NoteId == noteId && c.UserId == userId))
            {
                _logger.LogError("User ID: {UserId} does not have access to Note ID: {NoteId}", userId, noteId);
                throw new UserException("You do not have access to this note");
            }

            var labelsAssociated = await _context.Labels.Where(label => LabelIds.Contains(label.Id)).ToListAsync();
            var existingLabels = note.NoteLabels?.Select(n => n.LabelId).ToList() ?? new List<int>();
            var removeLabels = note.NoteLabels?.Where(label => !LabelIds.Contains(label.LabelId)).ToList() ?? new List<NoteLabel>();
            var labelsToAdd = labelsAssociated.Where(label => !existingLabels.Contains(label.Id)).ToList();

            foreach (var label in removeLabels)
            {
                note.NoteLabels.Remove(label);
                _logger.LogInformation("Removed label with ID: {LabelId} from Note ID: {NoteId}", label.LabelId, noteId);
            }

            foreach (var label in labelsToAdd)
            {
                var noteLabel = new NoteLabel { NoteId = note.Id, LabelId = label.Id };
                note.NoteLabels.Add(noteLabel);
                _logger.LogInformation("Added label with ID: {LabelId} to Note ID: {NoteId}", label.Id, noteId);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated labels for Note ID: {NoteId}", noteId);

            return new ApiResponse<string> { Success = true, Message = "Updated labels successfully", Data = null };
        }

        public async Task<ApiResponse<IEnumerable<LabelDto>>> GetAllLabelsFromDb()
        {
            _logger.LogInformation("Retrieving all labels from the database.");

            var allLabels = await _context.Labels.Select(label => new LabelDto { Id = label.Id, Name = label.Name }).ToListAsync();

            _logger.LogInformation("Successfully retrieved all labels from the database.");

            return new ApiResponse<IEnumerable<LabelDto>>
            {
                Success = true,
                Message = "Retrieved all labels successfully",
                Data = allLabels
            };
        }

        public async Task<ApiResponse<IEnumerable<int>>> GetLabelsForNoteFromDb(int noteId, int userId)
        {
            _logger.LogInformation("Retrieving labels for Note ID: {NoteId} by User ID: {UserId}", noteId, userId);

            var note = await _context.Notes.Include(n => n.Collaborators).Include(n => n.NoteLabels)
                                           .FirstOrDefaultAsync(n => n.Id == noteId);
            if (note == null)
            {
                _logger.LogWarning("Note with ID: {NoteId} not found", noteId);
                return new ApiResponse<IEnumerable<int>> { Success = false, Message = "Note not found", Data = null };
            }

            if (userId == note.UserId || _context.Collaborators.Any(c => c.NoteId == noteId && c.UserId == userId))
            {
                var labels = await _context.NoteLabels.Where(nl => nl.NoteId == noteId).Select(nl => nl.LabelId).ToListAsync();
                _logger.LogInformation("Successfully retrieved labels for Note ID: {NoteId}", noteId);

                return new ApiResponse<IEnumerable<int>> { Success = true, Message = "Retrieved labels successfully", Data = labels };
            }

            _logger.LogError("User ID: {UserId} does not have access to Note ID: {NoteId}", userId, noteId);
            throw new UserException("You do not have access to this note");
        }

        public async Task<ApiResponse<string>> DeleteLabelsFromDb(int Id)
        {
            _logger.LogInformation("Attempting to delete label with ID: {LabelId}", Id);

            var label = await _context.Labels.FindAsync(Id);
            if (label == null)
            {
                _logger.LogWarning("Label with ID: {LabelId} not found", Id);
                return new ApiResponse<string> { Success = false, Message = "Label not found", Data = null };
            }

            _context.Labels.Remove(label);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted label with ID: {LabelId}", Id);

            return new ApiResponse<string> { Success = true, Message = "Deleted label successfully", Data = null };
        }
    }
}
