using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayStationName.DataBase;
using System;

namespace PayStationName.DataBase.Seeding
{
    public class ParameterSeed : IEntityTypeConfiguration<ParameterDB>
    {
        public void Configure(EntityTypeBuilder<ParameterDB> builder)
        {
            builder.HasData(
                new ParameterDB
                {
                    Id = 1,
                    Name = "Parameter1",
                    Value = "Value1",
                    Description = "Description1",
                    Row1 = "Row1",
                    Row2 = "Row2",
                    Row3 = "Row3",
                    Row4 = "Row4",
                    Row5 = "Row5",
                    Row6 = "Row6",
                    Row7 = "Row7",
                    CashId = 1,
                    TerminalId = "Terminal1",
                    BanknoteValueInCassetteBox1 = 10,
                    MaxBanknoteGiveBackBox1 = 5,
                    BanknoteValueInCassetteBox2 = 20,
                    MaxBanknoteGiveBackBox2 = 10
                }
            );
        }
    }
}
