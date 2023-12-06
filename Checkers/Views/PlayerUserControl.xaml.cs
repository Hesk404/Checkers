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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Checkers.ViewModels;

namespace Checkers.Views
{
    /// <summary>
    /// Логика взаимодействия для PlayerUserControl.xaml
    /// </summary>
    public partial class PlayerUserControl : UserControl
    {
        public bool IsWhite { get; set; }

        public static readonly DependencyProperty IsWhiteProperty =
            DependencyProperty.Register(
                "IsWhite",
                typeof (bool),
                typeof (PlayerUserControl),
                new FrameworkPropertyMetadata(false)
                );
        public PlayerUserControl()
        {
            InitializeComponent();
        }
    }
}
