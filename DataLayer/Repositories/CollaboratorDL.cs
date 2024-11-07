using DataLayer.Data;
using DataLayer.Exceptions;
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
    public class CollaboratorDL:ICollaboratorDL
    {
        private readonly ApplicationDbContext _context;
        public CollaboratorDL(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto> AddCollaboratorToDb(int NoteId, string Email, int userId)
        {
            var ownerExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!ownerExists)
            {
                throw new UserException($"User not found.");
            }
            var note = await _context.Notes.FindAsync(NoteId);
            if (note == null)
            {
                throw new NoteException("Note not found");
            }
            if (note.UserId != userId)
            {
                throw new UserException("You have no permission to add collaborator");
            }
            var collabUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (collabUser == null) 
            {
                throw new UserException("Collaborator User not found");
            }
            var isCollabUserPresent=await _context.Collaborators.AnyAsync(u=>u.UserId == collabUser.Id && u.NoteId==NoteId);
            if (isCollabUserPresent)
            {
                throw new UserException("This user is already a collaborator for this note.");
            }
            var collaborator = new Collaborator { NoteId = NoteId, UserId = collabUser.Id };
                    _context.Collaborators.Add(collaborator);
                    await _context.SaveChangesAsync();
                    return new UserDto { Id = collabUser.Id, Name = collabUser.Name, Email = collabUser.Email };
            }
        public async Task DeleteCollaboratorInDb(int NoteId,string Email,int ownerId)
        {
            bool ownerExists=await _context.Users.AnyAsync(u=>u.Id==ownerId);
            if (!ownerExists)
            {
                throw new UserException("User not found");
            }
            var note = await _context.Notes.FindAsync(NoteId);
            if (note == null)
            {
                throw new NoteException("Note not found");
            }
            if (note.UserId !=ownerId)
            {
                throw new UserException("You have no permission to delete collaborator");
            }
            var User = await _context.Users.FirstOrDefaultAsync(u=>u.Email==Email);
            if (User == null)
            {
                throw new UserException("Collaborator user not found");
            }
            var collaborator=await _context.Collaborators.FirstOrDefaultAsync(c=>c.NoteId==NoteId && c.UserId==User.Id);
            if (collaborator == null)
            {
                throw new UserException("User is not a collaborator for this note");
            }
            _context.Collaborators.Remove(collaborator);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<UserDto>> GetCollaborators(int NoteId,int ownerId)
        {
            bool ownerExists = await _context.Users.AnyAsync(u => u.Id == ownerId);
            if (!ownerExists)
            {
                throw new UserException("User not found");
            }
            var note = await _context.Notes.FindAsync(NoteId);
            if (note == null)
            {
                throw new NoteException("Note not found");
            }
            if (note.UserId != ownerId)
            {
                throw new UserException("You do not have access to this get the Collaborators");
            }
            var collaborators=await  _context.Collaborators.Where(c=>c.NoteId==NoteId).Join(_context.Users,collaborator=>collaborator.UserId,user=>user.Id,(collaborator, user)=>new UserDto {Id=user.Id,Email=user.Email,Name=user.Name }).ToListAsync();
            return collaborators;
        }
        }
}



