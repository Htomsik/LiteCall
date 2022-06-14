using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LiteCall.Infrastructure.ValidationRule
{
    public class IpValidation : System.Windows.Controls.ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {



            var StringValue = value.ToString();


            char[] splitValue = { '.', ':' };

            var SplitArray = StringValue.Split(splitValue).ToList();

            if (SplitArray.Count != 2)
            {
                if (SplitArray.Count != 5) return new ValidationResult(false, "Incorrect Ip");
            }
            else
            {
                if(SplitArray[0].ToLower() != "localhost") return new ValidationResult(false, "Incorrect Ip");
            }

            int Port;

            try
            {
                Port = Convert.ToInt32(SplitArray[^1]);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Incorrect Port");
            }
            
            if (Port < 1 || Port > 65536) return new ValidationResult(false, "Incorrect Port");

            SplitArray.Remove(SplitArray.Last());

            if (SplitArray[0].ToLower() != "localhost")
            {
                foreach (var segment in SplitArray)
                {

                    if (Convert.ToInt32(segment) < 0 || Convert.ToInt32(segment) > 256) return new ValidationResult(false, "Incorrect Ip");
                }
            }
            
            return new ValidationResult(true, null);
        }
    }
}
