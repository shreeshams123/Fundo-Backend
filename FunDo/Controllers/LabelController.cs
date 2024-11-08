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
        [HttpPost]
        public async Task<IActionResult> AddLabels(LabelRequestDto requestDto)
        {
            return Ok(await _labelBL.AddLabelsToDbAsync(requestDto));
        }
    }
}
