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
using System.Globalization;
using System.Text;

using Sinbadsoft.Lib.Model.ToExpando;

namespace MonkeyOrm
{
    public static class CreateCommandDbConnectionExtension
    {
        public static IDbCommand CreateCommand(this IDbConnection connection, string query, object parameters = null)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            command.AddPropertiesAsParameters(parameters);
            return command;
        }

        public static IDbCommand CreateDeleteCommand(this IDbConnection connection, string table, string where, object parameters = null)
        {
            var command = connection.CreateCommand();
            command.CommandText = string.Format(CultureInfo.InvariantCulture, "DELETE FROM {0} WHERE {1}", table, where);
            command.AddPropertiesAsParameters(parameters);
            return command;
        }

        public static IDbCommand CreateInsertCommand(this IDbConnection connection, string table, object values, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            var keyedValues = (IDictionary<string, object>)values.ToExpando(whitelist: whitelist, blacklist: blacklist);
            if (keyedValues.Count <= 0)
            {
                throw new InvalidOperationException("Can't insert this object to the database: empty values set");
            }

            var keysBuffer = new StringBuilder();
            var paramsBuffer = new StringBuilder();
            var counter = 0;
            var command = connection.CreateCommand();
            foreach (var item in keyedValues)
            {
                keysBuffer.Append(item.Key).Append(',');
                var parameterName = string.Format(CultureInfo.InvariantCulture, "p{0}", counter++);
                paramsBuffer.AppendFormat("@{0},", parameterName);
                command.AddObjectParameter(parameterName, item.Value);
            }

            keysBuffer.Length = keysBuffer.Length - 1;
            paramsBuffer.Length = paramsBuffer.Length - 1;
            command.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", table, keysBuffer, paramsBuffer);
            return command;
        }

        public static IDbCommand CreateUpsertCommand(this IDbConnection connection, string table, object values, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            var keyedValues = (IDictionary<string, object>)values.ToExpando(whitelist: whitelist, blacklist: blacklist);
            if (keyedValues.Count <= 0)
            {
                throw new InvalidOperationException("Cannot upsert: there are no values to insert or update");
            }

            var keysBuffer = new StringBuilder();
            var paramsBuffer = new StringBuilder();
            var updateBuffer = new StringBuilder();
            var counter = 0;
            var command = connection.CreateCommand();
            foreach (var item in keyedValues)
            {
                keysBuffer.Append(item.Key).Append(',');
                var parameterName = string.Format(CultureInfo.InvariantCulture, "p{0}", counter++);
                updateBuffer.AppendFormat("{0}=@{1}", item.Key, parameterName).Append(',');
                paramsBuffer.AppendFormat("@{0},", parameterName);
                command.AddObjectParameter(parameterName, item.Value);
            }

            keysBuffer.Length = keysBuffer.Length - 1;
            paramsBuffer.Length = paramsBuffer.Length - 1;
            updateBuffer.Length = updateBuffer.Length - 1;
            command.CommandText = string.Format(
                "INSERT INTO {0} ({1}) VALUES ({2}) ON DUPLICATE KEY UPDATE {3}",
                table,
                keysBuffer,
                paramsBuffer,
                updateBuffer);
            return command;
        }

        public static IDbCommand CreateUpdateCommand(this IDbConnection connection, string table, object values, string where, object parameters = null, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            var keyedValues = (IDictionary<string, object>)values.ToExpando(whitelist: whitelist, blacklist: blacklist);
            if (keyedValues.Count <= 0)
            {
                throw new InvalidOperationException("Cannot update: there are no values to set");
            }

            var updateBuffer = new StringBuilder();
            var counter = 0;
            var command = connection.CreateCommand();
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