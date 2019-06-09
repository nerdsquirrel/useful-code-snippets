public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext()
        : base(WebConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString, false)
    {
        Configuration.LazyLoadingEnabled = false;
        Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationRole>()
            .ToTable("AspNetRoles")
            .HasRequired(r => r.Organization)
            .WithMany()
            .HasForeignKey(r => r.OrganizationId);

        modelBuilder.Entity<ApplicationPermission>()
            .HasMany(p => p.Roles)
            .WithMany(p => p.Permissions);

        base.OnModelCreating(modelBuilder);
    }
    
    // savechanges with error handling
    public override int SaveChanges()
    {
        try
        {
            return base.SaveChanges();
        }
        catch (DbEntityValidationException ex)
        {
            // Retrieve the error messages as a list of strings.
            var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

            // Join the list to a single string.
            var fullErrorMessage = string.Join("; ", errorMessages);
            throw new DbEntityValidationException(fullErrorMessage, ex.EntityValidationErrors);
        }
    }

    public DbSet<ApplicationRole> ApplicationRoles { get; set; }
    public DbSet<ApplicationPermission> ApplicationPermissions { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    
}

public class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
{
    public Configuration()
    {
        AutomaticMigrationsEnabled = true;
        AutomaticMigrationDataLossAllowed = true;
    }

    // if data seed is needed
    protected override void Seed(ApplicationDbContext context)
    {
        base.Seed(context);

        var permissions = new List<ApplicationPermission>
        {
            new ApplicationPermission {
                Name = "ViewEmployeeList", Description = "Access to Employee List"
            }, new ApplicationPermission {
                Name = "ViewEmployeeDetails", Description = "Access to Employee Details"
            }
        };

        context.ApplicationPermissions.AddRange(permissions);            
        context.SaveChanges();
    }
    
}