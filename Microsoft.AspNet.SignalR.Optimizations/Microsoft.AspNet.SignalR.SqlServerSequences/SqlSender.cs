// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Messaging;

namespace Microsoft.AspNet.SignalR.SqlServerSequences
{
    internal class SqlSender
    {
        private readonly string _connectionString;
        private readonly string _insertDml;
        private readonly TraceSource _trace;
        private readonly IDbProviderFactory _dbProviderFactory;

        public SqlSender(string connectionString, string tableName, TraceSource traceSource, IDbProviderFactory dbProviderFactory, bool executeDefaultSignalGCLogic)
        {
            _connectionString = connectionString;
            _insertDml = BuildInsertString(tableName, executeDefaultSignalGCLogic);
            _trace = traceSource;
            _dbProviderFactory = dbProviderFactory;
        }

        private string BuildInsertString(string tableName, bool executeDefaultSignalGCLogic)
        {
            var insertDml = GetType().Assembly.StringResource("Microsoft.AspNet.SignalR.SqlServerSequences.send.sql");
            if (!executeDefaultSignalGCLogic)
            {
                insertDml = insertDml.Replace("DECLARE @ExecuteDefaultSignalGCLogic BIT = 1;", "DECLARE @ExecuteDefaultSignalGCLogic BIT = 0;");
            }

            return insertDml.Replace("[SignalR]", String.Format(CultureInfo.InvariantCulture, "[{0}]", SqlMessageBus.SchemaName))
                            .Replace("[Messages_0", String.Format(CultureInfo.InvariantCulture, "[{0}", tableName))
                            .Replace("[SEQ_Messages_0", String.Format(CultureInfo.InvariantCulture, "[SEQ_{0}", tableName));
        }

        public Task Send(IList<Message> messages)
        {
            if (messages == null || messages.Count == 0)
            {
                return TaskAsyncHelper.Empty;
            }

            var parameter = _dbProviderFactory.CreateParameter();
            parameter.ParameterName = "Payload";
            parameter.DbType = DbType.Binary;
            parameter.Value = SqlPayload.ToBytes(messages);

            var operation = new DbOperation(_connectionString, _insertDml, _trace, parameter);

            return operation.ExecuteNonQueryAsync();
        }
    }
}
