using ITAssetHelpdesk.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAssetHelpdesk.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetAssignment> AssetAssignments => Set<AssetAssignment>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<AppUser>(e => e.HasIndex(u => u.Username).IsUnique());

        mb.Entity<Employee>(e =>
        {
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.EmployeeCode).IsUnique();
        });

        mb.Entity<Asset>(e =>
        {
            e.HasIndex(x => x.AssetTag).IsUnique();
            e.HasIndex(x => x.SerialNumber).IsUnique();
            e.HasOne(x => x.Employee)
             .WithMany(emp => emp.Assets)
             .HasForeignKey(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<AssetAssignment>(e =>
        {
            e.HasOne(x => x.Asset).WithMany(a => a.AssetAssignments).HasForeignKey(x => x.AssetId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Employee).WithMany(emp => emp.AssetAssignments).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<Ticket>(e =>
        {
            e.HasIndex(x => x.TicketNumber).IsUnique();
            e.HasOne(x => x.CreatedBy).WithMany(u => u.CreatedTickets).HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.AssignedTechnician).WithMany(u => u.AssignedTickets).HasForeignKey(x => x.AssignedTechnicianId).OnDelete(DeleteBehavior.SetNull);
        });
    }
}
