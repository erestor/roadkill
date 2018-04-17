using System.Collections.Generic;

namespace Roadkill.Core.Database.Schema
{
	public class PostgresSchema : SchemaBase
	{
		protected override IEnumerable<string> GetCreateStatements()
		{
			string sql = LoadFromResource("Roadkill.Core.Database.Schema.Postgres.Create.sql");
			return new string[] { sql };
		}

		protected override IEnumerable<string> GetDropStatements()
		{
			string sql = LoadFromResource("Roadkill.Core.Database.Schema.Postgres.Drop.sql");
			return new string[] { sql };
		}
	}
}
