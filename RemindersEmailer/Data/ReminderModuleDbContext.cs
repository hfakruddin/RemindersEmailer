using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemindersEmailer.Data
{
    public class ReminderModuleDbContext: DbContext
    {
        public ReminderModuleDbContext(DbContextOptions<ReminderModuleDbContext> options): base(options) 
        {
            
        }
        public DbSet<EmailModel> Emailsetup { get; set; }
        public DbSet<Reminder> Reminder { get; set; }
    }
}
