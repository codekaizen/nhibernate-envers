﻿using Iesi.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.RevisionInfo
{
	/// <summary>
	/// Automatically adds entity names changed during current revision.
	/// <see cref="ModifiedEntityTypesAttribute"/>
	/// <see cref="DefaultTrackingModifiedTypesRevisionEntity"/>
	/// </summary>
	public class DefaultTrackingModifiedTypesRevisionInfoGenerator : DefaultRevisionInfoGenerator
	{
		private readonly ISetter modifiedEntityTypesSetter;
		private readonly IGetter modifiedEntityTypesGetter;

		public DefaultTrackingModifiedTypesRevisionInfoGenerator(string revisionInfoEntityName, 
																		System.Type revisionInfoType, 
																		IRevisionListener revisionListener, 
																		PropertyData revisionInfoTimestampData, 
																		bool timestampAsDate,
																		PropertyData modifiedEntityNamesData) 
							: base(revisionInfoEntityName, revisionInfoType, revisionListener, revisionInfoTimestampData, timestampAsDate)
		{
			modifiedEntityTypesGetter = ReflectionTools.GetGetter(revisionInfoType, modifiedEntityNamesData);
			modifiedEntityTypesSetter = ReflectionTools.GetSetter(revisionInfoType, modifiedEntityNamesData);
		}

		public override void EntityChanged(System.Type entityClass, string entityName, object entityId, RevisionType revisionType, object revisionEntity)
		{
			base.EntityChanged(entityClass, entityName, entityId, revisionType, revisionEntity);
			var modifiedEntityNames = (ISet<string>)modifiedEntityTypesGetter.Get(revisionEntity);
			if (modifiedEntityNames == null)
			{
				modifiedEntityNames = new HashedSet<string>();
				modifiedEntityTypesSetter.Set(revisionEntity, modifiedEntityNames);
			}
			modifiedEntityNames.Add(entityName);
		}
	}
}