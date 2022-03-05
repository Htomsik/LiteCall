﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCall.Model
{
    public class Message
    {
        public string User { get; set; }
        public string text { get; set; }
        public DateTime DateSend { get; set; }

        public bool Type { get; set; }

        public Message CreateMs(string _Sender, string _text, DateTime _DateSend)
        {
            return new Message
            {
                text = _text,
                User = _Sender,
                DateSend = _DateSend
            };
        }
    }

    
}