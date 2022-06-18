using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores.ModelStores
{
    internal class StatusMessageStore:BaseVMD
    {
        private StatusMessage? _CurentStatuMessage;

        public StatusMessage? CurentStatusMessage
        {
            get => _CurentStatuMessage;
            set
            {
                Set(ref _CurentStatuMessage, value);
                OnCurentStatusMessageChanged();
            }
        }

        public bool IsOpen =>  !string.IsNullOrEmpty(CurentStatusMessage?.Message);

        public event Action CurentStatusMessageChanged;

        private void OnCurentStatusMessageChanged()
        {
            CurentStatusMessageChanged?.Invoke();
        }
    }
}
