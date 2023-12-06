using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Checkers.Commands;
using Checkers.Models;
using Checkers.Properties;

namespace Checkers.ViewModels
{
	internal class TournamentSettingsViewModel : ViewModelBase, IRequestCloseViewModel
	{

		public ObservableCollection<BrainViewModel> Bots { get; set; }
		public ObservableCollection<BrainViewModel> SelectedBots { get; set; }

		public int NumberOfGamesPerFight { get; set; }

		public int MaxThreads { get; set; }

		public TournamentSettingsViewModel()
		{
			Bots = BrainLoader.LoadBrains();
            SelectedBots = new ObservableCollection<BrainViewModel>();
			NumberOfGamesPerFight = 2;
			MaxThreads = 3;


		}

		#region Commands

		#region StartTourmanentCommand
		private DelegateCommand _startTourmanentCommand;
		public DelegateCommand StartTourmanentCommand
		{
			get
			{
				return _startTourmanentCommand ?? (_startTourmanentCommand = new DelegateCommand(
					() => RaiseRequestClose(true),
					() => SelectedBots.Count >= 2
					));
			}
			set { _startTourmanentCommand = value; }
		} 
		#endregion

		#region CancelCommand
		private DelegateCommand _cancelCommand;
		public DelegateCommand CancelCommand
		{
			get
			{
				if (_cancelCommand == null)
					_cancelCommand = new DelegateCommand(() => RaiseRequestClose(false));
				return _cancelCommand;
			}
			set { _cancelCommand = value; }
		} 
		#endregion

		#endregion

		#region IRequestCloseViewModel
		public event EventHandler RequestClose;
		private void RaiseRequestClose(bool dialogResult)
		{
			if (RequestClose != null)
				RequestClose(this, new DialogClosedEventArgs(dialogResult)); // Execute
		}
		#endregion
	}
}
