using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayStationName.DataBase;

namespace PayStationName.DataBase.Seeding
{
    public class DeviceEntitySeed : IEntityTypeConfiguration<DeviceEntityDB>
        {
            public void Configure(EntityTypeBuilder<DeviceEntityDB> builder)
            {
                builder.HasData(
                new DeviceEntityDB
                {
                    DeviceType = "1",
                    Description = "Gryphon (lettore monete)",
                    Enabled = "0"
                },
                new DeviceEntityDB
                {
                    DeviceType = "2",
                    Description = "Vega Pro (lettore banconote)",
                    Enabled = "0"
                },
                new DeviceEntityDB
                {
                    DeviceType = "3",
                    Description = "isef2000 (lettore POS)",
                    Enabled = "0"
                },
                new DeviceEntityDB
                {
                    DeviceType = "4",
                    Description = "KP300 (stampante ticket)",
                    Enabled = "0"
                },
                new DeviceEntityDB
                {
                    DeviceType = "5",
                    Description = "RC (rendi banconote sigolo taglio)",
                    Enabled = "0"
                },
                new DeviceEntityDB
                {
                    DeviceType = "6",
                    Description = "TWIN (rendi banconote doppio taglio)",
                    Enabled = "0"
                }
                );
        }
    }
}