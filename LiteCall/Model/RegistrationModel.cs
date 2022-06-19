using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCall.Model
{
    public class RegistrationModel: RecoveryModel
    {
        public string Captcha { get; set; }

        
    }

    public class RecoveryModel
    {

        public  Reg_Rec_PasswordAccount recoveryAccount { get; set; }

        public Question Question { get; set; }

        public string QestionAnswer { get; set; }

    }

}
