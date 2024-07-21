using AccountBook.DataAccess;
using AccountBook.Services;
using Microsoft.EntityFrameworkCore;

var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Server=DESKTOP-R9G1F52;Database=AccountBookDB;TrustServerCertificate=True;Trusted_Connection=True;").Options;

int? isLoggedIn = null;

using (var context = new ApplicationDbContext(dbContextOptions))
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    var userService = new UserService(context);

    while (true)
    {
        if (isLoggedIn == null)
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
                    isLoggedIn = Login(userService);
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
            Console.WriteLine("1. Add Store");
            Console.WriteLine("2. View Stores");
            Console.WriteLine("3. Logout");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddStore(userService, (int)isLoggedIn);
                    break;
                case "2":
                    ViewStores(userService, (int)isLoggedIn);
                    break;
                case "3":
                    isLoggedIn = null;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
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
    var stores = userService.ViewStores(userId);
    if (stores.Any())
    {
        Console.WriteLine("Stores:");
        foreach (var store in stores)
        {
            Console.WriteLine($"- {store.Name}");
        }
    }
    else
    {
        Console.WriteLine("No stores found.");
    }
}