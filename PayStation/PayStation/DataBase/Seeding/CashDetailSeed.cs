using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace PayStationSW.DataBase.Seeding
{
    public class CashDetailSeed : IEntityTypeConfiguration<CashDetailDB>
    {

        public void Configure(EntityTypeBuilder<CashDetailDB> builder)
        {
            builder.HasData(
                new CashDetailDB
                {
                    Id = 1,
                    Tube10 = 50,
                    Tube20 = 50,
                    Tube50 = 50,
                    Tube100 = 50,
                    Tube200 = 50,
                    DispensedBanknote = 10,
                    DispensedBanknote1 = 20
                }
            );
        }
    }
}
