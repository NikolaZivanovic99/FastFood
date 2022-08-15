using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class EmailConfig
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port{ get; set; }
    }
}
