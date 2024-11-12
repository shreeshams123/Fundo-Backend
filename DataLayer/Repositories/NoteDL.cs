using DataLayer.Data;
using DataLayer.Exceptions;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class NoteDL : INoteDL
    {
        private readonly ApplicationDbContext _context;
        public NoteDL(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NoteResponseDto> CreateNoteInDbAsync(Note note, int userId)
        {
            try
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

                if (!userExists)
                {
                    throw new UserException($"User not found.");
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

                return newNote;
            }
            catch (DbUpdateException ex)
            {
                throw new NoteException("An error occurred while saving the note.", ex);
            }

        }

        public async Task<IEnumerable<NoteResponseDto>> GetAllNotesInDb(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                throw new UserException($"User not found.");
            }
            var notes= await _context.Notes.Where(note => note.UserId == userId|| _context.Collaborators.Any(c=>c.UserId==userId && c.NoteId==note.Id)).
                Select(note => new NoteResponseDto { Id = note.Id, Title = note.Title, Description = note.Description, Color = note.Color, IsArchive = note.IsArchive, IsTrash = note.IsTrash,IsCreated=note.UserId==userId }).ToListAsync();
            return notes;
        
        
        }  
         
        public async Task<NoteResponseDto> GetNoteByIdInDb(int NoteId,int UserId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == UserId);

            if (!userExists)
            {
                throw new UserException($"User not found.");
            }
            var note = await _context.Notes.FindAsync(NoteId);
            if (note == null)
            {
                throw new NoteException("Note doesn't exist");
            }
            if (UserId != note.UserId&& !_context.Collaborators.Any(c=>c.UserId==UserId && c.NoteId==NoteId))
            {
                throw new UserException("You don't have access to this notes");
            }
            var newnote= new NoteResponseDto {Id=note.Id,Title=note.Title,Description=note.Description, Color = note.Color, IsArchive = note.IsArchive, IsTrash = note.IsTrash ,IsCreated=note.UserId==UserId};
            return newnote;
        }
        
        public async Task<NoteResponseDto> UpdateNoteInDb(NoteUpdateDto noteUpdateDto, int NoteId, int UserId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == UserId);
            if (!userExists)
            {
                throw new UserException($"User not found.");
            }
            var note = await _context.Notes.FindAsync(NoteId);
            if (note == null)
            {
                throw new NoteException("Note doesn't exist");
            }
            if (UserId == note.UserId||_context.Collaborators.Any(c=>c.NoteId==NoteId&& c.UserId==UserId))
            {
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
                var newnote = new NoteResponseDto
                {
                    Id = note.Id,
                    Title = note.Title,
                    Description = note.Description,
                    Color = note.Color,
                    IsArchive = note.IsArchive,
                    IsTrash = note.IsTrash,
                    IsCreated = note.UserId == UserId
                };
                return newnote;
            }

            throw new UnauthorizedAccessException("You do not have permission to update this note.");
        }
        public async Task toggleArchive(int noteId,int userId,bool isArchive)
        {
            var note=await _context.Notes.FindAsync(noteId);
            if(note == null)
            {
                throw new NoteException("Note not found");
            }
            if (note.UserId != userId)
            {
                throw new UserException("You are not allowed to access this note");
            }
            note.IsArchive = isArchive;
            await _context.SaveChangesAsync();
        }
        public async Task toggleTrash(int noteId,int userId,bool isTrash)
        {
            var note = await _context.Notes.FindAsync(noteId);
            if (note == null)
            {
                throw new NoteException("Note not found");
            }
            if (note.UserId != userId)
            {
                throw new UserException("You are not allowed to access this note");
            }
            note.IsTrash = isTrash;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNoteInDb(int noteId, int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new UserException($"User not found.");
            }
            var note = await _context.Notes.FindAsync(noteId);
            if (note == null)
            {
                throw new NullReferenceException("Note doesn't exist");
            }
            if (note.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this note.");
            }
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

        }
    }
}
