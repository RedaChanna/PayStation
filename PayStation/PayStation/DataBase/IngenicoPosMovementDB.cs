using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayStationName.DataBase
{
    public class IngenicoPosMovementDB
    {
        [Key]
        [Column("id_mov")]
        public int? Id { get; set; }
        [Column("id_movmentDB")]
        public int? IdMovmentDB { get; set; }

        [Column("paid_cents")]
        public int? PaidCents { get; set; }

        [Column("transaction_date")]
        public DateTime? TransactionDate { get; set; }

        [Column("success")]
        [MaxLength(1)]
        public string? Success { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("overhead")]
        public string? Overhead { get; set; }
    }
}
