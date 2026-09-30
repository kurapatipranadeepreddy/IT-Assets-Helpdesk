using System.ComponentModel.DataAnnotations;

namespace ITAssetHelpdesk.Models;

public class AssetAssignment
{
    public int Id { get; set; }

    public int AssetId { get; set; }
    public int EmployeeId { get; set; }

    public DateTime AssignedDate { get; set; } = DateTime.Now;

    public DateTime? ReturnedDate { get; set; }

    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;

    // Navigation
    public Asset Asset { get; set; } = null!;
    public Employee Employee { get; set; } = null!;

    public bool IsActive => !ReturnedDate.HasValue;
}
