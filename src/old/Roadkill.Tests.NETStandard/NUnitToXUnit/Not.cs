namespace Roadkill.Tests.NETStandard.NUnitToXUnit
{
	public class Not : Is
	{
		public bool IsNot { get; set; }

		public override bool Equals(object obj)
		{
			if (IsNot)
				return !obj.Equals(Item);

			return obj.Equals(Item);
		}

		public Is EqualTo(object item)
		{
			return new Is() { Item = item };
		}

		public Contains Contains(object item)
		{
			return new Contains()
			{
				TheItem = item,
				IsInverse = true
			};
		}
	}
}