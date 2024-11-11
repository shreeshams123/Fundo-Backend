using DataLayer.Data;
using DataLayer.Exceptions;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class LabelDL : ILabelDL
    {
        private readonly ApplicationDbContext _context;
        public LabelDL(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<LabelDto> AddLabelsToDb(Label label)
        {
            var labelExists = await _context.Labels.AnyAsync(u => u.Name == label.Name);
            if (labelExists)
            {
                throw new LabelException("Label already exists");
            }
            await _context.Labels.AddAsync(label);
            await _context.SaveChangesAsync();
            var newLabel = new LabelDto { Id = label.Id, Name = label.Name };
            return newLabel;
        }
        public async Task UpdateNoteLabel(int noteId, int userId, List<int> LabelIds)
        {
            if (LabelIds == null)
            {
                throw new ArgumentNullException(nameof(LabelIds), "LabelIds cannot be null.");
            }

            var note = await _context.Notes.Include(n => n.Collaborators).Include(n => n.NoteLabels).FirstOrDefaultAsync(n => n.Id == noteId);
            if (note == null)
            {
                throw new NoteException("Note not found");
            }
            if (userId != note.UserId && !_context.Collaborators.Any(c => c.NoteId == noteId && c.UserId == userId))
            {
                throw new UserException("You do not have access to this note");
            }
            var labelsAssociated = await _context.Labels
                .Where(label => LabelIds.Contains(label.Id))
                .ToListAsync();

            var existingLabels = note.NoteLabels?.Select(n => n.LabelId).ToList() ?? new List<int>();
            var removeLabels = note.NoteLabels?.Where(label => !LabelIds.Contains(label.LabelId)).ToList() ?? new List<NoteLabel>();
            var labelsToAdd = labelsAssociated.Where(label => !existingLabels.Contains(label.Id)).ToList();

            foreach (var label in removeLabels)
            {
                note.NoteLabels.Remove(label);
            }

            foreach (var label in labelsToAdd)
            {
                var noteLabel = new NoteLabel
                {
                    NoteId = note.Id,
                    LabelId = label.Id
                };
                note.NoteLabels.Add(noteLabel);
            }

            await _context.SaveChangesAsync();
        }
        public async Task<List<Label>> GetAllLabelsFromDb()
        {
            return await _context.Labels.ToListAsync();
        }
        public async Task<List<int>> GetLabelsForNoteFromDb(int noteId,int userId)
        {
            var note = await _context.Notes.Include(n => n.Collaborators).Include(n => n.NoteLabels).FirstOrDefaultAsync(n => n.Id == noteId);
            if (note == null)
            {
                throw new NoteException("Note not found");
            }
            if (userId == note.UserId || _context.Collaborators.Any(c => c.NoteId == noteId && c.UserId == userId))
            {
                return await _context.NoteLabels.Where(nl => nl.NoteId == noteId).Select(nl => nl.LabelId).ToListAsync();
                
            }

            throw new UserException("You do not have access to this note");
        }
        public async Task DeleteLabelsFromDb(int Id)
        {
            var label = await _context.Labels.FindAsync(Id);
            if (label == null)
            {
                throw new ArgumentNullException("Label not found");
            }
            else
            {
                 _context.Labels.Remove(label);
                await _context.SaveChangesAsync();
            }
        }
    }

    }