using System;

namespace Checkers.ViewModels
{
    public interface IRequestCloseViewModel
    {
        event EventHandler RequestClose;
    }

    public class DialogClosedEventArgs : EventArgs
    {
        public DialogClosedEventArgs(bool dialogOk)
        {
            this.DialogOK = dialogOk;
        }
        public bool DialogOK { get; set; }
    }
}
