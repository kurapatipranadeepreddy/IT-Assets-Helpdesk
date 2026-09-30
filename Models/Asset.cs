using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITAssetHelpdesk.Models;

public class Asset
{
    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string AssetTag { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Brand { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;

    [MaxLength(100)]
    public string SerialNumber { get; set; } = string.Empty;

    public DateTime PurchaseDate { get; set; } = DateTime.Today;

    public DateTime? WarrantyExpiryDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Cost { get; set; }

    [Required, MaxLength(30)]
    public string Status { get; set; } = "Available";

    public int? EmployeeId { get; set; }

    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;

    // Navigation
    public Employee? Employee { get; set; }
    public ICollection<AssetAssignment> AssetAssignments { get; set; } = new List<AssetAssignment>();

    public bool IsWarrantyExpiringSoon =>
        WarrantyExpiryDate.HasValue &&
        WarrantyExpiryDate.Value > DateTime.Today &&
        WarrantyExpiryDate.Value <= DateTime.Today.AddDays(30);

    public bool IsWarrantyExpired =>
        WarrantyExpiryDate.HasValue && WarrantyExpiryDate.Value < DateTime.Today;

    public static readonly string[] Categories =
        { "Laptop", "Desktop", "Monitor", "Printer", "Mobile Phone", "Tablet", "Network Equipment", "Other" };

    public static readonly string[] Statuses =
        { "Available", "Assigned", "Under Repair", "Retired" };
}
