using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Checkers.Models;
using Checkers.Models.Players;
using Checkers.Properties;

namespace Checkers.ViewModels
{
    class BrainSelectViewModel : ViewModelBase
    {
        ObservableCollection<BrainViewModel> _bots;

        public BasePlayer SelectedPlayer { get; set; }

        public ObservableCollection<BrainViewModel> Bots
        {
            get { return _bots; }
            set { _bots = value; OnPropertyChanged(() => Bots); }
        }

        public BrainSelectViewModel()
        {
            SelectedPlayer = new HumanPlayer();
            Bots = BrainLoader.LoadBrains();
        }
    }
}
