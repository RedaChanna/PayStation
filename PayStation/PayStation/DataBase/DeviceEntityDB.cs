using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayStationSW.DataBase
{
    public class DeviceEntityDB
    {
        [Key]
        [Column("device_type")]
        [MaxLength(2)]
        public string? DeviceType { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("enabled")]
        [MaxLength(1)]
        public string? Enabled { get; set; }
    }
}
