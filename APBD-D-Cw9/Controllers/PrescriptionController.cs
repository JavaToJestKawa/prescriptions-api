using APBD_D_Cw9.DTOs;
using APBD_D_Cw9.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_D_Cw9.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrescriptionController : ControllerBase
{
    private readonly IPrescriptionService _service;

    public PrescriptionController(IPrescriptionService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] AddPrescriptionRequest request)
    {
        try
        {
            await _service.AddPrescriptionAsync(request);
            return Ok("Prescription added.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("patient/{idPatient}")]
    public async Task<IActionResult> GetPatientDetails(int idPatient)
    {
        var result = await _service.GetPatientDetailsAsync(idPatient);
        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
