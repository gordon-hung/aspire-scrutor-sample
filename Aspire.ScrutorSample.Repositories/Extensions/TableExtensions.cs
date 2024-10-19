namespace Aspire.ScrutorSample.Repositories.Extensions;

public static class TableExtensions
{
	public static string GetTableName(this Type type)
	{
		dynamic tableattr = type.GetCustomAttributes(false).SingleOrDefault(attr => attr.GetType().Name == "TableAttribute")!;
		var name = type.Name;

		if (tableattr != null)
		{
			name = tableattr.Name;
		}

		return name;
	}
}
