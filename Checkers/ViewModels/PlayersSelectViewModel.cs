using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Checkers.Commands;
using Checkers.Models;
using Checkers.Models.Players;
using Checkers.Properties;
using CheckersBase.BrainBase;

namespace Checkers.ViewModels
{
    class PlayersSelectViewModel : ViewModelBase, IRequestCloseViewModel
    {
        public PlayerViewModel SelectedWhite
        {
            get { return _selectedWhite; }
            set { _selectedWhite = value; OnPropertyChanged(() => SelectedWhite); }
        }

        public PlayerViewModel SelectedBlack
        {
            get { return _selectedBlack; }
            set { _selectedBlack = value; OnPropertyChanged(() => SelectedBlack); }
        }

        ObservableCollection<BrainViewModel> _bots;
        public ObservableCollection<BrainViewModel> Bots
        {
            get { return _bots; }
            set { _bots = value; OnPropertyChanged(() => Bots); }
        }
        public PlayersSelectViewModel()
        {
            Bots = BrainLoader.LoadBrains();
        }


        #region IRequestCloseViewModel
        public event EventHandler RequestClose;
        private void RaiseRequestClose(bool dialogResult)
        {
            if (RequestClose != null)
                RequestClose(this, new DialogClosedEventArgs(dialogResult)); // Execute
        } 
        #endregion


        #region Commands
        #region SelectBlackAIPlayerCommand
        DelegateCommand<BrainViewModel> _selectBlackAIPlayerCommand;
        public DelegateCommand<BrainViewModel> SelectBlackAIPlayerCommand
        {
            get
            {
                if (_selectBlackAIPlayerCommand == null)
                    _selectBlackAIPlayerCommand = new DelegateCommand<BrainViewModel>(SelectBlackAIPlayer);
                return _selectBlackAIPlayerCommand;
            }
        }

        private void SelectBlackAIPlayer(BrainViewModel brainVM)
        {
            if (brainVM == null) return;
            SelectedBlack = new PlayerViewModel(new AIPlayer(brainVM.Brain));
        }
        #endregion

        #region SelectWhiteAIPlayerCommand
        DelegateCommand<BrainViewModel> _selectWhiteAIPlayerCommand;
        public DelegateCommand<BrainViewModel> SelectWhiteAIPlayerCommand
        {
            get
            {
                if (_selectWhiteAIPlayerCommand == null)
                    _selectWhiteAIPlayerCommand = new DelegateCommand<BrainViewModel>(SelectWhiteAIPlayer);
                return _selectWhiteAIPlayerCommand;
            }
        }

        private void SelectWhiteAIPlayer(BrainViewModel brainVM)
        {
            if (brainVM == null) return;
            SelectedWhite = new PlayerViewModel(new AIPlayer(brainVM.Brain));
        }
        #endregion

        #region SelectWhitePlayerHuman
        DelegateCommand _selectWhitePlayerHuman;
        public DelegateCommand SelectWhitePlayerHuman
        {
            get
            {
                if (_selectWhitePlayerHuman == null)
                    _selectWhitePlayerHuman = new DelegateCommand(() => SelectedWhite = new PlayerViewModel(new HumanPlayer()));
                return _selectWhitePlayerHuman;
            }
        }
        #endregion

        #region SelectBlackPlayerHuman
        DelegateCommand _selectBlackPlayerHuman;
        PlayerViewModel _selectedWhite;
        PlayerViewModel _selectedBlack;

        public DelegateCommand SelectBlackPlayerHuman
        {
            get
            {
                if (_selectBlackPlayerHuman == null)
                    _selectBlackPlayerHuman = new DelegateCommand(() => SelectedBlack = new PlayerViewModel(new HumanPlayer()));
                return _selectBlackPlayerHuman;
            }
        }
        #endregion

        #region OKCommand
        DelegateCommand _OkCommand;
        public DelegateCommand OKCommand
        {
            get
            {
                if (_OkCommand == null)
                {
                    _OkCommand = new DelegateCommand(
                        () => RaiseRequestClose(true),
                        () => SelectedWhite != null && SelectedBlack != null
                        );
                }
                return _OkCommand;
            }


        }

        #endregion
        #region CancelCommand
        DelegateCommand _CancelCommand;
        public DelegateCommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                {
                    _CancelCommand = new DelegateCommand(() => RaiseRequestClose(false));
                }
                return _CancelCommand;
            }


        } 
        #endregion

		#region FileDropCommand
		public DelegateCommand<DragEventArgs> FileDropCommand
		{
			get
			{
				return new DelegateCommand<DragEventArgs>(OnFileDrop);
			}
		}

		private void OnFileDrop(DragEventArgs eventArgs)
		{
			if (eventArgs.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])eventArgs.Data.GetData(DataFormats.FileDrop);
				foreach (var file in files)
				{
					var newFileName = Settings.Default.BotFolder + Path.GetFileName(file);

					// TODO: Обработать ситуацию, когда такой файл уже есть в папке
					// TODO: Обработать ситуацию, когда мы копируем именно этот файл
					// TODO: Обработать ситуацию, когда проблемы с записью
					File.Copy(file, newFileName);

					var brains = BrainLoader.LoadBrainsFromDll(newFileName);
					foreach(var brain in brains)
						Bots.Add(new BrainViewModel(brain));
				}
			}
		} 
		#endregion

        #endregion

        
    }
}
