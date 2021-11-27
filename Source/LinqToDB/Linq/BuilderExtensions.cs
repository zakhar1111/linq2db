using System.Reflection;

namespace LinqToDB.Linq
{
	using System;
	using System.Linq;
	using Common;
	using LinqToDB.Linq.Builder;
	using LinqToDB.Mapping;

	internal static class BuilderExtensions
	{
		internal static SqlInfo[] Clone(this SqlInfo[] sqlInfos, MemberInfo member)
		{
			if (sqlInfos.Length == 0)
				return Array<SqlInfo>.Empty;

			var sql = new SqlInfo[sqlInfos.Length];
			for (var i = 0; i < sql.Length; i++)
				sql[i] = sqlInfos[i].Clone(member);
			return sql;
		}

		internal static bool IsFSharpRecord(MappingSchema mappingSchema, Type objectType)
		{
			return IsFSharpRecord(mappingSchema.GetAttributes<Attribute>(objectType));
		}

		internal static bool IsFSharpRecord(MappingSchema mappingSchema, Type objectType, MemberInfo memberInfo)
		{
			return IsFSharpRecord(mappingSchema.GetAttributes<Attribute>(objectType, memberInfo));
		}

		internal static bool IsFSharpRecord(Attribute[] attrs)
		{
			return  attrs.Any(attr => attr.GetType().FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute")
				&& !attrs.Any(attr => attr.GetType().FullName == "Microsoft.FSharp.Core.CLIMutableAttribute");
		}

		internal static int GetFSharpRecordMemberSequence(MappingSchema mappingSchema, Type objectType, MemberInfo memberInfo)
		{
			var attrs = mappingSchema.GetAttributes<Attribute>(objectType, memberInfo);
			var compilationMappingAttr = attrs.FirstOrDefault(attr => attr.GetType().FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute");

			if (compilationMappingAttr != null)
			{
				// https://github.com/dotnet/fsharp/blob/1fcb351bb98fe361c7e70172ea51b5e6a4b52ee0/src/fsharp/FSharp.Core/prim-types.fsi
				// ObjectType = 3
				if (Convert.ToInt32(((dynamic)compilationMappingAttr).SourceConstructFlags) != 3)
					return ((dynamic)compilationMappingAttr).SequenceNumber;
			}

			return -1;
		}
	}
}
