using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayStationSW.DataBase;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Ports;

namespace PayStationSW.DataBase.Seeding
{
    public class TextPrinterObjectsSeed : IEntityTypeConfiguration<TextPrinterObjectsDB>
    {
        public void Configure(EntityTypeBuilder<TextPrinterObjectsDB> builder)
        {
            builder.HasData(
                new TextPrinterObjectsDB
                {
                    Id = 1,
                    Name = "Company",
                    Type = 1,
                    Text = "Company",
                    Highlighting = false,
                    TypeFont = 0,
                    SizeWidth = 1,
                    SizeHeight = 1,
                    Underline = false,
                    Bold = false,
                    Overlapping = false,
                    UpsideDown = false,
                    Revolving = false,
                    LeftMargin = 0,
                    Alligment = 0,
                    DistanceBeforeObj = 0,
                    DistanceAfterObj = 0
                }
            );
        }
    }
}