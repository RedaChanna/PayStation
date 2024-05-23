using System;
using PayStationSW.DataBase;

namespace PayStationSW
{
    public class PaymentProcessor
    {
        private MovementDB movement;

        public PaymentProcessor(MovementDB movement)
        {
            this.movement = movement;
            this.movement.MovementDateOpen = DateTime.Now;
            this.movement.PaidCents = 0;
            this.movement.Coins = 0;
            this.movement.Banknotes = 0;
            this.movement.Change = 0;
            this.movement.Outcome = "N/A";
        }

        public void StartPaymentProcess()
        {
            Console.WriteLine("Payment process started.");

            while (movement.PaidCents < movement.Amount)
            {
                Console.WriteLine($"Amount due: {movement.Amount / 100:C}. Please insert coins or notes.");
                int amountInserted = InsertMoney();
                movement.PaidCents += amountInserted;
                if (amountInserted > 0)
                {
                    if (amountInserted < 100)
                        movement.Coins += amountInserted;
                    else
                        movement.Banknotes += amountInserted;
                }

                if (movement.PaidCents >= movement.Amount)
                {
                    DisableMoneyInput();
                    int change = 10;//movement.PaidCents - movement.Amount.Value;
                    if (change > 0)
                    {
                        DispenseChange(change);
                    }
                    movement.Outcome = "SUCCESS";
                    movement.MovementDateClose = DateTime.Now;
                    RegisterCompletedTransaction();
                    break;
                }
                else
                {
                    RegisterPartialTransaction();
                }
            }
        }

        private int InsertMoney()
        {
            // Simulate money insertion
            // In a real application, this would interface with hardware
            Console.Write("Enter the amount inserted (in cents): ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int amountInserted))
            {
                return amountInserted;
            }
            else
            {
                Console.WriteLine("Invalid amount. Please try again.");
                return 0;
            }
        }

        private void DisableMoneyInput()
        {
            Console.WriteLine("Disabling money input.");
            // In a real application, disable hardware input here
        }

        private void DispenseChange(int change)
        {
            Console.WriteLine($"Dispensing change: {change / 100:C}");
            movement.Change = change;
            // In a real application, dispense the change here
        }

        private void RegisterPartialTransaction()
        {
            Console.WriteLine("Registering partial transaction.");
            movement.Outcome = "PARTIAL";
            // Register partial transaction in a real application
        }

        private void RegisterCompletedTransaction()
        {
            Console.WriteLine("Registering completed transaction.");
            // Register completed transaction in a real application
        }
    }
}
