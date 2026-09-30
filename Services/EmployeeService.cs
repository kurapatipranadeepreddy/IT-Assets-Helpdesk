using ITAssetHelpdesk.Data;
using ITAssetHelpdesk.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAssetHelpdesk.Services;

public class EmployeeService(AppDbContext db)
{
    public async Task<List<Employee>> GetAllAsync(string? search = null, string? dept = null)
    {
        var q = db.Employees.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) { var s = search.ToLower(); q = q.Where(e => e.FullName.ToLower().Contains(s) || e.Email.ToLower().Contains(s) || e.Department.ToLower().Contains(s)); }
        if (!string.IsNullOrWhiteSpace(dept) && dept != "All") q = q.Where(e => e.Department == dept);
        return await q.OrderBy(e => e.FullName).ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id) =>
        await db.Employees.Include(e => e.Assets).Include(e => e.AssetAssignments).ThenInclude(aa => aa.Asset)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<string> NextCodeAsync()
    {
        var count = await db.Employees.CountAsync();
        return $"EMP-{count + 1:D3}";
    }

    public async Task<List<string>> GetDepartmentsAsync() =>
        await db.Employees.Select(e => e.Department).Distinct().OrderBy(d => d).ToListAsync();

    public async Task<(bool ok, string msg)> AddAsync(Employee e)
    {
        if (await db.Employees.AnyAsync(x => x.Email == e.Email)) return (false, "Email already exists.");
        if (await db.Employees.AnyAsync(x => x.EmployeeCode == e.EmployeeCode)) return (false, "Employee code already exists.");
        db.Employees.Add(e); await db.SaveChangesAsync();
        return (true, "Employee added.");
    }

    public async Task<(bool ok, string msg)> UpdateAsync(Employee e)
    {
        if (await db.Employees.AnyAsync(x => x.Email == e.Email && x.Id != e.Id)) return (false, "Email already in use.");
        db.Employees.Update(e); await db.SaveChangesAsync();
        return (true, "Employee updated.");
    }

    public async Task<(bool ok, string msg)> DeleteAsync(int id)
    {
        var e = await db.Employees.Include(x => x.Assets).FirstOrDefaultAsync(x => x.Id == id);
        if (e == null) return (false, "Not found.");
        if (e.Assets.Any(a => a.Status == "Assigned")) return (false, $"Employee has {e.Assets.Count(a => a.Status == "Assigned")} active asset(s). Unassign first.");
        db.Employees.Remove(e); await db.SaveChangesAsync();
        return (true, "Employee deleted.");
    }
}
