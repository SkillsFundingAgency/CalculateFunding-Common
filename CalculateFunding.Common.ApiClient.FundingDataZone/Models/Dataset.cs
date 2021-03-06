﻿using System;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.FundingDataZone.Models
{
    public class Dataset
    {
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Dataset code is used as the programatic identifier for this dataset. It links together multiple versions of this dataset
        /// </summary>
        public string DatasetCode { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Table name in SQL
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Display name of dataset to show the user in the UI
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Funding Stream ID this dataset is associated with, eg PSG, DSG, GAG
        /// </summary>
        public string FundingStreamId { get; set; }

        /// <summary>
        /// Grouping level
        /// </summary>
        public GroupingLevel GroupingLevel { get; set; }

        /// <summary>
        /// The originating system of data, eg ILR, APT
        /// </summary>
        public string OriginatingSystem { get; set; }

        /// <summary>
        /// Version information within the originating system. Either a time or snapshot ID from that system
        /// </summary>
        public string OriginatingSystemVersion { get; set; }

        /// <summary>
        /// The column name which contains the identifier within the dataset. eg UKPRN or ProviderUKPRN
        /// </summary>

        public string IdentifierColumnName { get; set; }

        /// <summary>
        /// The type of identifier for this dataset, eg LACode or UKPRN etc
        /// </summary>
        public IdentifierType IdentifierType { get; set; }

        /// <summary>
        /// The provider snapshot ID for the snapshot of provider which match this dataset
        /// </summary>
        public string ProviderSnapshotId { get; set; }

        /// <summary>
        /// The version of this dataset. Allows multiple datasets of the same DatasetCode
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Custom properties
        /// </summary>
        public IDictionary<string, string> Properties { get; set; }
    }
}
