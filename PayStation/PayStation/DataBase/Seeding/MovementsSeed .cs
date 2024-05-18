using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PayStationName.DataBase.Seeding
{
    public class MovementsSeed : IEntityTypeConfiguration<MovementDB>
    {
        public void Configure(EntityTypeBuilder<MovementDB> builder)
        {
            builder.HasData(
                new MovementDB
                {
                    Id = 1,
                    MovementDate = DateTime.Now.AddDays(-5),
                    Outcome = "IN",
                    Description = "Initial deposit",
                    PaidCents = 1000,
                    Banknotes = 5,
                    Coins = 10,
                    Change = 0,
                    ChangeBanknotes = 0,
                    ChangeBanknotes1 = 0,
                    ClosingProgress = 0,
                    OperatorCode = "A"
                }
            );
        }
    }
}
