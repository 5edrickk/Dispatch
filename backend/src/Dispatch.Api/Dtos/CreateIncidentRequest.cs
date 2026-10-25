using System.ComponentModel.DataAnnotations;
using Dispatch.Api.Domain;

namespace Dispatch.Api.Dtos;

public class CreateIncidentRequest
{
    [Required]
    public IncidentType Type { get; set; }

    [Required]
    public IncidentPriority Priority { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [StringLength(1000, MinimumLength = 3)]
    public string Description { get; set; } = string.Empty;
}