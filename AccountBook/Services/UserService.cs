using AccountBook.DataAccess;
using AccountBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccountBook.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Register(string userName, string password)
        {
            try
            {
                var userInDb = _context.Users.Where(x => x.Username.ToLower() == userName.ToLower()).FirstOrDefault();

                if (userInDb != null)
                {
                    return false;
                }
                var hmac = new HMACSHA512();
                var user = new User
                {
                    Username = userName,
                    Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                    PasswordSalt = hmac.Key
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public (User,bool) Login(string userName, string password)
        {
            var user = _context.Users.Where(x => x.Username == userName).FirstOrDefault();
            if (user != null)
            {
                var hmac = new HMACSHA512(user.PasswordSalt);
                var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computeHash.Length; i++)
                {
                    if (computeHash[i] != user.Password[i])
                    {
                        return (null, false);
                    }
                }

                return (user, true);
            }
            return (null, false);
        }

        public bool AddStore(string storeName, int userId)
        {
            var storeInDb = _context.Stores.Where(x => x.Name.ToLower() == storeName.ToLower() && x.UserId == userId).FirstOrDefault();

            if (storeInDb != null)
            {
                return false;
            }

            var store = new Store
            {
                Name = storeName,
                UserId = userId
            };
            _context.Stores.Add(store);
            _context.SaveChanges();
            return true;
        }

        public List<Store> ViewStores(int userId)
        {
            var userStores = _context.Stores.Where(x => x.UserId == userId).ToList();
            return userStores;
        }
    }
}
