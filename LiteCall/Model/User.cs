using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCall.Model
{
    public class Account
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public bool Type { get; set; }

    }

    public class ServerUser
    {
        public string Login;
    }
}
