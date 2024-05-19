using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayStationSW.DataBase
{
    public class CashClosureReceiptDB
    {
        [Key]
        [Column("id_movement")]
        public int? Id { get; set; }

        [Column("closing_progress")]
        public int? ClosingProgress { get; set; }

        [Column("movement_date")]
        public DateTime? MovementDate { get; set; }

        [Column("operator")]
        public string? Operator { get; set; }

        [Column("outcome")]
        [MaxLength(3)]
        public string? Outcome { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("non_given_change")]
        public int? NonGivenChange { get; set; }

        [Column("banknotes_load")]
        public int? BanknotesLoad { get; set; }

        [Column("banknotes_load_5_rc")]
        public int? BanknotesLoad5RC { get; set; }

        [Column("banknotes_load_10_rc")]
        public int? BanknotesLoad10RC { get; set; }

        [Column("banknotes_load_20_rc")]
        public int? BanknotesLoad20RC { get; set; }

        [Column("banknotes_load_5_stacker")]
        public int? BanknotesLoad5Stacker { get; set; }

        [Column("banknotes_load_10_stacker")]
        public int? BanknotesLoad10Stacker { get; set; }

        [Column("banknotes_load_20_stacker")]
        public int? BanknotesLoad20Stacker { get; set; }

        [Column("banknotes_load_50_stacker")]
        public int? BanknotesLoad50Stacker { get; set; }

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

        [Column("non_loaded_banknotes_total")]
        public int? NonLoadedBanknotesTotal { get; set; }

        [Column("non_loaded_banknotes")]
        public int? NonLoadedBanknotes { get; set; }

        [Column("non_loaded_banknotes1")]
        public int? NonLoadedBanknotes1 { get; set; }

        [Column("total_paid_cents")]
        public int? TotalPaidCents { get; set; }

        [Column("total_paid_cents_cash")]
        public int? TotalPaidCentsCash { get; set; }

        [Column("pos_income")]
        public int? PosIncome { get; set; }

        [Column("banknotes_introduction")]
        public int? BanknotesIntroduction { get; set; }

        [Column("banknotes_introduction_5")]
        public int? BanknotesIntroduction5 { get; set; }

        [Column("banknotes_introduction_10")]
        public int? BanknotesIntroduction10 { get; set; }

        [Column("banknotes_introduction_20")]
        public int? BanknotesIntroduction20 { get; set; }

        [Column("banknotes_introduction_50")]
        public int? BanknotesIntroduction50 { get; set; }

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

        [Column("present_banknotes_total")]
        public int? PresentBanknotesTotal { get; set; }

        [Column("present_banknote_5")]
        public int? PresentBanknote5 { get; set; }

        [Column("present_banknote_10")]
        public int? PresentBanknote10 { get; set; }

        [Column("present_banknote_20")]
        public int? PresentBanknote20 { get; set; }

        [Column("present_banknote_50")]
        public int? PresentBanknote50 { get; set; }

        [Column("present_coins_total")]
        public int? PresentCoinsTotal { get; set; }

        [Column("present_coin_10")]
        public int? PresentCoin10 { get; set; }

        [Column("present_coin_20")]
        public int? PresentCoin20 { get; set; }

        [Column("present_coin_50")]
        public int? PresentCoin50 { get; set; }

        [Column("present_coin_100")]
        public int? PresentCoin100 { get; set; }

        [Column("present_coin_200")]
        public int? PresentCoin200 { get; set; }
    }
}
