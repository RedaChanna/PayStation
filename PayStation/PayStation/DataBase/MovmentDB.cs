using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayStationSW.DataBase
{ 
    public class MovementDB
    {
        [Key]
        [Column("id_movement")]
        public int? Id { get; set; }

        [Column("movement_date_open")]
        public DateTime? MovementDateOpen { get; set; }

        [Column("movement_date_close")]
        public DateTime? MovementDateClose { get; set; }

        [Column("amount")]
        public int? Amount { get; set; }

        [Column("outcome")]
        [MaxLength(3)]
        public string? Outcome { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("paid_cents")]
        public int? PaidCents { get; set; }

        [Column("banknotes")]
        public int? Banknotes { get; set; }

        [Column("coins")]
        public int? Coins { get; set; }

        [Column("change")]
        public int? Change { get; set; }

        [Column("change_banknotes")]
        public int? ChangeBanknotes { get; set; }

        [Column("change_banknotes1")]
        public int? ChangeBanknotes1 { get; set; }

        [Column("closing_progress")]
        public int? ClosingProgress { get; set; }
        [Column("operator_code")]
        public string? OperatorCode { get; set; }

    }

}