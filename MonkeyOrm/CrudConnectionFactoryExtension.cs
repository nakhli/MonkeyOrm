// <copyright file="CrudConnectionFactoryExtension.cs" company="Sinbadsoft">
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

namespace MonkeyOrm
{
    public static class CrudConnectionFactoryExtension
    {
        public static int Execute(this IConnectionFactory connectionFactory, string nonQuery, object parameters = null)
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                return connection.Execute(nonQuery, parameters);
            }
        }

        public static List<dynamic> ReadAll(this IConnectionFactory connectionFactory, string query, object parameters = null)
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                return connection.ReadAll(query, parameters);
            }
        }

        public static dynamic ReadOne(this IConnectionFactory connectionFactory, string query, object parameters = null)
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                return connection.ReadOne(query, parameters);
            }
        }

        public static int Save(this IConnectionFactory connectionFactory, string table, object values, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                return connection.Save(table, values, whitelist, blacklist);
            }
        }

        public static int Save(this IConnectionFactory connectionFactory, string table, object values, out int id, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                return connection.Save(table, values, out id, whitelist, blacklist);
            }
        }

        public static int Save(this IConnectionFactory connectionFactory, string table, object values, out long id, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                return connection.Save(table, values, out id, whitelist, blacklist);
            }
        }

        public static int SaveBatch(this IConnectionFactory connectionFactory, string table, IEnumerable<object> batch, int chunkSize = 0, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                return connection.SaveBatch(table, batch, chunkSize, whitelist, blacklist);
            }
        }

        public static int Update(this IConnectionFactory connectionFactory, string table, object values, string where, object parameters = null, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                return connection.Update(table, values, where, parameters, whitelist, blacklist);
            }
        }

        public static int Upsert(this IConnectionFactory connectionFactory, string table, object values, IEnumerable<string> properties = null, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                return connection.Upsert(table, values, whitelist, blacklist);
            }
        }

        public static int Delete(this IConnectionFactory connectionFactory, string table, string where, object parameters = null)
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                return connection.Delete(table, where, parameters);
            }
        }
    }
}