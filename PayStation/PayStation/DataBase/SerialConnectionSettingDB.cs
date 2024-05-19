using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Ports;

namespace PayStationSW.DataBase
{
    public class SerialConnectionSettingDB
    {
        [Key]
        public int Id { get; set; }

        [Column("Device")]
        public string? Device { get; set; }

        [Column("MaxRetryAttempts")]
        public int MaxRetryAttempts { get; set; }

        [Column("RetryDelayMilliseconds")]
        public int RetryDelayMilliseconds { get; set; }

        [Column("IsTimedMode")]
        public bool IsTimedMode { get; set; }

    }
}