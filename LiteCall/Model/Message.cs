using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCall.Model
{
    public class Message
    {
        public string User { get; set; }
        public string Info { get; set; }
        public DateTime Date { get; set; }

        public bool Type { get; set; }
    }
}
