using System.Collections;

namespace Roadkill.Tests.NETStandard.NUnitToXUnit
{
	public class Is
	{
		public object Item { get; set; }

		public override bool Equals(object obj)
		{
			return obj.Equals(Item);
		}

		public static Not Not
		{
			get
			{
				return new Not() { IsNot = true };
			}
		}

		public static Is EqualTo(object item)
		{
			return new Is() { Item = item };
		}
	}
}