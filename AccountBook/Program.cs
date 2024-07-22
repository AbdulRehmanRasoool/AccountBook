using AccountBook.DataAccess;
using AccountBook.Models;
using AccountBook.Services;
using Microsoft.EntityFrameworkCore;

var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Server=DESKTOP-SKSQHI2;Database=AccountBookDB;TrustServerCertificate=True;Trusted_Connection=True;").Options;

int? loggedInUserId = null;
int? selectedStoreId = null;

using (var context = new ApplicationDbContext(dbContextOptions))
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    var userService = new UserService(context);
    var journalService = new JournalService(context);

    while (true)
    {
        if (loggedInUserId == null)
        {
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Register(userService);
                    break;
                case "2":
                    loggedInUserId = Login(userService);
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        else
        {
            if (selectedStoreId == null)
            {
                Console.WriteLine("1. Add Store");
                Console.WriteLine("2. View Stores");
                Console.WriteLine("3. Select Store");
                Console.WriteLine("4. Logout");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddStore(userService, (int)loggedInUserId);
                        break;
                    case "2":
                        ViewStores(userService, (int)loggedInUserId);
                        break;
                    case "3":
                        selectedStoreId = SelectStore(userService, (int)loggedInUserId);
                        break;
                    case "4":
                        loggedInUserId = null;
                        selectedStoreId = null;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("1. Add Journal");
                Console.WriteLine("2. View Journals");
                Console.WriteLine("3. Logout");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddJournal(journalService, (int)selectedStoreId);
                        break;
                    case "2":
                        ViewJournals(journalService, (int)selectedStoreId);
                        break;
                    case "3":
                        selectedStoreId = null;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}

void Register(UserService userService)
{
    Console.WriteLine("Enter Username");
    var userName = Console.ReadLine();
    Console.WriteLine("Enter Password");
    var password = Console.ReadLine();

    if (userService.Register(userName,password))
    {
        Console.WriteLine("User Registerd Successfully");
    }
    else
    {
        Console.WriteLine("Username already taken");
    }
}

int? Login(UserService userService)
{
    Console.WriteLine("Enter Username");
    var userName = Console.ReadLine();
    Console.WriteLine("Enter Password");
    var password = Console.ReadLine();

    var isValidUser = userService.Login(userName, password);
    if (isValidUser.Item2)
    {
        Console.WriteLine("Login Successful");
        return isValidUser.Item1.Id;
    }
    else
    {
        Console.WriteLine("Invalid username or password");
        return null;
    }
}



void AddStore(UserService userService, int userId)
{
    Console.Write("Enter store name: ");
    var storeName = Console.ReadLine();

    if (userService.AddStore(storeName, userId))
    {
        Console.WriteLine("Store created successfully.");
    }
    else
    {
        Console.WriteLine("Failed to add store.");
    }
}

void ViewStores(UserService userService, int userId)
{
    var stores = userService.GetStores(userId);
    if (stores.Any())
    {
        Console.WriteLine("Stores:");
        foreach (var store in stores)
        {
            Console.WriteLine($"- {store.Id} : {store.Name}");
        }
    }
    else
    {
        Console.WriteLine("No stores found.");
    }
}

int? SelectStore(UserService userService, int userId)
{
    Console.WriteLine("Enter Store Id: ");
    var storeId = Convert.ToInt32(Console.ReadLine());
    var store = userService.GetStore(storeId,userId);
    if (store == null)
    {
        Console.WriteLine("Store not found");
        return null;
    }
    return storeId;
}

void AddJournal(JournalService journalService, int storeId)
{
    Console.Write("Enter description: ");
    var description = Console.ReadLine();
    Console.Write("Enter date (yyyy-mm-dd): ");
    if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
    {
        var debitEntries = new List<DebitEntry>();
        var creditEntries = new List<CreditEntry>();

        while (true)
        {
            Console.WriteLine("1. Add Debit Entry");
            Console.WriteLine("2. Add Credit Entry");
            Console.WriteLine("3. Finish Adding Entries");
            var entryChoice = Console.ReadLine();

            switch (entryChoice)
            {
                case "1":
                    AddDebitEntry(debitEntries);
                    break;
                case "2":
                    AddCreditEntry(creditEntries);
                    break;
                case "3":
                    if (journalService.ValidateAccounts(debitEntries, creditEntries))
                    {
                        var journal = new JournalEntry
                        {
                            Date = date,
                            Description = description,
                            StoreId = storeId,
                            DebitEntries = debitEntries,
                            CreditEntries = creditEntries
                        };

                        if (journalService.AddJournal(journal))
                        {
                            Console.WriteLine("Journal added successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to add journal.");
                        }
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Debits and credits do not balance. Please correct the entries.");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
    else
    {
        Console.WriteLine("Invalid date format. Please enter the date in yyyy-mm-dd format.");
    }
}

void AddDebitEntry(List<DebitEntry> debitEntries)
{

    Console.Write("Enter description: ");
    var description = Console.ReadLine();
    Console.Write("Enter amount: ");
    if (decimal.TryParse(Console.ReadLine(), out decimal amount))
    {
        Console.Write("Enter account ID: ");
        if (int.TryParse(Console.ReadLine(), out int accountId))
        {
            var debitEntry = new DebitEntry
            {
                Description = description,
                Amount = amount,
                AccountId = accountId
            };
            debitEntries.Add(debitEntry);
            Console.WriteLine("Debit entry added.");
        }
        else
        {
            Console.WriteLine("Invalid account ID. Please enter a valid integer.");
        }
    }
    else
    {
        Console.WriteLine("Invalid amount. Please enter a valid decimal number.");
    }
}

void AddCreditEntry(List<CreditEntry> creditEntries)
{
    Console.Write("Enter description: ");
    var description = Console.ReadLine();
    Console.Write("Enter amount: ");
    if (decimal.TryParse(Console.ReadLine(), out decimal amount))
    {
        Console.Write("Enter account ID: ");
        if (int.TryParse(Console.ReadLine(), out int accountId))
        {
            var creditEntry = new CreditEntry
            {
                Description = description,
                Amount = amount,
                AccountId = accountId
            };
            creditEntries.Add(creditEntry);
            Console.WriteLine("Credit entry added.");
        }
        else
        {
            Console.WriteLine("Invalid account ID. Please enter a valid integer.");
        }
    }
    else
    {
        Console.WriteLine("Invalid amount. Please enter a valid decimal number.");
    }
}

void ViewJournals(JournalService journalService, int storeId)
{
    var journals = journalService.GetJournals(storeId);
    if (journals.Any())
    {
        Console.WriteLine("Journals:");
        foreach (var journal in journals)
        {
            Console.WriteLine($"- {journal.Id}: {journal.Date.ToShortDateString()} - {journal.Description}");
            Console.WriteLine("  Debit Entries:");
            foreach (var debit in journal.DebitEntries)
            {
                Console.WriteLine($"    {debit.Id}: {debit.Description} - {debit.Amount:C} (Account: {debit.AccountId})");
            }
            Console.WriteLine("  Credit Entries:");
            foreach (var credit in journal.CreditEntries)
            {
                Console.WriteLine($"    {credit.Id}: {credit.Description} - {credit.Amount:C} (Account: {credit.AccountId})");
            }
        }
    }
    else
    {
        Console.WriteLine("No journals found for this store.");
    }
}