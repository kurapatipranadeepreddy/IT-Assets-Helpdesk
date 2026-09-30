using System.ComponentModel.DataAnnotations;

namespace ITAssetHelpdesk.Models;

public class Ticket
{
    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string TicketNumber { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Priority { get; set; } = "Medium";

    [Required, MaxLength(30)]
    public string Status { get; set; } = "Open";

    public int CreatedById { get; set; }

    public int? AssignedTechnicianId { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    public DateTime? ResolvedDate { get; set; }

    [MaxLength(2000)]
    public string ResolutionNotes { get; set; } = string.Empty;

    // Navigation
    public AppUser CreatedBy { get; set; } = null!;
    public AppUser? AssignedTechnician { get; set; }

    public bool IsOpen => Status != "Resolved" && Status != "Closed";
    public bool IsHighPriority => Priority == "High" || Priority == "Critical";

    public static readonly string[] Categories =
        { "Hardware", "Software", "Network", "Account Access", "Printer", "Email", "Security", "Other" };

    public static readonly string[] Priorities = { "Low", "Medium", "High", "Critical" };

    public static readonly string[] Statuses =
        { "Open", "In Progress", "Waiting for User", "Resolved", "Closed" };
}
