using System;

namespace LinqToDB.SqlQuery
{
	[Flags]
	public enum SetOperation
	{
		Union        = 0x01,
		UnionAll     = 0x02,
		Except       = 0x04,
		ExceptAll    = 0x08,
		Intersect    = 0x10,
		IntersectAll = 0x20,
	}
}
