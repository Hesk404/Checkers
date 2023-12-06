using CheckersBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersRules
{
    static class Extenstions
    {
        public static bool StartsFrom<T>(this IList<T> list1, IList<T> list2)
        {
            if (list1.Count <= list2.Count)
                return false;

            for (int i = 0; i < list2.Count; i++)
            {
                if (!list1[i].Equals(list2[i]))
                    return false;
            }

            return true;
        }

    }

    /// <summary>
    ///  структура для формирования всех возможных вариантов боя шашек
    /// </summary>
    class BeatNode : IDisposable
    {
        private List<BeatNode> _children;
        public BeatNode Parent { get; set; }
        public Point Beated { get; set; }
        public Point Move { get; set; }
        public BeatNode()
        {
            _children = new List<BeatNode>();
        }

        public void AddChild(ref BeatNode node)
        {
            _children.Add(node);
            node.Parent = this;
        }

        public List<List<Point>> SplitToBranches()
        {
            List<List<Point>> ret = new List<List<Point>>();

            List<BeatNode> finalLeafs = new List<BeatNode>();

            FindFinalLeafs(this, ref finalLeafs);

            foreach(var leaf in finalLeafs)
            {
                ret.Add(ReconstructBranch(leaf));
            }
            
            return ret;
        }

        protected static void FindFinalLeafs(BeatNode node, ref List<BeatNode> finalLeafs)
        {
            foreach(var child in node._children)
            {
                if (child._children.Count == 0)
                    finalLeafs.Add(child);
                else
                    FindFinalLeafs(child, ref finalLeafs);
            }
        }

        protected List<Point> ReconstructBranch(BeatNode leaf)
        {
            List<Point> ret = new List<Point>();

            for (var iter = leaf; iter != null; iter = iter.Parent)
            {
                ret.Add(iter.Move);
            }
                
            ret.Reverse();

            return ret;
        }

        public bool BranchContaintsValue(Point beated)
        {
            for (var iter = this; iter != null && iter.Beated != null; iter = iter.Parent)
            {
                if (iter.Beated.Equals(beated))
                {
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            foreach (var child in _children)
                child.Dispose();
            
            _children.Clear();
            Beated = null;
        }

        public void FilterForKingKills()
        {
            bool hasLongSeries = false;
            bool hasShortSeries = false;

            foreach (var child in _children)
            {
                if (child._children.Count == 0)
                    hasShortSeries = true;

                if (child._children.Count > 0)
                    hasLongSeries = true;
            }

            if (hasLongSeries && hasShortSeries)
            {
                _children = _children.Where(n => n._children.Count > 0).ToList();

                foreach (var child in _children)
                    child.FilterForKingKills();
            }
            
        }

    }

}
