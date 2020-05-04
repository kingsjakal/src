﻿using FluentMigrator.Builders.Create.Table;
using Smi.Core.Domain.Gdpr;

namespace Smi.Data.Mapping.Builders.Gdpr
{
    /// <summary>
    /// Represents a GDPR consent entity builder
    /// </summary>
    public partial class GdprConsentBuilder : SmiEntityBuilder<GdprConsent>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(GdprConsent.Message)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}