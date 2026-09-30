using ITAssetHelpdesk.Data;
using ITAssetHelpdesk.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAssetHelpdesk.Services;

public class AssetService(AppDbContext db)
{
    public async Task<List<Asset>> GetAllAsync(string? search = null, string? category = null, string? status = null)
    {
        var q = db.Assets.Include(a => a.Employee).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            q = q.Where(a => a.AssetTag.ToLower().Contains(s) || a.Name.ToLower().Contains(s) ||
                a.SerialNumber.ToLower().Contains(s) || (a.Employee != null && a.Employee.FullName.ToLower().Contains(s)));
        }
        if (!string.IsNullOrWhiteSpace(category) && category != "All") q = q.Where(a => a.Category == category);
        if (!string.IsNullOrWhiteSpace(status)   && status   != "All") q = q.Where(a => a.Status   == status);
        return await q.OrderBy(a => a.AssetTag).ToListAsync();
    }

    public async Task<Asset?> GetByIdAsync(int id) =>
        await db.Assets.Include(a => a.Employee).Include(a => a.AssetAssignments).ThenInclude(aa => aa.Employee)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<string> NextTagAsync()
    {
        var count = await db.Assets.CountAsync();
        var tag = $"AST-{count + 1:D4}";
        while (await db.Assets.AnyAsync(a => a.AssetTag == tag)) { count++; tag = $"AST-{count + 1:D4}"; }
        return tag;
    }

    public async Task<(bool ok, string msg)> AddAsync(Asset a)
    {
        if (await db.Assets.AnyAsync(x => x.AssetTag == a.AssetTag)) return (false, "Asset tag already exists.");
        if (!string.IsNullOrWhiteSpace(a.SerialNumber) && await db.Assets.AnyAsync(x => x.SerialNumber == a.SerialNumber)) return (false, "Serial number already exists.");
        db.Assets.Add(a);
        await db.SaveChangesAsync();
        return (true, "Asset added.");
    }

    public async Task<(bool ok, string msg)> UpdateAsync(Asset a)
    {
        if (await db.Assets.AnyAsync(x => x.AssetTag == a.AssetTag && x.Id != a.Id)) return (false, "Asset tag already exists.");
        if (!string.IsNullOrWhiteSpace(a.SerialNumber) && await db.Assets.AnyAsync(x => x.SerialNumber == a.SerialNumber && x.Id != a.Id)) return (false, "Serial number already exists.");
        db.Assets.Update(a);
        await db.SaveChangesAsync();
        return (true, "Asset updated.");
    }

    public async Task<(bool ok, string msg)> DeleteAsync(int id)
    {
        var a = await db.Assets.FindAsync(id);
        if (a == null) return (false, "Not found.");
        if (a.Status == "Assigned") return (false, "Cannot delete an assigned asset.");
        db.Assets.Remove(a);
        await db.SaveChangesAsync();
        return (true, "Asset deleted.");
    }

    public async Task<(bool ok, string msg)> AssignAsync(int assetId, int employeeId, string notes = "")
    {
        var asset = await db.Assets.FindAsync(assetId);
        if (asset == null) return (false, "Asset not found.");
        if (asset.Status == "Assigned") return (false, "Asset is already assigned.");
        if (asset.Status == "Retired")  return (false, "Cannot assign a retired asset.");
        var emp = await db.Employees.FindAsync(employeeId);
        if (emp == null || !emp.IsActive) return (false, "Employee not found or inactive.");
        asset.Status = "Assigned"; asset.EmployeeId = employeeId;
        db.AssetAssignments.Add(new() { AssetId=assetId, EmployeeId=employeeId, AssignedDate=DateTime.Now, Notes=notes });
        await db.SaveChangesAsync();
        return (true, $"Asset assigned to {emp.FullName}.");
    }

    public async Task<(bool ok, string msg)> UnassignAsync(int assetId)
    {
        var asset = await db.Assets.FindAsync(assetId);
        if (asset == null) return (false, "Asset not found.");
        if (asset.Status != "Assigned") return (false, "Asset is not assigned.");
        var active = await db.AssetAssignments.Where(aa => aa.AssetId == assetId && !aa.ReturnedDate.HasValue).FirstOrDefaultAsync();
        if (active != null) active.ReturnedDate = DateTime.Now;
        asset.Status = "Available"; asset.EmployeeId = null;
        await db.SaveChangesAsync();
        return (true, "Asset unassigned.");
    }
}
