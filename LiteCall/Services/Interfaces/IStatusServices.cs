using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;

namespace LiteCall.Services.Interfaces
{
    internal interface IStatusServices
    {
        public void ChangeStatus(StatusMessage newStatusMessage);

        public void DeleteStatus();
    }
}
