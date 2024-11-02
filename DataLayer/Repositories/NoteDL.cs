using DataLayer.Data;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<NoteResponseDto> CreateNoteInDbAsync(Note note)
        {
            await _context.Notes.AddAsync(note);
            await _context.SaveChangesAsync();
            var newnote = new NoteResponseDto
            {
                Id = note.Id,
                Title = note.Title,
                Description = note.Description,
                Color = note.Color,
                IsArchive = note.IsArchive,
                IsTrash = note.IsTrash,
            };
            return newnote;
        }

        public async Task<IEnumerable<NoteResponseDto>> GetNoteByIdAsync(int userId)
        {
            return await _context.Notes.Where(note => note.UserId == userId).
                Select(note => new NoteResponseDto { Id = note.Id, Title = note.Title, Description = note.Description, Color = note.Color, IsArchive = note.IsArchive, IsTrash = note.IsTrash }).ToListAsync();
        }
        public async Task<NoteResponseDto> UpdateNoteInDb(NoteUpdateDto noteUpdateDto, int NoteId, int UserId)
        {
            var note = await _context.Notes.FindAsync(NoteId);
            if (UserId == note.UserId)
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
                if (noteUpdateDto.IsArchive.HasValue)
                {
                    note.IsArchive = noteUpdateDto.IsArchive.Value;
                }
                if (noteUpdateDto.IsTrash.HasValue)
                {
                    note.IsTrash = noteUpdateDto.IsTrash.Value;
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
                };
                return newnote;
            }

            throw new UnauthorizedAccessException("You do not have permission to update this note.");
        }
        public async Task DeleteNoteInDb(int noteId, int userId)
        {
            var note = await _context.Notes.FindAsync(noteId);
            if (note.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to modify this note.");
            }
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

        }
    }
}
