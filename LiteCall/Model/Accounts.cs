using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCall.Model
{


    public class User
    {
        public string Login { get; set; }
    }


    public class ServerUser:User
    {
        public string Role { get; set; }
    }


    public class Reg_Rec_PasswordAccount : User
    {
        public string? Password { get; set; }
    }



    public class Account: Reg_Rec_PasswordAccount
    {
        
        public string CurrentServerLogin { get; set; }

        public string Role { get; set; }

        public string Token { get; set; }

        public bool IsAuthorise { get; set; }

    }

 

    
   
}
