using Abunemer_Project_2;
using Microsoft.Win32;
using Abunemer_Project_2.Models;
using static Abunemer_Project_2.Services.WalletManager;
using static Abunemer_Project_2.Services.IBusinessLogicServic;
using System;
using System.Linq;
using System.Linq.Expressions;
using Abunemer_Project_2.Services;

Storage storage = Storage.GetInstance();
WalletManager walletManager = new WalletManager(storage);
IBusinessLogicServic service = new BusinessLogicServic();

while (true)
{
    Console.ForegroundColor = ConsoleColor.White;
    var activeUser = storage.GetActiveUser();  // Get the active user from the storage
    Console.Clear();
    Console.WriteLine("Main menu options:");

    if (activeUser == null)
    {
        Console.WriteLine("Nobody is logged in!" + Environment.NewLine);
        Console.WriteLine("1 > Register");
        Console.WriteLine("2 > Login");
    }
    else
    {
        Console.WriteLine($"Hello: {activeUser.Email}");
        Console.WriteLine("1 > Create Wallet");
        Console.WriteLine("2 > Show My Wallets");
        Console.WriteLine("3 > Test Operations");
        Console.WriteLine("4 > Check Statistics");
        Console.WriteLine("5 > Logout");
        Console.WriteLine("6 > Delete Wallet");
        Console.WriteLine("7 > Choose Wallet");
    }
    Console.WriteLine("666 > Quit");
    Console.WriteLine(Environment.NewLine + "Choose menu: ");
    var userInput = Console.ReadLine();

    if (int.TryParse(userInput, out var output))
    {
        switch (output)
        {
            case 1:
                if (activeUser == null)
                {
                    Register(service);
                }
                else
                {
                    CreateWallet(walletManager);
                }
                break;

            case 2:
                if (activeUser == null)
                {
                    Login(service, storage);
                }
                else
                {
                    ShowWallets(walletManager);
                }
                break;

            case 3:
                if (activeUser != null)
                {
                    TestOperations(walletManager, storage);
                }
                else
                {
                    Console.WriteLine("You must log in first!");
                    Console.ReadLine();
                }
                break;

            case 4:
                if (activeUser != null)
                {
                    CheckStatistics(walletManager); 
                }
                else
                {
                    Console.WriteLine("You must log in first!");
                    Console.ReadLine();
                }
                break;

            case 5:
                if (activeUser != null)
                {
                    storage.LogOut();  // Ensure active user is logged out and set to null
                    Console.WriteLine("You have been logged out.");
                }
                else
                {
                    Console.WriteLine("Invalid option!");
                }
                Console.ReadLine();
                break;
            case 6:
                deleteWallet(walletManager);
                break;
            case 7:
                ChooseWallet(walletManager);
                break;
            case 666:
                return;

            default:
                Console.WriteLine("Invalid option. Please try again.");
                Console.ReadLine();
                break;
        }
    }
    else
    {
        Console.WriteLine("You must type a number only!");
        Console.ReadLine();
    }
}

static void Login(IBusinessLogicServic service, Storage storage)
{
    Console.Clear();
    Console.WriteLine("Type your email: ");
    var email = Console.ReadLine();
    Console.WriteLine("Type your password: ");
    var password = Console.ReadLine();
    var result = service.LogIn(email, password);
    Console.ForegroundColor = result.Item1 ? ConsoleColor.Green : ConsoleColor.Red;
    Console.WriteLine(result.Item2);
    Console.ForegroundColor = ConsoleColor.White;

    // If login is successful, set the active user in the storage
    if (result.Item1)
    {
        var activeUser = storage.GetActiveUser();
        Console.WriteLine($"Welcome, {activeUser.Email}");
    }
    Console.ReadLine();
}

static void Register(IBusinessLogicServic service)
{
    Console.Clear();
    Console.WriteLine("Type your email: ");
    var email = Console.ReadLine();
    Console.WriteLine("Type your password: ");
    var password = Console.ReadLine();
    Console.WriteLine("Type your birthdate in format 'dd.mm.yyyy': ");
    var birthDate = Console.ReadLine();
    Console.WriteLine("Type your Name: ");
    var name = Console.ReadLine();
    var result = service.Register(email, password, birthDate, name);
    Console.ForegroundColor = result.Item1 ? ConsoleColor.Green : ConsoleColor.Red;
    Console.WriteLine(result.Item2);
    Console.ForegroundColor = ConsoleColor.White;
    Console.ReadLine();
}

