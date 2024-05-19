using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayStationSW.DataBase;
using System;
using System.Collections.Generic;

namespace PayStationSW.DataBase.Seeding
{
    public class IngenicoPosMovementsSeed : IEntityTypeConfiguration<IngenicoPosMovementDB>
    {
        public void Configure(EntityTypeBuilder<IngenicoPosMovementDB> builder)
        {
            builder.HasData(
                new IngenicoPosMovementDB
                {
                    Id = 1,
                    IdMovmentDB = 0,
                    PaidCents = 1000,
                    TransactionDate = DateTime.Now,
                    Success = "Y",
                    Description = "Payment successful",
                    Overhead = "Some overhead data"
                }
            );
        }
    }
}
