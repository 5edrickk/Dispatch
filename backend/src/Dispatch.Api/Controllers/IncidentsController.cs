using Dispatch.Api.Dtos;
using Dispatch.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dispatch.Api.Controllers;

[ApiController]
[Route("api/incidents")]
public class IncidentsController : ControllerBase
{
    private readonly IIncidentService _service;

    public IncidentsController(IIncidentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<IncidentDto>> Create(CreateIncidentRequest request)
    {
        var dto = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<IncidentDto>>> List([FromQuery] IncidentFilter filter)
    {
        var incidents = await _service.ListAsync(filter);
        return Ok(incidents);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IncidentDto>> Get(int id)
    {
        var dto = await _service.GetAsync(id);
        if (dto is null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<IncidentDto>> UpdateStatus(int id, UpdateStatusRequest request)
    {
        try
        {
            var dto = await _service.UpdateStatusAsync(id, request.Status);
            if (dto is null)
            {
                return NotFound();
            }
            return Ok(dto);
        }
        catch (InvalidStatusTransitionException ex)
        {
            return Problem(
                title: "Invalid status transition",
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict);
        }
    }
}