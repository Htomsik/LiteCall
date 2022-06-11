using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCall.Model
{
    public class VoiceMessage
    {
        public string Name { get; set; }

        public byte[] AudioByteArray { get; set; }
    }
}
