using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Collaborator> Collaborators { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<NoteLabel> NoteLabels { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Collaborator>().HasKey(c => new { c.UserId, c.NoteId });
            modelBuilder.Entity<Collaborator>().HasOne(c=>c.User).WithMany(u=>u.Collaborators).HasForeignKey(u=>u.UserId);
            modelBuilder.Entity<Collaborator>().HasOne(c => c.Note).WithMany(n => n.Collaborators).HasForeignKey(c => c.NoteId);
            modelBuilder.Entity<NoteLabel>().HasKey(c => new {c.NoteId,c.LabelId});
            modelBuilder.Entity<NoteLabel>().HasOne(c=>c.Note).WithMany(u=>u.NoteLabels).HasForeignKey(c => c.NoteId);
            modelBuilder.Entity<NoteLabel>().HasOne(c=>c.Label).WithMany(u=>u.NoteLabels).HasForeignKey(c=>c.LabelId);
        }



    }
}
