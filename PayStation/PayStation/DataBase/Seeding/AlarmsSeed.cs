using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PayStationName.DataBase.Seeding
{
    public class AlarmsSeed : IEntityTypeConfiguration<AlarmsDB>
    {
        public void Configure(EntityTypeBuilder<AlarmsDB> builder)
        {
            builder.HasData(
                new AlarmsDB
                {
                    IdAlarm = 1,
                    AlarmDate = DateTime.Now.AddDays(-5),
                    Status = "I",
                    AlarmCode = "001",
                    Description = "Alarms",
                    OperatorCode = "A"
                }
            ) ;
        }
    }
}