using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCall.Model
{

    public class ServerUser
    {
        public string Login { get; set; }
    }

    public class Account:ServerUser
    {

        public string Password { get; set; }

        public string Token { get; set; }

        public bool Type { get; set; }

    }
}
