using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignlR
{
    public class Message
    {
        public string text { get; set; }
        public string Sender { get; set; }
        public DateTime DateSend { get; set; }

        public Message CreateMs(string _Sender, string _text, DateTime _DateSend)
        {
            return new Message
            {
                text = _text,
                Sender = _Sender,
                DateSend = _DateSend
            };
        }
    }
}
