﻿using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.RevEntity.TrackModifiedEntities
{
	[TestFixture]
	public class EntityNamesTest : TestBase
	{
		protected override void Initialize()
		{
			var person1 = new Person {Name = "Hernan", Age = 28};
			var person2 = new Person {Name = "Leandro", Age = 29};
			var person3 = new Person {Name = "Barba", Age = 32};
			var person4 = new Person {Name = "Camomo", Age = 15};

			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				var owners = new List<Person>{person1, person2, person3};
				var car1 = new Car {Number = 5, Owners = owners};
				Session.Save(car1);
				tx.Commit();
			}

			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				var owners = new List<Person> { person2, person3, person4 };
				var car2 = new Car { Number = 27, Owners = owners };
				person1.Name = "Hernan David";
				person1.Age = 40;

				Session.Save(car2);
				tx.Commit();
			}
		}

		[Test]
		public void ShouldFindModifiedEntityTypes()
		{
			AuditReader().FindEntityTypesChangedInRevision(1)
				.Should().Have.SameValuesAs(typeof(Car), typeof(Person));

			AuditReader().FindEntityTypesChangedInRevision(2)
							.Should().Have.SameValuesAs(typeof(Car), typeof(Person));
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetProperty(ConfigurationKey.TrackEntitiesChangedInRevision, "true");
		}
	}
}