// <copyright file="CreateCommandDbConnectionExtension.cs" company="Sinbadsoft">
// Copyright (c) Chaker Nakhli 2010-2012
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at http://www.apache.org/licenses/LICENSE-2.0 Unless required by 
// applicable law or agreed to in writing, software distributed under the License
// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific language governing
// permissions and limitations under the License.
// </copyright>
// <author>Chaker Nakhli</author>
// <email>chaker.nakhli@sinbadsoft.com</email>
// <date>2012/02/19</date>
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;

using Sinbadsoft.Lib.Model.ToExpando;

namespace MonkeyOrm
{
    public static class CreateCommandDbConnectionExtension
    {
        public static IDbCommand CreateCommand(this IDbConnection connection, string text, object parameters = null, IDbTransaction transaction = null)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = text;
            command.AddPropertiesAsParameters(parameters);
            return command;
        }

        public static IDbCommand CreateDeleteCommand(this IDbConnection connection, string table, string where, object parameters = null, IDbTransaction transaction = null)
        {
            var text = string.Format(CultureInfo.InvariantCulture, "DELETE FROM {0} WHERE {1}", table, @where);
            return connection.CreateCommand(text, parameters, transaction);
        }

        public static IDbCommand CreateInsertCommand(this IDbConnection connection, string table, object values, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null, IDbTransaction transaction = null)
        {
            var keyedValues = (IDictionary<string, object>)values.ToExpando(whitelist: whitelist, blacklist: blacklist);
            if (keyedValues.Count <= 0)
            {
                throw new InvalidOperationException("Can't insert this object to the database: empty values set");
            }

            var keysBuffer = new StringBuilder();
            var valuesBuffer = new StringBuilder();
            var counter = 0;
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            foreach (var item in keyedValues)
            {
                keysBuffer.Append(item.Key).Append(',');
                var parameterName = string.Format(CultureInfo.InvariantCulture, "p{0}", counter++);
                valuesBuffer.AppendFormat("@{0},", parameterName);
                command.AddObjectParameter(parameterName, item.Value);
            }

            keysBuffer.Length = keysBuffer.Length - 1;
            valuesBuffer.Length = valuesBuffer.Length - 1;
            command.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", table, keysBuffer, valuesBuffer);
            return command;
        }

        public static IDbCommand CreateBatchInsertCommand(this IDbConnection connection, string table, IEnumerable<object> batch, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null, IDbTransaction transaction = null)
        {
            var whitelistList = whitelist == null ? null : whitelist.ToList();
            var blacklistList = blacklist == null ? null : blacklist.ToList();
            var expandos = new List<ExpandoObject>();
            var keys = new HashSet<string>();
            foreach (var expando in batch.Select(obj => obj.ToExpando(whitelist: whitelistList, blacklist: blacklistList)))
            {
                keys.UnionWith(((IDictionary<string, object>)expando).Keys);
                expandos.Add(expando);
            }

            var keysBuffer = new StringBuilder();
            foreach (var key in keys)
            {
                keysBuffer.Append(key).Append(',');
            }

            if (keys.Count == 0)
            {
                throw new ArgumentException("Nothing to insert: empty batch or empty batch elements");
            }

            var valuesBuffer = new StringBuilder();
            var counter = 0;
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            foreach (IDictionary<string, object> expando in expandos)
            {
                valuesBuffer.Append("(");
                foreach (var key in keys)
                {
                    object value;
                    if (expando.TryGetValue(key, out value))
                    {
                        var parameterName = string.Format(CultureInfo.InvariantCulture, "p{0}", counter++);
                        valuesBuffer.AppendFormat("@{0},", parameterName);
                        command.AddObjectParameter(parameterName, value);
                    }
                    else
                    {
                        valuesBuffer.Append("DEFAULT,");
                    }
                }

                valuesBuffer.Length = valuesBuffer.Length - 1;
                valuesBuffer.Append("),");
            }

            keysBuffer.Length = keysBuffer.Length - 1;
            valuesBuffer.Length = valuesBuffer.Length - 1;
            command.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES {2}", table, keysBuffer, valuesBuffer);
            return command;
        }

        public static IDbCommand CreateUpsertCommand(this IDbConnection connection, string table, object values, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null, IDbTransaction transaction = null)
        {
            var keyedValues = (IDictionary<string, object>)values.ToExpando(whitelist: whitelist, blacklist: blacklist);
            if (keyedValues.Count <= 0)
            {
                throw new InvalidOperationException("Cannot upsert: there are no values to insert or update");
            }

            var keysBuffer = new StringBuilder();
            var valuesBuffer = new StringBuilder();
            var updateBuffer = new StringBuilder();
            var counter = 0;
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            foreach (var item in keyedValues)
            {
                keysBuffer.Append(item.Key).Append(',');
                var parameterName = string.Format(CultureInfo.InvariantCulture, "p{0}", counter++);
                updateBuffer.AppendFormat("{0}=@{1}", item.Key, parameterName).Append(',');
                valuesBuffer.AppendFormat("@{0},", parameterName);
                command.AddObjectParameter(parameterName, item.Value);
            }

            keysBuffer.Length = keysBuffer.Length - 1;
            valuesBuffer.Length = valuesBuffer.Length - 1;
            updateBuffer.Length = updateBuffer.Length - 1;
            command.CommandText = string.Format(
                "INSERT INTO {0} ({1}) VALUES ({2}) ON DUPLICATE KEY UPDATE {3}",
                table,
                keysBuffer,
                valuesBuffer,
                updateBuffer);
            return command;
        }

        public static IDbCommand CreateUpdateCommand(this IDbConnection connection, string table, object values, string where, object parameters = null, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null, IDbTransaction transaction = null)
        {
            var keyedValues = (IDictionary<string, object>)values.ToExpando(whitelist: whitelist, blacklist: blacklist);
            if (keyedValues.Count <= 0)
            {
                throw new InvalidOperationException("Cannot update: there are no values to set");
            }

            var updateBuffer = new StringBuilder();
            var counter = 0;
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            foreach (var item in keyedValues)
            {
                var parameterName = string.Format(CultureInfo.InvariantCulture, "p{0}", counter++);
                updateBuffer.AppendFormat("{0}=@{1}", item.Key, parameterName).Append(',');
                command.AddObjectParameter(parameterName, item.Value);
            }

            updateBuffer.Length = updateBuffer.Length - 1;
            command.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2}", table, updateBuffer, where);
            command.AddPropertiesAsParameters(parameters);
            return command;
        }
    }
}