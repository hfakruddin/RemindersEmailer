using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemindersEmailer.Data
{
    public class EmailModel
    {
        public int Id { get; set; }
        public string EmailHost { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public string HUName { get; set; }
        public string HUPassword { get; set; }
        public int Interval { get; set; } = 5;
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string LogFilePath { get; set; }   
        public string SenderName { get; set; }
    }

}
