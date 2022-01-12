using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppContext : IdentityDbContext
{
public AppContext(DbContextOptions options) : base(options) {}

public DbSet<AppointmentModel> Appointments {get;set;}
}