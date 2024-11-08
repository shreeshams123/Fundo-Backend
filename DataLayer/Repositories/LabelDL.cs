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
    public class LabelDL : ILabelDL
    {
        private readonly ApplicationDbContext _context;
        public LabelDL(ApplicationDbContext context)
        {
            _context = context;   
        }
        public async Task<LabelDto> AddLabelsToDb(Label label)
        {
            var labelExists=await _context.Labels.AnyAsync(u=>u.Name == label.Name);
            if (labelExists)
            {
                throw new LabelException("Label already exists");
            }
            await _context.Labels.AddAsync(label);
            await _context.SaveChangesAsync();
            var newLabel = new LabelDto {Id=label.Id,Name=label.Name };
            return newLabel;
        }
    }
}
