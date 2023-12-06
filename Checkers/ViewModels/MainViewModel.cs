using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Checkers.Commands;
using Checkers.Views;
using CheckersBase;
using Checkers.Models.Players;
using Checkers.Models;
using CheckersBase.BrainBase;

namespace Checkers.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private BoardViewModel _boardViewModel;
        private HistoryViewModel _historyViewModel;

        private PlayerViewModel _whitePlayer;
        private PlayerViewModel _blackPlayer;
        public PlayerViewModel WhitePlayer
        {
            get { return _whitePlayer; }
            set { _whitePlayer = value; OnPropertyChanged(() => WhitePlayer); }
        }

        public PlayerViewModel BlackPlayer
        {
            get { return _blackPlayer; }
            set { _blackPlayer = value; OnPropertyChanged(() => BlackPlayer); }
        }

        public BoardViewModel BoardVM
        {
            get { return _boardViewModel; }
            set { _boardViewModel = value; OnPropertyChanged(() => BoardVM); }
        }

        public HistoryViewModel HistoryVM
        {
            get { return _historyViewModel; }
            set { _historyViewModel = value; OnPropertyChanged(() => HistoryVM); }
        }

        public MainViewModel()
        {
        }
        #region Команды

        #region NewGameCommand
        private DelegateCommand _newGameCommand;

        public ICommand NewGameCommand
        {
            get
            {
                if (_newGameCommand == null)
                    _newGameCommand = new DelegateCommand(StartNewGame, CanStartNewGame);
                return _newGameCommand;
            }
        } 
        #endregion

        #region SettingsCommand
        private DelegateCommand _settingsCommand;
        

        public ICommand SettingsCommand
        {
            get
            {
                if (_settingsCommand == null)
                    _settingsCommand = new DelegateCommand(OpenSettings);
                return _settingsCommand;
            }
        }
        #endregion


        #region ChooseBlackPlayerCommand
        private DelegateCommand _choosePlayersCommand;
        public ICommand ChoosePlayersCommand
        {
            get
            {
                if (_choosePlayersCommand == null)
                    _choosePlayersCommand = new DelegateCommand(ChoosePlayers);
                return _choosePlayersCommand;
            }
        }

        private void ChoosePlayers()
        {
            var playersSelectViewModel = new PlayersSelectViewModel();
            playersSelectViewModel.SelectedBlack = this.BlackPlayer;
            playersSelectViewModel.SelectedWhite = this.WhitePlayer;
            var playersSelectWindow = new PlayerSelectWindow();
            playersSelectWindow.DataContext = playersSelectViewModel;
            playersSelectWindow.ShowDialog();
            if (playersSelectWindow.DialogResult == true)
            {
                WhitePlayer = playersSelectViewModel.SelectedWhite;
                BlackPlayer= playersSelectViewModel.SelectedBlack;
            }
        }
        #endregion

        private void OpenSettings()
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.DataContext = new SettingsViewModel();
            settingsWindow.ShowDialog();
        }

        public Game _game;
        
        private void StartNewGame()
        {
            if (WhitePlayer == null || BlackPlayer == null)
            {
                ChoosePlayers();
                if (WhitePlayer == null || BlackPlayer == null)
                {
                    return;
                }
            }
            if (_game != null)
            {
                // Удаляем старую игру. Если этого не сделать, старая игра будет работать, обрабатывая события доски и все остальное
                _game.Dispose();
            }
            _game = new Game(WhitePlayer.Player, BlackPlayer.Player);
	        _game.OnGameOver += (sender, args) =>
	        {
		        MessageBox.Show(args.ToString());
                Rating.Instance.SaveChanges();
	        };
            BoardVM = _game.BoardViewModel;
            HistoryVM = new HistoryViewModel(new GameHistory());

            _game.HistoryViewModel = HistoryVM; 
            _game.Start();

        }

        private bool CanStartNewGame()
        {
            return true;
        }


		#region StartTournamentCommand
		private DelegateCommand _startTournamentCommand;

		public DelegateCommand StartTournamentCommand
		{
			get
			{
				if (_startTournamentCommand == null)
					_startTournamentCommand = new DelegateCommand(StartTournament);
				return _startTournamentCommand;
			}
		}

		private void StartTournament()
		{
			TournamentSettingsViewModel tournamentSettingsVM = new TournamentSettingsViewModel();
			TournamentSettignsWindow sett = new TournamentSettignsWindow();
			sett.DataContext = tournamentSettingsVM;
			sett.ShowDialog();
			if (sett.DialogResult == true)
			{
				TournamentViewModel tournamentVM = new TournamentViewModel();
				tournamentVM.TournamentSettingsVM = tournamentSettingsVM;
				var tournamentWindow = new TournamentWindow();
				tournamentWindow.DataContext = tournamentVM;
				tournamentVM.Start();
				tournamentWindow.ShowDialog();
				tournamentVM.Stop();
				
			}
		} 
		#endregion


	    #endregion
    }
}
