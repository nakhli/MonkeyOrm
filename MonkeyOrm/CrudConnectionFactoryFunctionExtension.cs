// <copyright file="CrudConnectionFactoryFunctionExtension.cs" company="Sinbadsoft">
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

using Sinbadsoft.Lib.Model.ToExpando;

namespace MonkeyOrm
{
    using Sinbadsoft.Lib.Model.CopyTo;

    public static class CrudConnectionFactoryFunctionExtension
    {
        public static int Execute(this Func<IDbConnection> connectionFactory, string nonQuery, object parameters = null)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return connection.Execute(nonQuery, parameters);
            }
        }

        public static List<dynamic> ReadAll(this Func<IDbConnection> connectionFactory, string query, object parameters = null)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return connection.ReadAll(query, parameters);
            }
        }

        public static IEnumerable<dynamic> ReadStream(this Func<IDbConnection> connectionFactory, string query, object parameters = null)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                using (var command = connection.CreateCommand(query, parameters))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.ToExpando();
                    }
                }
            }
        }

        public static void ReadStream(this Func<IDbConnection> connectionFactory, string query, Func<dynamic, bool> action, object parameters = null)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                connection.ReadStream(query, action, parameters);
            }
        }

        public static dynamic ReadOne(this Func<IDbConnection> connectionFactory, string query, object parameters = null)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return connection.ReadOne(query, parameters);
            }
        }

        public static int Save(this Func<IDbConnection> connectionFactory, string table, object values, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return connection.Save(table, values, whitelist, blacklist);
            }
        }

        public static int Save(this Func<IDbConnection> connectionFactory, string table, object values, out long id, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return connection.Save(table, values, out id, whitelist, blacklist);
            }
        }

        public static int Save(this Func<IDbConnection> connectionFactory, string table, object values, out int id, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return connection.Save(table, values, out id, whitelist, blacklist);
            }
        }

        public static int SaveBatch(this Func<IDbConnection> connectionFactory, string table, IEnumerable<object> batch, int chunkSize = 0, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return connection.SaveBatch(table, batch, chunkSize, whitelist, blacklist);
            }
        }

        public static int Update(this Func<IDbConnection> connectionFactory, string table, object values, string where, object parameters = null, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return connection.Update(table, values, where, parameters, whitelist, blacklist);
            }
        }

        public static int SaveOrUpdate(this Func<IDbConnection> connectionFactory, string table, object values, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return connection.SaveOrUpdate(table, values, whitelist, blacklist);
            }
        }

        public static int Delete(this Func<IDbConnection> connectionFactory, string table, string where, object parameters = null)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return connection.Delete(table, where, parameters);
            }
        }

        public static List<T> ReadAll<T>(this Func<IDbConnection> connectionFactory, string query, object parameters = null) where T : new()
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return connection.ReadAll<T>(query, parameters);
            }
        }

        public static IEnumerable<T> ReadStream<T>(this Func<IDbConnection> connectionFactory, string query, object parameters = null) where T : new()
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                using (var command = connection.CreateCommand(query, parameters))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.CopyTo<T>();
                    }
                }
            }
        }

        public static void ReadStream<T>(this Func<IDbConnection> connectionFactory, string query, Func<T, bool> action, object parameters = null) where T : new()
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                connection.ReadStream(query, action, parameters);
            }
        }

        public static T ReadOne<T>(this Func<IDbConnection> connectionFactory, string query, object parameters = null) where T : new()
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                return connection.ReadOne<T>(query, parameters);
            }
        }
    }
}