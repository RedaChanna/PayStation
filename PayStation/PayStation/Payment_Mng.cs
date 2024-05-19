using PayStationSW;
using System;
using System.Collections.Generic;
using System.IO.Ports;

public enum UserAction
{
    InsertCoin,
    InsertBill,
    InsertPOS
}

public class Payment_Management
{
  //  private PayStation paymentStation;
    private decimal amountToPay;
    private decimal amountInserted;
    
    /*
    
    public void PayAmount(decimal amountToPay)
    {

        paymentStation = Program.paymentStation;
        this.amountToPay = amountToPay;
        // Enable all devices when the PaymentStation is initialized
        //coinDevice.Enable();
        paymentStation.CashDevice.Enable();
        paymentStation.POSDevice.EnableDevice();
    }

    private UserAction GetUserAction()
    {
        Console.WriteLine("Select an action:");
        Console.WriteLine("1. Insert Coin");
        Console.WriteLine("2. Insert Bill");
        Console.WriteLine("3. Insert POS");
        string input = Console.ReadLine();

        switch (input)
        {
            case "1":
                return UserAction.InsertCoin;
            case "2":
                return UserAction.InsertBill;
            case "3":
                return UserAction.InsertPOS;
            default:
                Console.WriteLine("Invalid input. Please try again.");
                return GetUserAction(); // Recursively call to get a valid input
        }
    }
    private void HandleUserAction()
    {
        // Disable all devices at the beginning
        paymentStation?.Devices[DeviceEnum.Coin].Disable();
        paymentStation?.Devices[DeviceEnum.Cash].Disable();
        paymentStation?.Devices[DeviceEnum.Pos].Disable();

        var userAction = GetUserAction(); // Get user input

        if (userAction == UserAction.InsertCoin || userAction == UserAction.InsertBill)
        {
            // Enable both coin and cash devices
            //coinDevice.Enable();
            paymentStation?.Devices[DeviceEnum.Cash].Enable();

            while (amountInserted < amountToPay)
            {
                Console.WriteLine("Stand-by mode. Please insert a coin or a bill to proceed:");

                // Wait for user action and get feedback
                userAction = GetUserAction(); // You would implement this to get user input

                if (userAction == UserAction.InsertCoin || userAction == UserAction.InsertBill)
                {
                    // Handle the insertion and return the inserted amount (implement this)
                    decimal insertedAmount = HandleCoinOrBillInsertion();

                    // Update the amountInserted
                    amountInserted += insertedAmount;

                    // Check if the amountInserted has reached or exceeded the amountToPay
                    if (amountInserted >= amountToPay)
                    {
                        // Payment is complete
                        Console.WriteLine("Payment successful. Amount paid: " + amountInserted);
                        break; // Exit the while loop
                    }

                    // After handling insertion, disable the devices
                    //coinDevice.Disable();
                    paymentStation?.Devices[DeviceEnum.Cash].Disable();
                    paymentStation?.Devices[DeviceEnum.Pos].Disable();
                }
            }
        }
        else if (userAction == UserAction.InsertPOS)
        {
            // Disable coin and cash devices, enable POS, and perform POS payment
            //coinDevice.Disable();
            paymentStation?.Devices[DeviceEnum.Cash].Disable();
            paymentStation?.Devices[DeviceEnum.Pos].Enable();
            WaitForPOSPayment();
            // After completing POS payment, you can disable the POS device here
            paymentStation?.Devices[DeviceEnum.Pos].Disable();
        }
    }
    private void WaitForCoinAndCashPayment()
    {
        while (amountInserted < amountToPay)
        {
            Console.WriteLine("Please insert more coins or bills to complete the payment.");
            // Wait for user action and handle coin or bill insertion (implement this logic)
            decimal insertedAmount = HandleCoinOrBillInsertion();
            amountInserted += insertedAmount;

            if (amountInserted >= amountToPay)
            {
                // Payment is complete
                Console.WriteLine("Payment successful. Amount paid: " + amountInserted);
                break; // Exit the loop
            }
        }

        // After completing the payment, you may want to return to stand-by mode
        //coinDevice.Disable();
        paymentStation?.Devices[DeviceEnum.Cash].Disable();
    }
    private void WaitForPOSPayment()
    {
        Console.WriteLine("Please complete the POS payment.");
        // Implement the logic for handling POS payment here

        // After completing the payment, you may want to return to stand-by mode
        //coinDevice.Disable();
        paymentStation?.Devices[DeviceEnum.Cash].Disable();
        // You can choose to disable the POS device here, or it can be handled separately
    }
    private decimal HandleCoinOrBillInsertion()
    {
        decimal insertedAmount = 0;
        insertedAmount = 5.0M; // Assuming 5 Euro bill
        return insertedAmount;
    }
    private void ReturnChange(decimal amountChange)
    {
        if (amountChange <= 0)
        {
            Console.WriteLine("No change to return.");
            return;
        }
        // Check for the presence of the RC module
        if (paymentStation.Devices[DeviceEnum.Cash].RCModule.IsDevicePresent)
        {
            // Determine the type of the RC module (type 1 or type 2)
            bool isType1 = paymentStation.Devices[DeviceEnum.Cash].IsRCModuleType1;
            bool isType2 = paymentStation.Devices[DeviceEnum.Cash].IsRCModuleType2;
            if (isType1)
            {
                int maxBillsOfType1 = paymentStation.Devices[DeviceEnum.Cash].RCModule.GetMaxBillsOfType1();
                int billsToReturnOfType1 = Math.Min(maxBillsOfType1, (int)(amountChange / 5.0m));
                if (billsToReturnOfType1 > 0)
                {
                    // Simulate returning bills using the RC module for type 1 (modify this logic)
                    Console.WriteLine($"Returning {billsToReturnOfType1} bills of 5 euro(s)");
                    amountChange -= billsToReturnOfType1 * 5.0m;
                }
            }
            else if (isType2)
            {
                int maxBillsOfType1 = paymentStation.Devices[DeviceEnum.Cash].RCModule.GetMaxBillsOfType1();
                int maxBillsOfType2 = paymentStation.Devices[DeviceEnum.Cash].RCModule.GetMaxBillsOfType2();
                int billsToReturnOfType2 = Math.Min(maxBillsOfType2, (int)(amountChange / 10.0m));
                // First, return using the largest denomination (10 euro bills)
                if (billsToReturnOfType2 > 0)
                {
                    Console.WriteLine($"Returning {billsToReturnOfType2} bills of 10 euro(s)");
                    amountChange -= billsToReturnOfType2 * 10.0m;
                }
                // If there's still change left and Type 2 allows for 5 euro bills, use them
                if (amountChange > 0 && maxBillsOfType1 > 0)
                {
                    int billsToReturnOfType1 = Math.Min(maxBillsOfType1, (int)(amountChange / 5.0m));
                    if (billsToReturnOfType1 > 0)
                    {
                        Console.WriteLine($"Returning {billsToReturnOfType1} bills of 5 euro(s)");
                        amountChange -= billsToReturnOfType1 * 5.0m;
                    }
                }
            }
        }
        if (amountChange > 0)
        {
            // If there is still change to return, return the remaining change in coins
            //coinDevice.ReturnCoin(amountChange);
            Console.WriteLine($"Returning change of {amountChange} euro(s) in coins");
        }
        // Register the operation and handle any remaining change
        RegisterOperationInDatabase();
        Console.WriteLine("Change Returned: " + (amountChange));
    }
    private void RegisterOperationInDatabase()
    {
        // Your logic to register the successful payment operation in the database
        Console.WriteLine("Operation Registered in Database");
    }
    private void RegisterPartialOperationInDatabase()
    {
        // Your logic to register a partial payment operation in the database
        Console.WriteLine("Partial Operation Registered in Database");
    }
    private void RegisterFailedOperationInDatabase(decimal changeAmount)
    {
        // Your logic to register a failed payment operation in the database
        Console.WriteLine($"Failed Operation Registered in Database. Change cannot be delivered, no device for RC is present. Amount to return: {changeAmount}");
    }
    private decimal GetInsertedAmount()
    {
        // Your logic to obtain the amount inserted (from user input or payment devices)
        // Replace with actual implementation
        return 0.0m;
    }


*/
}