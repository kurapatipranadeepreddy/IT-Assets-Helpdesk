using System.ComponentModel.DataAnnotations;

namespace ITAssetHelpdesk.Models;

public class Employee
{
    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(200), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Department { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string JobTitle { get; set; } = string.Empty;

    public DateTime DateJoined { get; set; } = DateTime.Today;

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    public ICollection<AssetAssignment> AssetAssignments { get; set; } = new List<AssetAssignment>();

    public string StatusBadge => IsActive ? "Active" : "Inactive";
}
