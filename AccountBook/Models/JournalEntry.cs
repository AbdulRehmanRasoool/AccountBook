using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBook.Models
{
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string Description { get; set; }
        [ForeignKey("Store")]
        public int StoreId { get; set; }
        public Store Store { get; set; }

        public ICollection<DebitEntry> DebitEntries { get; set; }
        public ICollection<CreditEntry> CreditEntries { get; set; }
    }
}
