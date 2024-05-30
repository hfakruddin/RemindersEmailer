using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemindersEmailer.Data
{
    public class Reminder
    {
        public int ReminderId { get; set; }
        public string Title { get; set; }
        public DateTime ReminderDateTime { get; set; }
        public bool EmailSent { get; set; }
    }
}
