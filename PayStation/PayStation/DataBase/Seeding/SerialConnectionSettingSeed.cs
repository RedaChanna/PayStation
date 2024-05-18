using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayStationName.DataBase;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Ports;

namespace PayStationName.DataBase.Seeding
{
    public class SerialConnectionSettingSeed : IEntityTypeConfiguration<SerialConnectionSettingDB>
    {
        public void Configure(EntityTypeBuilder<SerialConnectionSettingDB> builder)
        {
            builder.HasData(
                new SerialConnectionSettingDB
                {
                    Id = 1,
                    Device = "CASH",
                    MaxRetryAttempts = 3,
                    RetryDelayMilliseconds = 300,
                    IsTimedMode = true
                }, 
                new SerialConnectionSettingDB
                {
                    Id = 2,
                    Device = "PRINTER",
                    MaxRetryAttempts = 3,
                    RetryDelayMilliseconds = 300,
                    IsTimedMode = true
                },
                                
                new SerialConnectionSettingDB
                {
                    Id = 3,
                    Device = "COIN",
                    MaxRetryAttempts = 3,
                    RetryDelayMilliseconds = 300,
                    IsTimedMode = true
                },

                new SerialConnectionSettingDB
                {
                    Id = 4,
                    Device = "POS",
                    MaxRetryAttempts = 3,
                    RetryDelayMilliseconds = 300,
                    IsTimedMode = true
                }
            );
        }
    }
}