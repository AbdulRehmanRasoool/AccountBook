using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBook.Models
{
    public class CreditEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal Amount { get; set; }

        [ForeignKey("Account")]
        public int AccountId { get; set; }
        public Account Account { get; set; }

        [ForeignKey("JournalEntry")]
        public int JournalEntryId { get; set; }
        public JournalEntry JournalEntry { get; set; }
    }
}
