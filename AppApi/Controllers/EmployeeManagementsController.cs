using AppApi.IService;
using AppApi.ViewModels.EmployeeManagementDTOs;
using AppApi.Features.Auth.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeManagementsController : ControllerBase
    {
        private readonly IEmployeeManagementService _service;

        public EmployeeManagementsController(IEmployeeManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 10,
     [FromQuery] string? fullName = null,
     [FromQuery] string? email = null,
     [FromQuery] string? phoneNumber = null,
     [FromQuery] bool? isActive = null,
     [FromQuery] bool? gender = null)
        {
            var result = await _service.GetAllAsync(page, pageSize, fullName, email, phoneNumber, isActive, gender);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound("Không tìm thấy người dùng.");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AddEmployeeDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateEmployeeDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var result = await _service.AssignRoleAsync(dto);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var result = await _service.ToggleActiveStatusAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] Features.Auth.DTOs.ResetPasswordDto dto)
        {
            var result = await _service.ResetPasswordAsync(dto);
            return StatusCode(result.StatusCode, result);
        }
    }
}
