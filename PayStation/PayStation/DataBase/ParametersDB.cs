using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayStationName.DataBase
{
    public class ParameterDB
    {
        [Key]
        public int Id { get; set; }

        [Column("Name")]
        public string? Name { get; set; }

        [Column("Value")]
        public string? Value { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("Row1")]
        public string? Row1 { get; set; }

        [Column("Row2")]
        public string? Row2 { get; set; }

        [Column("Row3")]
        public string? Row3 { get; set; }

        [Column("Row4")]
        public string? Row4 { get; set; }

        [Column("Row5")]
        public string? Row5 { get; set; }

        [Column("Row6")]
        public string? Row6 { get; set; }

        [Column("Row7")]
        public string? Row7 { get; set; }

        [Column("Cash_ID")]
        public int? CashId { get; set; }

        [Column("Terminal_ID")]
        public string? TerminalId { get; set; }

        [Column("BanknoteValueInCassetteBox1")]
        public int? BanknoteValueInCassetteBox1 { get; set; }

        [Column("MaxBanknoteGiveBackBox1")]
        public int? MaxBanknoteGiveBackBox1 { get; set; }

        [Column("BanknoteValueInCassetteBox2")]
        public int? BanknoteValueInCassetteBox2 { get; set; }

        [Column("MaxBanknoteGiveBackBox2")]
        public int? MaxBanknoteGiveBackBox2 { get; set; }
    }
}
