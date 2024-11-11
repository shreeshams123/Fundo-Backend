using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;

namespace FunDo.Controllers
{
    [ApiController]
    [Route("api/label")]
    public class LabelController:ControllerBase
    {
        private readonly ILabelBL _labelBL;
        public LabelController(ILabelBL labelBL)
        {
            _labelBL = labelBL;
        }
        [Authorize]
        [HttpPost("/add-labels")]
        public async Task<IActionResult> AddLabels(LabelRequestDto requestDto)
        {
            var labelDto = await _labelBL.AddLabelsToDbAsync(requestDto);
            return Ok(labelDto);
        }
        [Authorize]
        [HttpPost("/update-labels/{noteId}")]
        public async Task<IActionResult> UpdateLabels([FromRoute]int noteId, [FromBody] List<int> LabelIds)
        {
            await _labelBL.UpdateLabelToNotesAsync(noteId,LabelIds);
            return Ok();
        }
        [Authorize]
        [HttpGet("get-labels-checklist/{noteId}")]
        public async Task<IActionResult> getLabelsChecklist([FromRoute] int noteId)
        {
            try
            {
                var labelCheckList = await _labelBL.GetLabelChecklistForNote(noteId);
                return Ok(labelCheckList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("get-all-labels")]
        public async Task<IActionResult> getAllLabels()
        {
            return Ok(await _labelBL.GetAllLabelsAsync());
        }
        [Authorize]
        [HttpDelete("delete-label{labelId}")]
        public async Task<IActionResult> DeleteLabel([FromRoute]int labelId){
            await _labelBL.DeleteLabelAsync(labelId);
            return Ok();
        }

    }
}
