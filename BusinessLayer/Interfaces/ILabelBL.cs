using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ILabelBL
    {
        Task<LabelDto> AddLabelsToDbAsync(LabelRequestDto labelRequestDto);
    }
}
