using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.ViewModels.Base;
using Newtonsoft.Json;

namespace LiteCall.Model
{
    internal class Settings:BaseVMD
    {

        [JsonIgnore]
        private int _OutputDeviceId;

        public int OutputDeviceId
        {
            get => _OutputDeviceId;
            set
            {
                Set(ref _OutputDeviceId, value);
                OnCurrentSettingsChanged();
            }
        }

        [JsonIgnore]
        private int _CaptureDeviceId;

        public int CaptureDeviceId
        {
            get => _CaptureDeviceId;
            set
            {
                Set(ref _CaptureDeviceId, value);
                OnCurrentSettingsChanged();

            }
           
        }

       
        private void OnCurrentSettingsChanged()
        {
            CurrentsettingsChanged?.Invoke();
        }
    
        public event Action? CurrentsettingsChanged;


    }
}
