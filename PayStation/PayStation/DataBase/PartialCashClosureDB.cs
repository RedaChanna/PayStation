using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayStationSW.DataBase
{
    public class PartialCashClosureDB
    {
        [Key]
        [Column("closing_progress")]
        public int? ClosingProgress { get; set; }

        [Column("date_update")]
        public DateTime? DateUpdate { get; set; }

        [Column("non_given_change")]
        public int? NonGivenChange { get; set; }

        [Column("banknote_load")]
        public int? BanknoteLoad { get; set; }

        [Column("banknote_load_5_rc")]
        public int? BanknoteLoad5RC { get; set; }

        [Column("banknote_load_10_rc")]
        public int? BanknoteLoad10RC { get; set; }

        [Column("banknote_load_20_rc")]
        public int? BanknoteLoad20RC { get; set; }

        [Column("banknote_load_5_stacker")]
        public int? BanknoteLoad5Stacker { get; set; }

        [Column("banknote_load_10_stacker")]
        public int? BanknoteLoad10Stacker { get; set; }

        [Column("banknote_load_20_stacker")]
        public int? BanknoteLoad20Stacker { get; set; }

        [Column("banknote_load_50_stacker")]
        public int? BanknoteLoad50Stacker { get; set; }

        [Column("coin_load")]
        public int? CoinLoad { get; set; }

        [Column("coin_load_10")]
        public int? CoinLoad10 { get; set; }

        [Column("coin_load_20")]
        public int? CoinLoad20 { get; set; }

        [Column("coin_load_50")]
        public int? CoinLoad50 { get; set; }

        [Column("coin_load_100")]
        public int? CoinLoad100 { get; set; }

        [Column("coin_load_200")]
        public int? CoinLoad200 { get; set; }

        [Column("total_paid_cents")]
        public int? TotalPaidCents { get; set; }

        [Column("total_paid_cents_cash")]
        public int? TotalPaidCentsCash { get; set; }

        [Column("pos_income")]
        public int? PosIncome { get; set; }

        [Column("banknote_introduction")]
        public int? BanknoteIntroduction { get; set; }

        [Column("banknote_introduction_5")]
        public int? BanknoteIntroduction5 { get; set; }

        [Column("banknote_introduction_10")]
        public int? BanknoteIntroduction10 { get; set; }

        [Column("banknote_introduction_20")]
        public int? BanknoteIntroduction20 { get; set; }

        [Column("banknote_introduction_50")]
        public int? BanknoteIntroduction50 { get; set; }

        [Column("coin_introduction")]
        public int? CoinIntroduction { get; set; }

        [Column("coin_introduction_10")]
        public int? CoinIntroduction10 { get; set; }

        [Column("coin_introduction_20")]
        public int? CoinIntroduction20 { get; set; }

        [Column("coin_introduction_50")]
        public int? CoinIntroduction50 { get; set; }

        [Column("coin_introduction_100")]
        public int? CoinIntroduction100 { get; set; }

        [Column("coin_introduction_200")]
        public int? CoinIntroduction200 { get; set; }

        [Column("excess_coin_10")]
        public int? ExcessCoin10 { get; set; }

        [Column("excess_coin_20")]
        public int? ExcessCoin20 { get; set; }

        [Column("excess_coin_50")]
        public int? ExcessCoin50 { get; set; }

        [Column("excess_coin_100")]
        public int? ExcessCoin100 { get; set; }

        [Column("excess_coin_200")]
        public int? ExcessCoin200 { get; set; }

        [Column("change")]
        public int? Change { get; set; }

        [Column("change_10")]
        public int? Change10 { get; set; }

        [Column("change_20")]
        public int? Change20 { get; set; }

        [Column("change_50")]
        public int? Change50 { get; set; }

        [Column("change_100")]
        public int? Change100 { get; set; }

        [Column("change_500")]
        public int? Change500 { get; set; }

        [Column("change_1000")]
        public int? Change1000 { get; set; }

        [Column("change_2000")]
        public int? Change2000 { get; set; }
    }
}