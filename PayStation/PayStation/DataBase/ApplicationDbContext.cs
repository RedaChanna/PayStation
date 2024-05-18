using Microsoft.EntityFrameworkCore;

namespace PayStationName.DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet properties representing your tables
        public DbSet<MovementDB> MovementsDB { get; set; }
        public DbSet<AlarmsDB> AlarmsDB { get; set; }
        public DbSet<IngenicoPosMovementDB> IngenicoPosMovementsDB { get; set; }
        public DbSet<CashClosureReceiptDB> CashClosureReceiptsDB { get; set; }
        public DbSet<PartialCashClosureDB> PartialCashClosuresDB { get; set; }
        public DbSet<CashDetailDB> CashDetailsDB { get; set; }
        public DbSet<DeviceEntityDB> DevicesDB { get; set; }
        public DbSet<ParameterDB> ParametersDB { get; set; }
        public DbSet<SerialConnectionParameterDB> SerialConnectionParametersDB { get; set; }
        public DbSet<SerialConnectionSettingDB> SerialConnectionSettingDB { get; set; }
        public DbSet<TicketLayoutDB> TicketLayoutDB { get; set; }
        public DbSet<TextPrinterObjectsDB> TextPrinterObjectsDB { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity type configurations from the assembly containing the DbContext
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
