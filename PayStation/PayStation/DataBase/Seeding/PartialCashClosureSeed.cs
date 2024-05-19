using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace PayStationSW.DataBase.Seeding
{

    public class PartialCashClosureSeed : IEntityTypeConfiguration<PartialCashClosureDB>
    {
        public void Configure(EntityTypeBuilder<PartialCashClosureDB> builder)
        {
            builder.HasData(
                new PartialCashClosureDB
                {
                    ClosingProgress = 1,
                    DateUpdate = DateTime.Now,
                    NonGivenChange = 100,
                    BanknoteLoad = 200,
                    BanknoteLoad5RC = 50,
                    BanknoteLoad10RC = 50,
                    BanknoteLoad20RC = 50,
                    BanknoteLoad5Stacker = 50,
                    BanknoteLoad10Stacker = 50,
                    BanknoteLoad20Stacker = 50,
                    BanknoteLoad50Stacker = 50,
                    CoinLoad = 100,
                    CoinLoad10 = 20,
                    CoinLoad20 = 20,
                    CoinLoad50 = 20,
                    CoinLoad100 = 20,
                    CoinLoad200 = 20,
                    TotalPaidCents = 500,
                    TotalPaidCentsCash = 200,
                    PosIncome = 300,
                    BanknoteIntroduction = 150,
                    BanknoteIntroduction5 = 30,
                    BanknoteIntroduction10 = 30,
                    BanknoteIntroduction20 = 30,
                    BanknoteIntroduction50 = 30,
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
                    Change2000 = 0
                }
            );
        }
    }
}
