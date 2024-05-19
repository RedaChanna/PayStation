using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayStationSW.DataBase;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Ports;

namespace PayStationSW.DataBase.Seeding
{
    public class TicketLayoutSeed : IEntityTypeConfiguration<TicketLayoutDB>
    {
        public void Configure(EntityTypeBuilder<TicketLayoutDB> builder)
        {
            builder.HasData(
                new TicketLayoutDB
                {
                    Id = 1,
                    Name = "Ticket cassa",
                    Objects = [ 1, 2 ]
                }
            );
        }
    }
}