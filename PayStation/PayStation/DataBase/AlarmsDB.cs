using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayStationName.DataBase
{
    public class AlarmsDB
    {
        [Key]

        [Column("id_alarm")]
        public int? IdAlarm { get; set; }

        [Column("alarm_date")]
        public DateTime? AlarmDate { get; set; }

        [Column("status_code")]
        [MaxLength(1)]
        public string? Status { get; set; }

        [Column("alarm_code")]
        [MaxLength(3)]
        public string? AlarmCode { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("operator_code")]
        public string? OperatorCode { get; set; }
        [Column("test_colum")]
        public string? TestColumn { get; set; }
    }
}
