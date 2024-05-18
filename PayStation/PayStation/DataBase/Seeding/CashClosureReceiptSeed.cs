using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Data.Entity;

namespace PayStationName.DataBase.Seeding
{
    public class CashClosureReceiptSeed : IEntityTypeConfiguration<CashClosureReceiptDB>
    {

        public void Configure(EntityTypeBuilder<CashClosureReceiptDB> builder)
        {


            builder.HasData(
                new CashClosureReceiptDB
                {
                    Id = 1,
                    ClosingProgress = 1,
                    MovementDate = DateTime.Now,
                    Operator = "Operator1",
                    Outcome = "OUT",
                    Description = "Description1",
                    NonGivenChange = 100,
                    BanknotesLoad = 200,
                    BanknotesLoad5RC = 50,
                    BanknotesLoad10RC = 50,
                    BanknotesLoad20RC = 50,
                    BanknotesLoad5Stacker = 50,
                    BanknotesLoad10Stacker = 50,
                    BanknotesLoad20Stacker = 50,
                    BanknotesLoad50Stacker = 50,
                    CoinLoad = 100,
                    CoinLoad10 = 20,
                    CoinLoad20 = 20,
                    CoinLoad50 = 20,
                    CoinLoad100 = 20,
                    CoinLoad200 = 20,
                    NonLoadedBanknotesTotal = 150,
                    NonLoadedBanknotes = 30,
                    NonLoadedBanknotes1 = 30,
                    TotalPaidCents = 500,
                    TotalPaidCentsCash = 200,
                    PosIncome = 300,
                    BanknotesIntroduction = 150,
                    BanknotesIntroduction5 = 30,
                    BanknotesIntroduction10 = 30,
                    BanknotesIntroduction20 = 30,
                    BanknotesIntroduction50 = 30,
                    CoinIntroduction = 50,
                    CoinIntroduction10 = 10,
                    CoinIntroduction20 = 10,
                    CoinIntroduction50 = 10,
                    CoinIntroduction100 = 10,
                    CoinIntroduction200 = 10,
                    ExcessCoin10 = 5,
                    ExcessCoin20 = 5,
                    ExcessCoin50 = 5,
                    ExcessCoin100 = 5,
                    ExcessCoin200 = 5,
                    Change = 50,
                    Change10 = 10,
                    Change20 = 10,
                    Change50 = 10,
                    Change100 = 10,
                    Change500 = 0,
                    Change1000 = 0,
                    Change2000 = 0,
                    PresentBanknotesTotal = 150,
                    PresentBanknote5 = 30,
                    PresentBanknote10 = 30,
                    PresentBanknote20 = 30,
                    PresentBanknote50 = 30,
                    PresentCoinsTotal = 50,
                    PresentCoin10 = 10,
                    PresentCoin20 = 10,
                    PresentCoin50 = 10,
                    PresentCoin100 = 10,
                    PresentCoin200 = 10
                }
            );
        }
    }
}
