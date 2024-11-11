using BusinessLayer.Interfaces;
using BusinessLayer.Utilities;
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
        private readonly TokenHelper _tokenHelper;
        public LabelBL(ILabelDL labelDL, TokenHelper tokenHelper) { 
            _labelDL = labelDL;
            _tokenHelper = tokenHelper;
        }
        public async Task<LabelDto> AddLabelsToDbAsync(LabelRequestDto labelRequestDto)
        {
            var newDto = new Label {Name=labelRequestDto.Name };
            return await _labelDL.AddLabelsToDb(newDto);
        }
        public async Task UpdateLabelToNotesAsync(int noteId,List<int> labelIds)
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            await _labelDL.UpdateNoteLabel(noteId,userId,labelIds);
        }
        public async Task<List<LabelDto>> GetAllLabelsAsync()
        {
            var userId = _tokenHelper.GetUserIdFromToken();
            var allLabels = await _labelDL.GetAllLabelsFromDb();
            return allLabels.Select(label => new LabelDto
            {
                Id = label.Id,
                Name = label.Name,
            }).ToList();
        }
        
        public async Task<List<LabelCheckListDto>> GetLabelChecklistForNote(int noteId) { 
            var userId= _tokenHelper.GetUserIdFromToken();
            var allLabels=await _labelDL.GetAllLabelsFromDb();
            var noteLabels= await _labelDL.GetLabelsForNoteFromDb(noteId,userId);
            return allLabels.Select(label=>new LabelCheckListDto
            {
                LabelId=label.Id,
                LabelName=label.Name,
                IsChecked=noteLabels.Contains(label.Id)
            }).ToList();
        }
        public async Task DeleteLabelAsync(int Id)
        {
             await _labelDL.DeleteLabelsFromDb(Id);
        }
    }
}
