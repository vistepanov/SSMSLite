﻿using System;
using System.Collections.Generic;
using SsmsLite.Core.Ui.Search;
using SsmsLite.Core.Utils;

namespace SsmsLite.Core.Database.Entities.Search
{
    public class ConstraintSearchTarget : SearchTargetBase
    {
        public ConstraintSearchTarget(Persisted.DbObject dbObject)
        {
            DbObject = dbObject;
        }

        public Persisted.DbObject DbObject { get; }

        public override string UniqueIdentifier => Guid.NewGuid().ToString();

        public override string Type => DbObject.Type;

        public override string SchemaName => DbObject.SchemaName;

        public override string Name => DbObject.Name;

        public override DateTime? ModificationDate => DbObject.ModificationDate;

        public override string ModificationDateStr => DbObject.ModificationDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

        public override TextFragments RichName => new TextFragments(TextFragment.Primary(DbObject.Name));

        public override TextFragments RichSmallDefinition
        {
            get
            {
                var smallDef = DbObject.Definition?.Trim().Truncate(300).RemoveLineReturns();
                return new TextFragments(TextFragment.Primary(smallDef));
            }
        }

        public override TextFragments RichFullDefinition
        {
            get
            {
                return new TextFragments(TextFragment.Primary(DbObject.Definition));
            }
        }

        public override IReadOnlyCollection<string> DbRealtivePath()
        {
            var isCheckOrDefault = this.SqlObjectType == DbObjectType.CHECK_CONSTRAINT || this.SqlObjectType == DbObjectType.DEFAULT_CONSTRAINT;
            var folder = isCheckOrDefault ? "Constraints" : "Keys";
            var parent = DbObject.Parent;
            return new List<string>() { "UserTables", $"{parent.SchemaName}.{parent.Name}", folder, Name };
        }
    }
}
