using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.EntityFrameworkCore;

namespace ITAssetHelpdesk.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (!await db.Users.AnyAsync())
        {
            db.Users.AddRange(
                new AppUser { Username = "admin",  PasswordHash = PasswordHasher.Hash("admin123"), FullName = "System Administrator", Role = "Administrator" },
                new AppUser { Username = "tech",   PasswordHash = PasswordHasher.Hash("tech123"),  FullName = "John Technician",       Role = "Technician" },
                new AppUser { Username = "tech2",  PasswordHash = PasswordHasher.Hash("tech123"),  FullName = "Sarah Mitchell",        Role = "Technician" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Employees.AnyAsync())
        {
            db.Employees.AddRange(
                new Employee { EmployeeCode="EMP-001", FullName="Alice Johnson",  Email="alice.johnson@company.com",  Phone="+1-555-0101", Department="IT",         JobTitle="Software Developer",    DateJoined=new DateTime(2021,3,15) },
                new Employee { EmployeeCode="EMP-002", FullName="Bob Martinez",   Email="bob.martinez@company.com",   Phone="+1-555-0102", Department="HR",         JobTitle="HR Manager",            DateJoined=new DateTime(2019,7,1)  },
                new Employee { EmployeeCode="EMP-003", FullName="Carol Williams", Email="carol.williams@company.com", Phone="+1-555-0103", Department="Finance",    JobTitle="Financial Analyst",     DateJoined=new DateTime(2020,1,10) },
                new Employee { EmployeeCode="EMP-004", FullName="David Chen",     Email="david.chen@company.com",     Phone="+1-555-0104", Department="Marketing",  JobTitle="Marketing Specialist",  DateJoined=new DateTime(2022,5,20) },
                new Employee { EmployeeCode="EMP-005", FullName="Emma Davis",     Email="emma.davis@company.com",     Phone="+1-555-0105", Department="Sales",      JobTitle="Sales Executive",       DateJoined=new DateTime(2021,9,8)  },
                new Employee { EmployeeCode="EMP-006", FullName="Frank Thompson", Email="frank.thompson@company.com", Phone="+1-555-0106", Department="IT",         JobTitle="Network Administrator", DateJoined=new DateTime(2018,11,12)},
                new Employee { EmployeeCode="EMP-007", FullName="Grace Kim",      Email="grace.kim@company.com",      Phone="+1-555-0107", Department="Operations", JobTitle="Operations Manager",    DateJoined=new DateTime(2020,6,3)  },
                new Employee { EmployeeCode="EMP-008", FullName="Henry Brown",    Email="henry.brown@company.com",    Phone="+1-555-0108", Department="Sales",      JobTitle="Sales Manager",         DateJoined=new DateTime(2017,4,25), IsActive=false }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Assets.AnyAsync())
        {
            var employees = await db.Employees.ToListAsync();
            var alice = employees.First(e => e.EmployeeCode == "EMP-001");
            var bob   = employees.First(e => e.EmployeeCode == "EMP-002");
            var carol = employees.First(e => e.EmployeeCode == "EMP-003");
            var david = employees.First(e => e.EmployeeCode == "EMP-004");
            var emma  = employees.First(e => e.EmployeeCode == "EMP-005");
            var frank = employees.First(e => e.EmployeeCode == "EMP-006");

            var assets = new List<Asset>
            {
                new() { AssetTag="AST-0001", Name="Dell XPS 15 Laptop",          Category="Laptop",            Brand="Dell",    Model="XPS 15 9520",          SerialNumber="DXP15-SN-001",    PurchaseDate=new DateTime(2022,6,15),  WarrantyExpiryDate=new DateTime(2025,6,15),   Cost=1799.99m, Status="Assigned",     EmployeeId=alice.Id, Location="Office Desk 12A" },
                new() { AssetTag="AST-0002", Name="HP EliteBook 840",             Category="Laptop",            Brand="HP",     Model="EliteBook 840 G9",     SerialNumber="HPE840-SN-002",   PurchaseDate=new DateTime(2022,8,20),  WarrantyExpiryDate=new DateTime(2025,8,20),   Cost=1349.00m, Status="Assigned",     EmployeeId=bob.Id,   Location="Office Desk 5B" },
                new() { AssetTag="AST-0003", Name="Lenovo ThinkPad X1",           Category="Laptop",            Brand="Lenovo", Model="ThinkPad X1 Carbon",   SerialNumber="LNV-X1-SN-003",   PurchaseDate=new DateTime(2023,1,10),  WarrantyExpiryDate=new DateTime(2026,1,10),   Cost=1599.00m, Status="Available",    EmployeeId=null,     Location="IT Storage Room" },
                new() { AssetTag="AST-0004", Name="Apple MacBook Pro 14",         Category="Laptop",            Brand="Apple",  Model="MacBook Pro 14 M2",    SerialNumber="APL-MBP14-SN-004", PurchaseDate=new DateTime(2023,3,5),  WarrantyExpiryDate=new DateTime(2024,3,5),    Cost=1999.00m, Status="Assigned",     EmployeeId=carol.Id, Location="Office Desk 9C" },
                new() { AssetTag="AST-0005", Name="Dell OptiPlex 7090 Desktop",   Category="Desktop",           Brand="Dell",   Model="OptiPlex 7090",        SerialNumber="DLOPT-SN-005",    PurchaseDate=new DateTime(2021,5,12),  WarrantyExpiryDate=DateTime.Today.AddDays(18),Cost=950.00m,  Status="Assigned",     EmployeeId=david.Id, Location="Marketing Stn 3" },
                new() { AssetTag="AST-0006", Name="HP ProDesk 600",               Category="Desktop",           Brand="HP",     Model="ProDesk 600 G6",       SerialNumber="HPD600-SN-006",   PurchaseDate=new DateTime(2020,11,8),  WarrantyExpiryDate=DateTime.Today.AddDays(12),Cost=799.00m,  Status="Under Repair", EmployeeId=null,     Location="IT Service Desk" },
                new() { AssetTag="AST-0007", Name="LG UltraWide Monitor 34",      Category="Monitor",           Brand="LG",     Model="34WN80C-B",            SerialNumber="LG34W-SN-007",    PurchaseDate=new DateTime(2022,4,18),  WarrantyExpiryDate=new DateTime(2025,4,18),   Cost=499.00m,  Status="Assigned",     EmployeeId=alice.Id, Location="Office Desk 12A" },
                new() { AssetTag="AST-0008", Name="Dell P2419H Monitor 24",       Category="Monitor",           Brand="Dell",   Model="P2419H",               SerialNumber="DLP24-SN-008",    PurchaseDate=new DateTime(2021,7,22),  WarrantyExpiryDate=new DateTime(2024,7,22),   Cost=279.00m,  Status="Available",    EmployeeId=null,     Location="IT Storage Room" },
                new() { AssetTag="AST-0009", Name="HP LaserJet Pro MFP",          Category="Printer",           Brand="HP",     Model="LaserJet MFP M428fdw", SerialNumber="HPLJ-SN-009",     PurchaseDate=new DateTime(2021,9,14),  WarrantyExpiryDate=new DateTime(2024,9,14),   Cost=449.00m,  Status="Available",    EmployeeId=null,     Location="Floor 2 Print Room" },
                new() { AssetTag="AST-0010", Name="Canon PIXMA Office Printer",   Category="Printer",           Brand="Canon",  Model="PIXMA TR8620",          SerialNumber="CAN-TR-SN-010",   PurchaseDate=new DateTime(2022,2,28),  WarrantyExpiryDate=new DateTime(2025,2,28),   Cost=199.00m,  Status="Assigned",     EmployeeId=bob.Id,   Location="HR Office" },
                new() { AssetTag="AST-0011", Name="iPhone 14 Pro",                Category="Mobile Phone",      Brand="Apple",  Model="iPhone 14 Pro",         SerialNumber="APL-IP14-SN-011", PurchaseDate=new DateTime(2022,10,5),  WarrantyExpiryDate=new DateTime(2024,10,5),   Cost=999.00m,  Status="Assigned",     EmployeeId=emma.Id,  Location="Sales Department" },
                new() { AssetTag="AST-0012", Name="Samsung Galaxy S23",           Category="Mobile Phone",      Brand="Samsung",Model="Galaxy S23",            SerialNumber="SAM-S23-SN-012",  PurchaseDate=new DateTime(2023,2,17),  WarrantyExpiryDate=new DateTime(2026,2,17),   Cost=799.00m,  Status="Available",    EmployeeId=null,     Location="IT Storage Room" },
                new() { AssetTag="AST-0013", Name="Cisco Catalyst 2960 Switch",   Category="Network Equipment", Brand="Cisco",  Model="Catalyst 2960-X",      SerialNumber="CSC-CAT-SN-013",  PurchaseDate=new DateTime(2020,3,11),  WarrantyExpiryDate=new DateTime(2025,3,11),   Cost=2499.00m, Status="Available",    EmployeeId=null,     Location="Server Room" },
                new() { AssetTag="AST-0014", Name="iPad Pro 12.9",                Category="Tablet",            Brand="Apple",  Model="iPad Pro 12.9 M2",     SerialNumber="APL-IPAD-SN-014", PurchaseDate=new DateTime(2022,11,9),  WarrantyExpiryDate=new DateTime(2025,11,9),   Cost=1099.00m, Status="Assigned",     EmployeeId=frank.Id, Location="IT Department" },
                new() { AssetTag="AST-0015", Name="Lenovo ThinkPad E15",          Category="Laptop",            Brand="Lenovo", Model="ThinkPad E15 Gen 4",   SerialNumber="LNV-E15-SN-015",  PurchaseDate=new DateTime(2021,8,3),   WarrantyExpiryDate=new DateTime(2024,8,3),    Cost=899.00m,  Status="Retired",      EmployeeId=null,     Location="IT Storage Room" }
            };
            db.Assets.AddRange(assets);
            await db.SaveChangesAsync();

            foreach (var a in assets.Where(x => x.EmployeeId != null))
            {
                db.AssetAssignments.Add(new AssetAssignment
                {
                    AssetId = a.Id,
                    EmployeeId = a.EmployeeId!.Value,
                    AssignedDate = a.PurchaseDate.AddDays(7),
                    Notes = "Initial assignment"
                });
            }
            await db.SaveChangesAsync();
        }

        if (!await db.Tickets.AnyAsync())
        {
            var admin = await db.Users.FirstAsync(u => u.Username == "admin");
            var tech  = await db.Users.FirstAsync(u => u.Username == "tech");
            var tech2 = await db.Users.FirstAsync(u => u.Username == "tech2");
            var now   = DateTime.Now;

            db.Tickets.AddRange(
                new Ticket { TicketNumber="TKT-0001", Title="Laptop running very slowly",         Description="My laptop has been running extremely slowly for the past week. Applications take 2-3 minutes to open.",    Category="Hardware",       Priority="High",     Status="In Progress",      CreatedById=admin.Id, AssignedTechnicianId=tech.Id,  CreatedDate=now.AddDays(-5),  UpdatedDate=now.AddDays(-3) },
                new Ticket { TicketNumber="TKT-0002", Title="Cannot connect to VPN",              Description="Unable to connect to the company VPN from home. Error: Connection timeout.",                               Category="Network",        Priority="High",     Status="Open",             CreatedById=admin.Id, AssignedTechnicianId=null,     CreatedDate=now.AddDays(-2),  UpdatedDate=now.AddDays(-2) },
                new Ticket { TicketNumber="TKT-0003", Title="Email not syncing on mobile",        Description="Company email has stopped syncing on iPhone 14 Pro since yesterday morning.",                              Category="Email",          Priority="Medium",   Status="Resolved",         CreatedById=admin.Id, AssignedTechnicianId=tech.Id,  CreatedDate=now.AddDays(-10), UpdatedDate=now.AddDays(-8),  ResolvedDate=now.AddDays(-8),  ResolutionNotes="Reconfigured Exchange account. Email sync restored." },
                new Ticket { TicketNumber="TKT-0004", Title="Printer not responding HR Office",   Description="The Canon PIXMA printer in HR Office is showing offline.",                                                Category="Printer",        Priority="Medium",   Status="Waiting for User", CreatedById=admin.Id, AssignedTechnicianId=tech2.Id, CreatedDate=now.AddDays(-3),  UpdatedDate=now.AddDays(-1) },
                new Ticket { TicketNumber="TKT-0005", Title="New employee account setup",         Description="New employee starting Monday. Need full account setup: email, VPN access, and software.",                 Category="Account Access", Priority="High",     Status="Open",             CreatedById=admin.Id, AssignedTechnicianId=null,     CreatedDate=now.AddDays(-1),  UpdatedDate=now.AddDays(-1) },
                new Ticket { TicketNumber="TKT-0006", Title="Suspected malware on workstation",   Description="Marketing workstation showing unusual behavior. Multiple pop-up ads and browser redirects.",               Category="Security",       Priority="Critical", Status="In Progress",      CreatedById=admin.Id, AssignedTechnicianId=tech.Id,  CreatedDate=now.AddDays(-1),  UpdatedDate=now },
                new Ticket { TicketNumber="TKT-0007", Title="Microsoft Office activation issue",  Description="Office applications showing Product Activation Failed error on multiple machines.",                        Category="Software",       Priority="High",     Status="Resolved",         CreatedById=admin.Id, AssignedTechnicianId=tech2.Id, CreatedDate=now.AddDays(-15), UpdatedDate=now.AddDays(-14), ResolvedDate=now.AddDays(-14), ResolutionNotes="Renewed Microsoft 365 licenses. Reactivated on all affected machines." },
                new Ticket { TicketNumber="TKT-0008", Title="Password reset request",             Description="User forgot domain password and is locked out of critical EOD reports.",                                   Category="Account Access", Priority="Medium",   Status="Closed",           CreatedById=admin.Id, AssignedTechnicianId=tech.Id,  CreatedDate=now.AddDays(-20), UpdatedDate=now.AddDays(-20), ResolvedDate=now.AddDays(-20), ResolutionNotes="Password reset via Active Directory." },
                new Ticket { TicketNumber="TKT-0009", Title="Shared drive access permissions",   Description="Sales team cannot access the new Q3-Reports shared folder. Permission denied errors.",                     Category="Network",        Priority="Low",      Status="Open",             CreatedById=admin.Id, AssignedTechnicianId=null,     CreatedDate=now.AddHours(-5), UpdatedDate=now.AddHours(-5) },
                new Ticket { TicketNumber="TKT-0010", Title="Video conferencing audio issues",    Description="During Teams meetings, audio keeps cutting out or has significant echo.",                                   Category="Software",       Priority="Medium",   Status="In Progress",      CreatedById=admin.Id, AssignedTechnicianId=tech2.Id, CreatedDate=now.AddDays(-4),  UpdatedDate=now.AddDays(-2) },
                new Ticket { TicketNumber="TKT-0011", Title="Monitor display flickering",         Description="Dell monitor on Desk 9C is flickering intermittently, especially after warming up.",                       Category="Hardware",       Priority="Low",      Status="Resolved",         CreatedById=admin.Id, AssignedTechnicianId=tech.Id,  CreatedDate=now.AddDays(-25), UpdatedDate=now.AddDays(-23), ResolvedDate=now.AddDays(-23), ResolutionNotes="Replaced DisplayPort cable. Monitor functioning normally." },
                new Ticket { TicketNumber="TKT-0012", Title="Server room temperature alert",      Description="Automated temperature alert: 28 degrees C above 26 degree threshold. Possible HVAC issue.",               Category="Hardware",       Priority="Critical", Status="Resolved",         CreatedById=admin.Id, AssignedTechnicianId=tech2.Id, CreatedDate=now.AddDays(-7),  UpdatedDate=now.AddDays(-7),  ResolvedDate=now.AddDays(-7),  ResolutionNotes="Emergency HVAC service called. Temperature normalized to 23 degrees C." }
            );
            await db.SaveChangesAsync();
        }
    }
}
