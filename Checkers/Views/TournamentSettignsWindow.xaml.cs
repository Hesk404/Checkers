using System;
using System.Linq;
using System.Windows;
using Checkers.ViewModels;

namespace Checkers.Views
{
	public partial class TournamentSettignsWindow : Window
	{
		public TournamentSettignsWindow()
		{
			InitializeComponent();
			this.DataContextChanged += (sender, args) =>
			{
				var iclosable = (this.DataContext) as IRequestCloseViewModel;
				if (iclosable != null)
					iclosable.RequestClose += IclosableOnRequestClose;
			};
		}

		private void IclosableOnRequestClose(object sender, EventArgs e)
		{
			DialogResult = (e as DialogClosedEventArgs).DialogOK;
			Close();
		}

		private void btnMoveToSelected_Click(object sender, RoutedEventArgs e)
		{
			var selectedSource = dgSourceBots.SelectedItems.Cast<BrainViewModel>();
			var dc = this.DataContext as TournamentSettingsViewModel;
			foreach (var brainViewModel in selectedSource)
			{
				dc.SelectedBots.Add(brainViewModel);
			}

		}

		private void btnRemoveFromSelected_Click(object sender, RoutedEventArgs e)
		{
			var selectedSource = dgSelectedBots.SelectedItems.Cast<BrainViewModel>().ToList();
			
			var dc = this.DataContext as TournamentSettingsViewModel;
			
			foreach (var brainViewModel in selectedSource)
			{
				dc.SelectedBots.Remove(brainViewModel);
			}

		}
	}
}
