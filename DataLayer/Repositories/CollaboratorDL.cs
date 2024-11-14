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
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class CollaboratorDL:ICollaboratorDL
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CollaboratorDL> _logger;
        public CollaboratorDL(ApplicationDbContext context,ILogger<CollaboratorDL> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<UserDto>> AddCollaboratorToDb(int noteId, string email, int userId)
        {
            _logger.LogInformation("Received request to add collaborator to note with ID: {NoteId} by user ID: {UserId}", noteId, userId);

            var ownerExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!ownerExists)
            {
                _logger.LogWarning("User with ID: {UserId} not found", userId);
                return new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = $"User with ID: {userId} not found",
                    Data = null
                };
            }

            var note = await _context.Notes.FindAsync(noteId);
            if (note == null)
            {
                _logger.LogWarning("Note with ID: {NoteId} not found", noteId);
                return new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Note not found",
                    Data = null
                };
            }

            if (note.UserId != userId && !_context.Collaborators.Any(c => c.UserId == userId && c.NoteId == noteId))
            {
                _logger.LogWarning("User with ID: {UserId} does not have permission to add collaborator to note with ID: {NoteId}", userId, noteId);
                throw new UserException("You have no permission to add collaborator");
            }

            var collabUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (collabUser == null)
            {
                _logger.LogWarning("User with email: {Email} not found", email);
                return new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = $"User with email: {email} not found",
                    Data = null
                };
            }

            var isCollabUserPresent = await _context.Collaborators.AnyAsync(u => u.UserId == collabUser.Id && u.NoteId == noteId);
            if (isCollabUserPresent)
            {
                _logger.LogWarning("User with email: {Email} is already a collaborator for note with ID: {NoteId}", email, noteId);
                return new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "This user is already a collaborator for this note.",
                    Data = null
                };
            }

            var collaborator = new Collaborator { NoteId = noteId, UserId = collabUser.Id };
            _context.Collaborators.Add(collaborator);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully added user with email: {Email} as a collaborator to note with ID: {NoteId}", email, noteId);

            var newDto = new UserDto { Id = collabUser.Id, Name = collabUser.Name, Email = collabUser.Email };
            return new ApiResponse<UserDto>
            {
                Success = true,
                Message = "Added Collaborator Successfully",
                Data = newDto
            };
        }

        public async Task<ApiResponse<string>> DeleteCollaboratorInDb(int noteId, string email, int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                _logger.LogWarning("User with ID: {UserId} not found", userId);
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = $"User with ID: {userId} not found",
                    Data = null
                };
            }

            var note = await _context.Notes.FindAsync(noteId);
            if (note == null)
            {
                _logger.LogWarning("Note with ID: {NoteId} not found", noteId);
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Note not found",
                    Data = null
                };
            }

            if (note.UserId != userId && !_context.Collaborators.Any(c => c.UserId == userId && c.NoteId == noteId))
            {
                _logger.LogWarning("User with ID: {UserId} is not authorized to delete collaborator", userId);
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "You are not authorized to remove collaborators from this note",
                    Data = null
                };
            }

            var collaboratorToDelete = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (collaboratorToDelete == null)
            {
                _logger.LogWarning("User with email: {Email} not found", email);
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = $"User with email: {email} not found",
                    Data = null
                };
            }

            var collaboratorEntry = await _context.Collaborators
                .FirstOrDefaultAsync(c => c.NoteId == noteId && c.UserId == collaboratorToDelete.Id);

            if (collaboratorEntry == null)
            {
                _logger.LogWarning("This user is not a collaborator for the note");
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "This user is not a collaborator for this note",
                    Data = null
                };
            }
            if (note.UserId == userId || collaboratorToDelete.Id == userId)
            {
                _context.Collaborators.Remove(collaboratorEntry);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Collaborator with email: {Email} removed from note with ID: {NoteId}", email, noteId);
                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Collaborator removed successfully",
                    Data = null
                };
            }
            else
            {
                _logger.LogWarning("User with ID: {UserId} attempted to remove another collaborator", userId);
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "You cannot remove other collaborators",
                    Data = null
                };
            }
        }


        public async Task<ApiResponse<IEnumerable<UserDto>>> GetCollaborators(int NoteId,int ownerId)
        {
            bool ownerExists = await _context.Users.AnyAsync(u => u.Id == ownerId);
            if (!ownerExists)
            {
                _logger.LogWarning("User with email: {Id} not found", ownerId);
                return new ApiResponse<IEnumerable<UserDto>>
                {
                    Success = false,
                    Message = $"User with email: {ownerId} not found",
                    Data = null
                };
            }
            var note = await _context.Notes.FindAsync(NoteId);
            if (note == null)
            {
                _logger.LogWarning("Note with ID: {NoteId} not found", NoteId);
                return new ApiResponse<IEnumerable<UserDto>>
                {
                    Success = false,
                    Message = "Note not found",
                    Data = null
                };
            }
            if (note.UserId != ownerId && !_context.Collaborators.Any(c=>c.UserId==ownerId&& c.NoteId==NoteId))
            {
                throw new UserException("You do not have access to  get the Collaborators");
            }
            var collaborators=await  _context.Collaborators.Where(c=>c.NoteId==NoteId).Join(_context.Users,collaborator=>collaborator.UserId,user=>user.Id,(collaborator, user)=>new UserDto {Id=user.Id,Email=user.Email,Name=user.Name }).ToListAsync();
            return new ApiResponse<IEnumerable<UserDto>>
            {
                Success=true,
                Message="Retrieved Collaborators successfully",
                Data=collaborators
            };
        }
        }
}



