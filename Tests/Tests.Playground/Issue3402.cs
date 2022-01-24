using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using LinqToDB.Mapping;
using NUnit.Framework;

namespace Tests.Playground
{
	[TestFixture]
	public class Issue3402 : TestBase
	{
		[Table(Name = "VEMPLOYEE_SCHEDULE_SECTIONS")]
		private class EmployeeScheduleSection
		{
			[Column(Name = "ACTIVE", CanBeNull = false)]
			public bool Active { get; set; }

			[Column(Name = "ID", CanBeNull = false, IsPrimaryKey = true)]
			public long ID { get; set; }

			[Column(Name = "NAME", CanBeNull = false)]
			public string Name { get; set; }

			[Association(ThisKey = "ID", OtherKey = "EmployeeScheduleSectionID")]
			public List<EmployeeScheduleSectionAdditionalPermission> AdditionalPermissions { get; set; }

			public EmployeeScheduleSection()
			{
				AdditionalPermissions = new List<EmployeeScheduleSectionAdditionalPermission>();
				Name = String.Empty;
			}
		}

		[Table(Name = "VEMPLOYEE_SCHDL_SEC_ADDTL_PERM")]
		private class EmployeeScheduleSectionAdditionalPermission
		{
			[Column(Name = "EMPLOYEE_SCHEDULE_SECTION_ID", CanBeNull = false, IsPrimaryKey = true)]
			public long EmployeeScheduleSectionID { get; set; }

			[Column(Name = "IS_ACTIVE", CanBeNull = false)]
			public bool IsActive { get; set; }
		}

		[Test]
		public void GoodTest([IncludeDataSources(TestProvName.AllSQLite)] string context)
		{
			using (var db = GetDataContext(context))
			using (var table1 = db.CreateLocalTable<EmployeeScheduleSection>())
			using (var table2 = db.CreateLocalTable<EmployeeScheduleSectionAdditionalPermission>())
			{


				bool fullAccess = false;

				var permissions = (
						from ess in table1
						let allowEdit = fullAccess || ess.AdditionalPermissions.Any(y => y.IsActive)
						where allowEdit
						select new
						{
							SectionID = ess.ID,
						}
			);

				var sql1 = permissions.ToString();
				var data1 = permissions.ToList();

			}
		}

		[Test]
		public void BrokenTest2([IncludeDataSources(TestProvName.AllSQLite)] string context)
		{
			using (var db = GetDataContext(context))
			using (var table1 = db.CreateLocalTable<EmployeeScheduleSection>())
			using (var table2 = db.CreateLocalTable<EmployeeScheduleSectionAdditionalPermission>())
			{

				bool fullAccess = true;

				var permissions = (
						from ess in table1
						//let allowEdit = fullAccess || ess.AdditionalPermissions.Any(y => y.IsActive)
						where ( fullAccess || ess.AdditionalPermissions.Any(y => y.IsActive))
						select new
						{
							SectionID = ess.ID,
						}
			);

				
				var sql1 = permissions.ToString();
				var data1 = permissions.ToList();

			}
		}

		[Test]
		public void BrokenTest3([IncludeDataSources(TestProvName.AllSQLite)] string context)
		{
			using (var db = GetDataContext(context))
			using (var table1 = db.CreateLocalTable<EmployeeScheduleSection>())
			using (var table2 = db.CreateLocalTable<EmployeeScheduleSectionAdditionalPermission>())
			{
				bool fullAccess = true;

				var permissions = table1.Where(ess => ( fullAccess || ess.AdditionalPermissions.Any(y => y.IsActive)) ).Select( n => new { SectionID = n.ID});

				var sql1 = permissions.ToString();
				var data1 = permissions.ToList();

			}
		}
		[Test]
		public void BrokenTest([IncludeDataSources(TestProvName.AllSQLite)] string context)
		{
			using (var db = GetDataContext(context))
			using (var table1 = db.CreateLocalTable<EmployeeScheduleSection>())
			using (var table2 = db.CreateLocalTable<EmployeeScheduleSectionAdditionalPermission>())
			{

				// error:
				bool fullAccess = true; //it seems that fail translate fullAccess constant

				var permissions = (
					from ess in table1
					let allowEdit = fullAccess || ess.AdditionalPermissions.Any(y => y.IsActive)
					where allowEdit
					select new
					{
						SectionID = ess.ID,
					}
				);

				var sql2 = permissions.ToString();
				var data2 = permissions.ToList(); // throws exception
			}
		}
	}
}
