using Core.Models.Users;

namespace Core.Models.AccountManagement;


    public class RecoveryModel
    {
        public RegistrationUser RecoveryAccount { get; set; }

        public Question Question { get; set; }

        public string QuestionAnswer { get; set; }
    }
