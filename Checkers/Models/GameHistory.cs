using System.Collections.ObjectModel;
using CheckersBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Models
{
    class GameHistory
    {
        public ObservableCollection<MotionInfo> Motions = new ObservableCollection<MotionInfo>();

        public void AddMotionInfo(MotionInfo mtnInfo)
        {
            Motions.Add(mtnInfo);
        }

        public void Save()
        {

        }

        public void Load()
        {

        }



    }
}
