using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.Models.Players;
using CheckersBase;

namespace Checkers.ViewModels
{
    internal class PlayerViewModel : ViewModelBase
    {
	    public BasePlayer Player { get; private set; }
        public PlayerViewModel(BasePlayer player)
        {
            Player = player;
        }

        public void UpdatePlayer(BasePlayer player)
        {
            this.Player = player;
            OnPropertyChanged(() => Name);
        }

        public string Name
        {
            get { return Player.Name; }
        }
    }
}
