using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FunDo.Controllers
{
    [ApiController]
    [Route("api/label")]
    public class LabelController : ControllerBase
    {
        private readonly ILabelBL _labelBL;
        private readonly ILogger<LabelController> _logger;

        public LabelController(ILabelBL labelBL, ILogger<LabelController> logger)
        {
            _labelBL = labelBL;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("/add-labels")]
        public async Task<IActionResult> AddLabels(LabelRequestDto requestDto)
        {
            _logger.LogInformation("Attempting to add labels");
            var apiresponse = await _labelBL.AddLabelsToDbAsync(requestDto);
            if (apiresponse.Success)
            {
                _logger.LogInformation("Labels added successfully");
                return Ok(apiresponse);
            }
            _logger.LogWarning("Failed to add labels: {Message}", apiresponse.Message);
            return BadRequest(apiresponse);
        }

        [Authorize]
        [HttpPost("/update-labels/{noteId}")]
        public async Task<IActionResult> UpdateLabels([FromRoute] int noteId, [FromBody] List<int> LabelIds)
        {
            _logger.LogInformation("Attempting to update labels for Note ID: {NoteId}", noteId);
            var apiresponse = await _labelBL.UpdateLabelToNotesAsync(noteId, LabelIds);
            if (apiresponse.Success)
            {
                _logger.LogInformation("Labels updated successfully for Note ID: {NoteId}", noteId);
                return Ok(apiresponse);
            }
            _logger.LogWarning("Failed to update labels for Note ID: {NoteId}: {Message}", noteId, apiresponse.Message);
            return BadRequest(apiresponse);
        }

        [Authorize]
        [HttpGet("get-labels-checklist/{noteId}")]
        public async Task<IActionResult> GetLabelsChecklist([FromRoute] int noteId)
        {
            _logger.LogInformation("Retrieving label checklist for Note ID: {NoteId}", noteId);
            var apiresponse = await _labelBL.GetLabelChecklistForNote(noteId);
            if (apiresponse.Success)
            {
                _logger.LogInformation("Label checklist retrieved successfully for Note ID: {NoteId}", noteId);
                return Ok(apiresponse);
            }
            _logger.LogWarning("Failed to retrieve label checklist for Note ID: {NoteId}: {Message}", noteId, apiresponse.Message);
            return BadRequest(apiresponse);
        }

        [Authorize]
        [HttpGet("get-all-labels")]
        public async Task<IActionResult> GetAllLabels()
        {
            _logger.LogInformation("Retrieving all labels");
            var apiresponse = await _labelBL.GetAllLabelsAsync();
            if (apiresponse.Success)
            {
                _logger.LogInformation("All labels retrieved successfully");
                return Ok(apiresponse);
            }
            _logger.LogWarning("Failed to retrieve all labels: {Message}", apiresponse.Message);
            return BadRequest(apiresponse);
        }

        [Authorize]
        [HttpDelete("delete-label/{labelId}")]
        public async Task<IActionResult> DeleteLabel([FromRoute] int labelId)
        {
            _logger.LogInformation("Attempting to delete label with ID: {LabelId}", labelId);
            var apiresponse = await _labelBL.DeleteLabelAsync(labelId);
            if (apiresponse.Success)
            {
                _logger.LogInformation("Label deleted successfully with ID: {LabelId}", labelId);
                return Ok(apiresponse);
            }
            _logger.LogWarning("Failed to delete label with ID: {LabelId}: {Message}", labelId, apiresponse.Message);
            return BadRequest(apiresponse);
        }
    }
}
