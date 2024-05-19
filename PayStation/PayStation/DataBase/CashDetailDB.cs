using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayStationSW.DataBase
{
    public class CashDetailDB
    {
        [Key]
        public int? Id { get; set; }

        [Column("tube_10")]
        public short? Tube10 { get; set; }

        [Column("tube_20")]
        public short? Tube20 { get; set; }

        [Column("tube_50")]
        public short? Tube50 { get; set; }

        [Column("tube_100")]
        public short? Tube100 { get; set; }

        [Column("tube_200")]
        public short? Tube200 { get; set; }

        [Column("dispensed_banknote")]
        public short? DispensedBanknote { get; set; }

        [Column("dispensed_banknote1")]
        public short? DispensedBanknote1 { get; set; }
    }
}
