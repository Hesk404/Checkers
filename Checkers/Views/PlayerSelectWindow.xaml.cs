using System;
using System.Windows;
using System.Windows.Media.TextFormatting;
using Checkers.ViewModels;

namespace Checkers.Views
{
    public partial class PlayerSelectWindow : Window
    {
        public PlayerSelectWindow()
        {
            this.DataContextChanged += (sender, args) =>
            {
                var iclosable = (this.DataContext) as IRequestCloseViewModel;
                if (iclosable != null)
                    iclosable.RequestClose += IclosableOnRequestClose;
            };
            InitializeComponent();
            
        }

        void IclosableOnRequestClose(object sender, EventArgs eventArgs)
        {
            DialogResult = (eventArgs as DialogClosedEventArgs).DialogOK;
            Close();
        }
    }
}
