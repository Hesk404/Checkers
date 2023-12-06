using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CheckersBase.BrainBase;

namespace CheckersBase
{
	public static class BrainBaseExtensions
	{
		public static string GetName(this BrainBase.BrainBase brainBase)
		{
			if (brainBase == null) return String.Empty;
			var brainInfoAttr = brainBase.GetType().GetCustomAttribute(typeof(BrainInfoAttribute)) as BrainInfoAttribute;
			if (brainInfoAttr == null) return String.Empty;

			return brainInfoAttr.BrainName;
		}

		public static string GetStudent(this BrainBase.BrainBase brainBase)
		{
			if (brainBase == null) return String.Empty;
			var brainInfoAttr = brainBase.GetType().GetCustomAttribute(typeof(BrainInfoAttribute)) as BrainInfoAttribute;
			if (brainInfoAttr == null) return String.Empty;

			return brainInfoAttr.Student;
		}

		public static string GetStudentGroup(this BrainBase.BrainBase brainBase)
		{
			if (brainBase == null) return String.Empty;
			var brainInfoAttr = brainBase.GetType().GetCustomAttribute(typeof(BrainInfoAttribute)) as BrainInfoAttribute;
			if (brainInfoAttr == null) return String.Empty;

			return brainInfoAttr.StudentGroup;
		}
	}
}
