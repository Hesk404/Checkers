using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersBase.BrainBase
{
    /// <summary>
    ///  Атрибут для индивидуализации мозга
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class BrainInfoAttribute : Attribute
    {
        public BrainInfoAttribute()
        {}
        public BrainInfoAttribute( string brainName )
        {
            BrainName = brainName;
        }
        public BrainInfoAttribute( string brainName ,string student )
            :this(brainName)
        {
            Student = student;
        }

        public BrainInfoAttribute( string brainName, string student, string studentGroup )
            : this( brainName, student )
        {
            StudentGroup = studentGroup;
        }

        public string BrainName { get; set; }
        public string Student { get; set; }
        public string StudentGroup { get; set; }
    }
}
