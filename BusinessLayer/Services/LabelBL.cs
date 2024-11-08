using BusinessLayer.Interfaces;
using DataLayer.Interfaces;
using Models.DTOs;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class LabelBL:ILabelBL
    {
        private readonly ILabelDL _labelDL;
        public LabelBL(ILabelDL labelDL) { 
            _labelDL = labelDL;
        }
        public async Task<LabelDto> AddLabelsToDbAsync(LabelRequestDto labelRequestDto)
        {
            var newDto = new Label {Name=labelRequestDto.Name };
            return await _labelDL.AddLabelsToDb(newDto);
        }
    }
}
