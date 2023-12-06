using Checkers.Models;
using CheckersBase;
using CheckersBase.BrainBase;

namespace Checkers.ViewModels
{
    internal class BrainViewModel : ViewModelBase
    {

        public BrainBase Brain { get; set; }

        public BrainStats Stats { get; private set; }

        public string Name
        {
            get { return Brain.GetName(); }
        }

        public string StudentFIO
        {
            get { return Brain.GetStudent(); }
        }

        public string StudentGroup
        {
            get { return Brain.GetStudentGroup(); }
        }

        public string Wins
        {
            get { return Stats.Wins.ToString(); }
        }

        public string Loses
        {
            get { return Stats.Loses.ToString(); }
        }

        public string Draws
        {
            get { return Stats.Draws.ToString(); }
        }

        public string WinRate
        {
            get { return Stats.WinRate; }
        }

        public BrainViewModel(BrainBase brainBase)
        {
            this.Brain = brainBase;
            this.Stats = Rating.Instance.GetBrainStats(brainBase);
        }
    }
}
