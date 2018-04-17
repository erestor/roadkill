using System.Collections.Generic;

namespace Roadkill.Core.Database.Schema
{
	public class MySqlSchema : SchemaBase
	{
		protected override IEnumerable<string> GetCreateStatements()
		{
			string sql = LoadFromResource("Roadkill.Core.Database.Schema.MySql.Create.sql");
			return new string[] { sql };
		}

		protected override IEnumerable<string> GetDropStatements()
		{
			string sql = LoadFromResource("Roadkill.Core.Database.Schema.MySql.Drop.sql");
			return new string[] { sql };
		}
	}
}
