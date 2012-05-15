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

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Sinbadsoft.Lib.Model.ToExpando;

namespace MonkeyOrm
{
    public static class CrudDbConnectionExtension
    {
        public static int Execute(this IDbConnection connection, string nonQuery, object parameters = null, IDbTransaction transaction = null)
        {
            using (var command = connection.CreateCommand(nonQuery, parameters, transaction))
            {
                return command.ExecuteNonQuery();
            }
        }

        public static List<dynamic> ReadAll(this IDbConnection connection, string query, object parameters = null, IDbTransaction transaction = null)
        {
            using (var command = connection.CreateCommand(query, parameters, transaction))
            using (var reader = command.ExecuteReader())
            {
                return reader.ToExpandoList();
            }
        }

        public static IEnumerable<dynamic> ReadStream(this IDbConnection connection, string query, object parameters = null, IDbTransaction transaction = null)
        {
            using (var command = connection.CreateCommand(query, parameters, transaction))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return reader.ToExpando();
                }
            }
        }

        public static void ReadStream(this IDbConnection connection, string query, Func<dynamic, bool> action, object parameters = null, IDbTransaction transaction = null)
        {
            using (var command = connection.CreateCommand(query, parameters, transaction))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read() && action(reader.ToExpando()))
                {
                    // do nothing
                }
            }
        }

        public static dynamic ReadOne(this IDbConnection connection, string query, object parameters = null, IDbTransaction transaction = null)
        {
            using (var command = connection.CreateCommand(query, parameters, transaction))
            using (var reader = command.ExecuteReader())
            {
                return reader.Read() ? reader.ToExpando() : null;
            }
        }

        public static int Save(this IDbConnection connection, string table, object values, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null, IDbTransaction transaction = null)
        {
            using (var command = connection.CreateInsertCommand(table, values, whitelist, blacklist, transaction))
            {
                return command.ExecuteNonQuery();
            }
        }

        public static int Save(this IDbConnection connection, string table, object values, out int id, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null, IDbTransaction transaction = null)
        {
            using (var command = connection.CreateInsertCommand(table, values, whitelist, blacklist, transaction))
            {
                int affectedRows = command.ExecuteNonQuery();
                id = (int)connection.LastInsertedId();
                return affectedRows;
            }
        }

        public static int Save(this IDbConnection connection, string table, object values, out long id, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null, IDbTransaction transaction = null)
        {
            using (var command = connection.CreateInsertCommand(table, values, whitelist, blacklist, transaction))
            {
                int affectedRows = command.ExecuteNonQuery();
                id = (long)connection.LastInsertedId();
                return affectedRows;
            }
        }

        public static int SaveBatch(this IDbConnection connection, string table, IEnumerable<object> batch, int chunkSize = 0, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null, IDbTransaction transaction = null)
        {
            if (chunkSize < 1)
            {
                using (var command = connection.CreateBatchInsertCommand(table, batch, whitelist, blacklist, transaction))
                {
                    return command.ExecuteNonQuery();
                }
            }

            var whitelistList = whitelist == null ? null : whitelist.ToList();
            var blacklistList = blacklist == null ? null : blacklist.ToList();
            var affectedRows = 0;
            var chunk = new List<object>(chunkSize);

            Func<int> insertChunk = () =>
                {
                    using (var command = connection.CreateBatchInsertCommand(table, chunk, whitelistList, blacklistList, transaction))
                    {
                        chunk.Clear();
                        return command.ExecuteNonQuery();
                    }
                };

            foreach (var obj in batch)
            {
                if (chunk.Count >= chunkSize)
                {
                    affectedRows += insertChunk();
                }
                
                chunk.Add(obj);
            }

            if (chunk.Count > 0)
            {
                affectedRows += insertChunk();
            }

            return affectedRows;
        }

        public static int Upsert(this IDbConnection connection, string table, object values, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null, IDbTransaction transaction = null)
        {
            using (var command = connection.CreateUpsertCommand(table, values, whitelist, blacklist, transaction))
            {
                int affectedRows = command.ExecuteNonQuery();
                return affectedRows;
            }
        }

        public static int Update(this IDbConnection connection, string table, object values, string where, object parameters = null, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null, IDbTransaction transaction = null)
        {
            using (var command = connection.CreateUpdateCommand(table, values, where, parameters, whitelist, blacklist, transaction))
            {
                return command.ExecuteNonQuery();
            }
        }

        public static int Delete(this IDbConnection connection, string table, string where, object parameters = null, IDbTransaction transaction = null)
        {
            using (var command = connection.CreateDeleteCommand(table, where, parameters, transaction))
            {
                return command.ExecuteNonQuery();
            }
        }

        internal static long LastInsertedId(this IDbConnection connection, IDbTransaction transaction = null)
        {
            using (var command = connection.CreateCommand("SELECT LAST_INSERT_ID()", transaction: transaction))
            {
                return (long)command.ExecuteScalar();
            }
        }
    }
}