static void CreateWallet(WalletManager walletManager)
{  
    Console.Clear();
    Console.WriteLine("Type wallet name: ");
    var name = Console.ReadLine();
    Console.WriteLine("Enter currency ($, EUR, RUB): ");
    var cur = Console.ReadLine();
    Console.WriteLine("Enter initial amount in form of (Sing ,Int, Fractional, Currency  " +
        "(e.g., +100.00 or -50.00$ or EUR ot RUB)");
    var amount = Console.ReadLine();

    var startAmount = new Money(amount);  
    if (cur != startAmount.GetCurrencySymbol())
    {
        throw new ArgumentException("The currency of your money" +
            " is not the same as the currency wallet");
    }
    try
    {
       // var startAmount = new Money(amount); 
        walletManager.CreateWallet(name, startAmount, cur);  // Link wallet to active user
        Console.WriteLine("Wallet created successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating wallet: {ex.Message}");
    }
    Console.ReadLine();
}
static void deleteWallet(WalletManager wm )
{
    Console.Clear();
    Console.WriteLine("Type wallet name: ");
    var name = Console.ReadLine();
    wm.DeleteWallet(name);
    Console.WriteLine("Wallet deleted successfully!");
}
static void ChooseWallet(WalletManager wm)
{
    Console.Clear();
    Console.WriteLine("Type wallet name: ");
    var name = Console.ReadLine();
    wm.ChooseWallet(name);
    //Console.WriteLine($"Wallet {name} is now the active wallet!");
}

static void ShowWallets(WalletManager walletManager)
{
    Console.Clear();
    var wallets = walletManager.GetAllWallets();

    if (wallets == null || !wallets.Any())
    {
        Console.WriteLine("No wallets available. Create one first.");
    }
    else
    {
        Console.WriteLine("Your wallets:");
        foreach (var wallet in wallets)
        {
            // Display wallet balance and currency correctly
            Console.WriteLine($"=> {wallet.Name} {wallet.balance.Display()}");
        }
    }

    Console.ReadLine();
}

static void TestOperations(WalletManager walletManager, Storage storage)
{
    Console.Clear();
    Console.WriteLine("Choose a wallet to operate on:");
    var wallets = walletManager.GetAllWallets();
    // List wallets for operation selection
    foreach (var wallet in wallets)
    {
        Console.WriteLine($"=> {wallet.Name} {wallet.balance.Display()}");
    }

    Console.WriteLine("Enter wallet name to choose:");
    var walletName = Console.ReadLine();
    walletManager.ChooseWallet(walletName);  // Set the selected wallet as the active wallet

    var activeWallet = storage.GetActiveWallet();
    //i did not write this condition in the sequences diagram 
    if (activeWallet == null)
    {
        Console.WriteLine("Invalid wallet name.");
        return;
    }

    Console.WriteLine("Choose operation type:");
    Console.WriteLine("1 > Add Income");
    Console.WriteLine("2 > Add Expense");
    Console.WriteLine("0 > Go Back");
    var userInput = Console.ReadLine();

    if (int.TryParse(userInput, out var output))
    {
        switch (output)
        {
            case 1:
                Console.WriteLine("Type value: ");
                var value = Console.ReadLine();
                Console.WriteLine("Choose category (1 > Salary, 2 > Scholarship, 3 > Other): ");
                var categoryIncome = Console.ReadLine();
                Console.WriteLine("Enter date (dd.mm.yyyy): ");
                var dateIncome = Console.ReadLine();
                if (!(DateTime.TryParseExact(dateIncome, "dd.MM.yyyy", null,
                    System.Globalization.DateTimeStyles.None, out DateTime date1)))
                {
                    Console.WriteLine("Invalid date format.");
                }
                walletManager.AddOperation(new Income(), new Money(value),
                    Convert.ToInt32(categoryIncome), Convert.ToDateTime(dateIncome));  // Link operation to selected wallet
                break;

            case 2:
                Console.WriteLine("Type value: ");
                var expenseValue = Console.ReadLine();
                Console.WriteLine("Choose category (1 > Food, 2 > Restaurants, ...): ");
                var categoryExpense = Console.ReadLine();
                Console.WriteLine("Enter date (dd.mm.yyyy): ");
                var dateExpense = Console.ReadLine();
                if (!(DateTime.TryParseExact(dateExpense, "dd.MM.yyyy", null,
                    System.Globalization.DateTimeStyles.None, out DateTime date2)))
                {
                    Console.WriteLine("Invalid date format.");
                }
               
                walletManager.AddOperation(new Expense(), new Money(expenseValue),
                    Convert.ToInt32(categoryExpense), Convert.ToDateTime(dateExpense));  // Link operation to selected wallet
                break;

            case 0:
                return;

            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

}

static void CheckStatistics(WalletManager walletManager)
{
    Console.Clear();
    Console.WriteLine("Enter start date (dd.mm.yyyy): ");
    var startDate = Console.ReadLine();
    Console.WriteLine("Enter end date (dd.mm.yyyy): ");
    var endDate = Console.ReadLine();

    if (DateTime.TryParseExact(startDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime from) &&
        DateTime.TryParseExact(endDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime to))
    {
        var statistics = walletManager.collectStatistics(from, to); 
        Console.WriteLine($"{statistics}");
    }
    else
    {
        Console.WriteLine("Invalid date format.");
    }

    Console.ReadLine();
}


