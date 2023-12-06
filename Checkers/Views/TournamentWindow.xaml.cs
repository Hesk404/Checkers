using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Checkers.Models;
using Checkers.ViewModels;

namespace Checkers.Views
{

	public partial class TournamentWindow : Window
	{
		internal TournamentSettingsViewModel TournamentSettings { get; set; }
		public TournamentWindow()
		{
			InitializeComponent();
		}

		private void TbxLog_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			tbxLog.ScrollToEnd();
		}
	}
}
