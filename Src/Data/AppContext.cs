using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppContext : IdentityDbContext<AppUser>
{
public AppContext(DbContextOptions options) : base(options) {}

public DbSet<AppointmentModel> Appointments {get;set;}
}