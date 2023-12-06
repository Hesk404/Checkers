using System.Collections.ObjectModel;
using Checkers.Commands;
using Checkers.Models;
using CheckersBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Checkers.ViewModels
{
    class HistoryViewModel : ViewModelBase
    {
        GameHistory _history;

        public HistoryViewModel(GameHistory history)
        {
            _history = history;
        }

        public ObservableCollection<MotionInfo> Motions
        {
            get { return _history.Motions; } 
            set { _history.Motions = value; }
        } 

        public Board HistoryBoard {get; private set;}

        internal void AddMotion(MotionInfo mtn)
        {
            _history.AddMotionInfo(mtn);
            OnPropertyChanged(() => Motions);
        }
    }
}
