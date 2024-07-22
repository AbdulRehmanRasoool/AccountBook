using AccountBook.DataAccess;
using AccountBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBook.Services
{
    public class JournalService
    {
        private readonly ApplicationDbContext _context;
        public JournalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddJournal(JournalEntry journal)
        {
            try
            {
                _context.Add(journal);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<JournalEntry> GetJournals(int storeId) 
        {
            var journals = _context.JournalEntries.Where(x => x.StoreId == storeId).Include(x => x.DebitEntries).Include(x => x.CreditEntries).ToList();
            return journals;
        }

        public bool ValidateAccounts(List<DebitEntry> debitEntries, List<CreditEntry> creditEntries)
        {
            var isEqual = debitEntries.Sum(x => x.Amount) == creditEntries.Sum(x => x.Amount);
            return isEqual;
        }
    }
}
