using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PayStationSW.DataBase
{
    public class TicketLayoutDB
    {
        [Key]
        public int Id { get; set; }

        [Column("Name")]
        public string? Name { get; set; }

        [Column("Objects")]
        public int[]? Objects { get; set; }
    }
}