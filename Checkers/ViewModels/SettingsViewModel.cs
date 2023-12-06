using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            
        }

        public int BotTimeLimit
        {
            get
            {
                return Properties.Settings.Default.BotTimeLimit;
            }
            set
            {
                Properties.Settings.Default.BotTimeLimit = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(() => BotTimeLimit);
            }
        }
    }
}
