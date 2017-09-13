// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using Microsoft.AspNet.SignalR.Messaging;

namespace Microsoft.AspNet.SignalR
{
    /// <summary>
    /// Settings for the SQL Server scale-out message bus implementation.
    /// </summary>
    public class SqlScaleoutConfiguration : ScaleoutConfiguration
    {
        private int _tableCount = 1;
        private bool _executeDefaultSignalGCLogic = true;

        public SqlScaleoutConfiguration(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            ConnectionString = connectionString;
        }

        /// <summary>
        /// The SQL Server connection string to use.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// The number of tables to store messages in. Using more tables reduces lock contention and may increase throughput.
        /// This must be consistent between all nodes in the web farm.
        /// Defaults to 1.
        /// </summary>
        public int TableCount
        {
            get
            {
                return _tableCount;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _tableCount = value;
            }
        }

        /// <summary>
        /// GC is an operation that is executed with each newely inserted message. 
        /// In case the number of rows in the message table is greater than the max row count (10000 by default) then the GC will be executed.
        /// The GC logic is implemented on the send.sql file.
        /// If you want to use your GC implementation set the value to False
        /// Defaults to True
        /// </summary>
        public bool ExecuteDefaultSignalGCLogic
        {
            get
            {
                return _executeDefaultSignalGCLogic;
            }
            set
            {
                _executeDefaultSignalGCLogic = value;
            }
        }
    }
}
