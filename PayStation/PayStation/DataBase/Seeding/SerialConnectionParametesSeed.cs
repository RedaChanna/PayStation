using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayStationSW.DataBase;
using System;
using System.IO.Ports;

namespace PayStationSW.DataBase.Seeding
{
    public class SerialConnectionParameterSeed : IEntityTypeConfiguration<SerialConnectionParameterDB>
    {
        public void Configure(EntityTypeBuilder<SerialConnectionParameterDB> builder)
        {
            builder.HasData(
                new SerialConnectionParameterDB
                {
                    Id = 1,
                    Device = DeviceEnum.Cash,
                    LastPortName = "COM2",
                    BaudRate = 9600,
                    Parity = 2,
                    StopBits = 1,
                    DataBits=8,
                    Handshake=0
                },
                new SerialConnectionParameterDB
                {
                    Id = 2,
                    Device = DeviceEnum.Printer,
                    LastPortName = "COM8",
                    BaudRate = 9600,
                    Parity = 2,
                    StopBits = 1,
                    DataBits = 8,
                    Handshake = 0
                },
                new SerialConnectionParameterDB
                {
                    Id = 3,
                    Device = DeviceEnum.Coin,
                    LastPortName = "COM7",
                    BaudRate = 9600,
                    Parity = 0,
                    StopBits = 1,
                    DataBits = 8,
                    Handshake = 0
                },
                new SerialConnectionParameterDB
                {
                    Id = 4,
                    Device = DeviceEnum.Pos,
                    LastPortName = "COM8",
                    BaudRate = 115200,
                    Parity = 0,
                    StopBits = 1,
                    DataBits = 8,
                    Handshake = 0
                }
            );
        }
    }
}