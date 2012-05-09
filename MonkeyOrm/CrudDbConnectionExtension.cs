// <copyright file="CrudDbConnectionExtension.cs" company="Sinbadsoft">
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
using System.Collections.Generic;
using System.Data;

using Sinbadsoft.Lib.Model.ToExpando;

namespace MonkeyOrm
{
    public static class CrudDbConnectionExtension
    {
        public static int Execute(this IDbConnection connection, string nonQuery, object parameters = null)
        {
            using (var command = connection.CreateCommand(nonQuery, parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

        public static List<dynamic> ReadAll(this IDbConnection connection, string query, object parameters = null)
        {
            using (var command = connection.CreateCommand(query, parameters))
            using (var reader = command.ExecuteReader())
            {
                return reader.ToExpandoList();
            }
        }

        public static dynamic ReadOne(this IDbConnection connection, string query, object parameters = null)
        {
            using (var command = connection.CreateCommand(query, parameters))
            using (var reader = command.ExecuteReader())
            {
                return reader.Read() ? reader.ToExpando() : null;
            }
        }

        public static int Save(this IDbConnection connection, string table, object values, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var command = connection.CreateInsertCommand(table, values, whitelist, blacklist))
            {
                return command.ExecuteNonQuery();
            }
        }

        public static int Save(this IDbConnection connection, string table, object values, out int id, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var command = connection.CreateInsertCommand(table, values, whitelist, blacklist))
            {
                int affectedRows = command.ExecuteNonQuery();
                id = (int)connection.LastInsertedId();
                return affectedRows;
            }
        }

        public static int Save(this IDbConnection connection, string table, object values, out long id, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var command = connection.CreateInsertCommand(table, values, whitelist, blacklist))
            {
                int affectedRows = command.ExecuteNonQuery();
                id = (long)connection.LastInsertedId();
                return affectedRows;
            }
        }

        public static int Upsert(this IDbConnection connection, string table, object values, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var command = connection.CreateUpsertCommand(table, values, whitelist, blacklist))
            {
                int affectedRows = command.ExecuteNonQuery();
                return affectedRows;
            }
        }

        public static int Update(this IDbConnection connection, string table, object values, string where, object parameters = null, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var command = connection.CreateUpdateCommand(table, values, where, parameters, whitelist, blacklist))
            {
                return command.ExecuteNonQuery();
            } 
        }

        public static int Delete(this IDbConnection connection, string table, string where, object parameters = null)
        {
            using (var command = connection.CreateDeleteCommand(table, where, parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

        internal static object LastInsertedId(this IDbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT LAST_INSERT_ID()";
                return command.ExecuteScalar();
            }
        }
    }
}