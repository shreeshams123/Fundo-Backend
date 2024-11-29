using DataLayer.Data;
using DataLayer.Exceptions;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class NoteDL : INoteDL
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NoteDL> _logger;
        private readonly IDistributedCache _distributedCache;
        public NoteDL(ApplicationDbContext context,ILogger<NoteDL> logger,IDistributedCache distributedCache)
        {
            _context = context;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<ApiResponse<NoteResponseDto>> CreateNoteInDbAsync(Note note, int userId)
        {
            try
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

                if (!userExists)
                {
                    _logger.LogWarning("User with ID: {UserId} not found", userId);
                    var notfoundresponse = new ApiResponse<NoteResponseDto>
                    {
                        Success = false,
                        Message = $"User with ID: {userId} not found",
                        Data = null
                    };
                    return notfoundresponse;
                }
                await _context.Notes.AddAsync(note);
                await _context.SaveChangesAsync();
                var newNote = new NoteResponseDto
                {
                    Id = note.Id,
                    Title = note.Title,
                    Description = note.Description,
                    Color = note.Color,
                    IsArchive = note.IsArchive,
                    IsTrash = note.IsTrash,
                    IsCreated=true
                };
                var response = new ApiResponse<NoteResponseDto>
                {
                    Success = true,
                    Message = "Note created successfully",
                    Data = newNote
                };
                await _distributedCache.RemoveAsync($"user:{userId}notes");
                return response;
            }
            catch (DbUpdateException ex)
            {
                var response = new ApiResponse<NoteResponseDto>
                {
                    Success = false,
                    Message = ex.Message,
                    Data=null
                };
                throw new NoteException("An error occurred while saving the note.", ex);
            }

        }

        public async Task<ApiResponse<IEnumerable<NoteResponseDto>>> GetAllNotesInDb(int userId)
        {
            _logger.LogInformation($"Checking if user with ID: {userId} exists");
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                _logger.LogWarning($"User with ID: {userId} not found");
                var notfoundresponse = new ApiResponse<IEnumerable<NoteResponseDto>>
                {
                    Success = false,
                    Message = $"User with ID: {userId} not found",
                    Data = null 
                };
                return notfoundresponse;
            }

            _logger.LogInformation("Fetching notes for user with ID: {UserId}", userId);
            var notes = await _context.Notes
                .Where(note => note.UserId == userId || _context.Collaborators.Any(c => c.UserId == userId && c.NoteId == note.Id))
                .Select(note => new NoteResponseDto
                {
                    Id = note.Id,
                    Title = note.Title,
                    Description = note.Description,
                    Color = note.Color,
                    IsArchive = note.IsArchive,
                    IsTrash = note.IsTrash,
                    IsCreated = note.UserId == userId
                }).ToListAsync();

            _logger.LogInformation("{Count} notes retrieved for user with ID: {UserId}", notes.Count, userId);

            var response = new ApiResponse<IEnumerable<NoteResponseDto>>
            {
                Success = true,
                Message = "Notes retrieved successfully",
                Data = notes
            };
            _logger.LogInformation("Serializing notes to store it in cache");
            var serializednotes = JsonSerializer.Serialize(notes);
            await _distributedCache.SetStringAsync($"user:{userId}notes", serializednotes, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return response;
        }

        public async Task<ApiResponse<NoteResponseDto>> GetNoteByIdInDb(int noteId, int userId)
        {
            _logger.LogInformation("Checking if user with ID: {UserId} exists", userId);
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                _logger.LogWarning("User with ID: {UserId} not found", userId);
                return new ApiResponse<NoteResponseDto>
                {
                    Success = false,
                    Message = $"User with ID: {userId} not found",
                    Data = null
                };
            }

            _logger.LogInformation("Retrieving note with ID: {NoteId}", noteId);
            var note = await _context.Notes.FindAsync(noteId);
            if (note == null)
            {
                _logger.LogWarning("Note with ID: {NoteId} not found", noteId);
                return new ApiResponse<NoteResponseDto>
                {
                    Success = false,
                    Message = "Note not found",
                    Data = null
                };
            }

            if (userId != note.UserId && !_context.Collaborators.Any(c => c.UserId == userId && c.NoteId == noteId))
            {
                _logger.LogWarning("User with ID: {UserId} does not have access to note ID: {NoteId}", userId, noteId);
                throw new UserException("You don't have access to this note");
            }

            var newNote = new NoteResponseDto
            {
                Id = note.Id,
                Title = note.Title,
                Description = note.Description,
                Color = note.Color,
                IsArchive = note.IsArchive,
                IsTrash = note.IsTrash,
                IsCreated = note.UserId == userId
            };

            _logger.LogInformation("Note with ID: {NoteId} retrieved successfully", noteId);

            return new ApiResponse<NoteResponseDto>
            {
                Success = true,
                Message = "Note retrieved successfully",
                Data = newNote
            };
        }

        public async Task<ApiResponse<NoteResponseDto>> UpdateNoteInDb(NoteUpdateDto noteUpdateDto, int noteId, int userId)
        {
            _logger.LogInformation("Checking if user with ID: {UserId} exists", userId);
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                _logger.LogWarning("User with ID: {UserId} not found", userId);
                return new ApiResponse<NoteResponseDto>
                {
                    Success = false,
                    Message = $"User with ID: {userId} not found",
                    Data = null
                };
            }

            _logger.LogInformation("Retrieving note with ID: {NoteId}", noteId);
            var note = await _context.Notes.FindAsync(noteId);

            if (note == null)
            {
                _logger.LogWarning("Note with ID: {NoteId} not found", noteId);
                return new ApiResponse<NoteResponseDto>
                {
                    Success = false,
                    Message = "Note not found",
                    Data = null
                };
            }

            if (userId == note.UserId || _context.Collaborators.Any(c => c.NoteId == noteId && c.UserId == userId))
            {
                _logger.LogInformation("User with ID: {UserId} has permission to update note ID: {NoteId}", userId, noteId);

                if (!string.IsNullOrWhiteSpace(noteUpdateDto.Title))
                {
                    note.Title = noteUpdateDto.Title;
                }

                if (!string.IsNullOrWhiteSpace(noteUpdateDto.Description))
                {
                    note.Description = noteUpdateDto.Description;
                }

                if (!string.IsNullOrWhiteSpace(noteUpdateDto.Color))
                {
                    note.Color = noteUpdateDto.Color;
                }

                await _context.SaveChangesAsync();

                var updatedNote = new NoteResponseDto
                {
                    Id = note.Id,
                    Title = note.Title,
                    Description = note.Description,
                    Color = note.Color,
                    IsArchive = note.IsArchive,
                    IsTrash = note.IsTrash,
                    IsCreated = note.UserId == userId
                };

                _logger.LogInformation("Successfully updated note with ID: {NoteId}", noteId);
                await _distributedCache.RemoveAsync($"user:{userId}notes");
                return new ApiResponse<NoteResponseDto>
                {
                    Success = true,
                    Message = "Updated note successfully",
                    Data = updatedNote
                };
            }

            _logger.LogWarning("User with ID: {UserId} does not have permission to update note ID: {NoteId}", userId, noteId);
            throw new UserException("You do not have permission to update this note.");
        }
        public async Task<ApiResponse<string>> ToggleArchive(int noteId,int userId,bool isArchive)
        {
            _logger.LogInformation($"Received isArchive value: {isArchive}");

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
            var note=await _context.Notes.FindAsync(noteId);
            if(note == null)
            {
                _logger.LogWarning("Note with ID: {NoteId} not found", noteId);
                return new ApiResponse<String>
                {
                    Success = false,
                    Message = "Note not found",
                    Data = null
                };
            }
            if (note.UserId != userId)
            {
                _logger.LogWarning("User unauthorized");
                throw new UserException("You are not allowed to access this note");
            }
            note.IsArchive = isArchive;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Note with ID: {NoteId} archive status updated to {IsArchive}", noteId, isArchive);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Note archive status updated successfully",
                Data = isArchive ? "Archived" : "Unarchived"
            };
        }
        public async Task<ApiResponse<string>> ToggleTrash(int noteId,int userId,bool isTrash)
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
                return new ApiResponse<String>
                {
                    Success = false,
                    Message = "Note not found",
                    Data = null
                };
            }
            if (note.UserId != userId)
            {
                _logger.LogWarning("User unauthorized");
                throw new UserException("You are not allowed to access this note");
            }
            note.IsTrash = isTrash;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Note with ID: {NoteId} trash status updated to {isTrash}", noteId, isTrash);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Note trash status updated successfully",
                Data = isTrash ? "Trashed" : "UnTrashed"
            };
        }

        public async Task<ApiResponse<string>> DeleteNoteInDb(int noteId, int userId)
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
                return new ApiResponse<String>
                {
                    Success = false,
                    Message = "Note not found",
                    Data = null
                };
            }
            if (note.UserId != userId)
            {
                _logger.LogWarning("User unauthorized");
                throw new UnauthorizedAccessException("You do not have permission to delete this note.");
            }
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted note with {noteId} successfully", noteId);
            await _distributedCache.RemoveAsync($"user:{userId}notes");
            return new ApiResponse<string> {Success=true,Message="Deleted note Successfully",Data=null };
        }
    }
}
