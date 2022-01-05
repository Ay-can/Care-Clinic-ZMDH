using Microsoft.EntityFrameworkCore;

public class AppContext : DbContext
{
public AppContext(DbContextOptions options) : base(options) {}
}