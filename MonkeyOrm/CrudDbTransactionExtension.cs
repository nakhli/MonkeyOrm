// <copyright file="CrudDbTransactionExtension.cs" company="Sinbadsoft">
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
// <date>2012/05/04</date>

using System;
using System.Collections.Generic;
using System.Data;

namespace MonkeyOrm
{
    public static class CrudDbTransactionExtension
    {
        public static int Execute(this IDbTransaction transaction, string nonQuery, object parameters = null)
        {
            return transaction.Connection.Execute(nonQuery, parameters, transaction);
        }

        public static List<dynamic> ReadAll(this IDbTransaction transaction, string query, object parameters = null)
        {
            return transaction.Connection.ReadAll(query, parameters, transaction);
        }

        public static IEnumerable<dynamic> ReadStream(this IDbTransaction transaction, string query, object parameters = null)
        {
            return transaction.Connection.ReadStream(query, parameters, transaction);
        }

        public static void ReadStream(this IDbTransaction transaction, string query, Func<dynamic, bool> action, object parameters = null)
        {
            transaction.Connection.ReadStream(query, action, parameters, transaction);
        }

        public static dynamic ReadOne(this IDbTransaction transaction, string query, object parameters = null)
        {
            return transaction.Connection.ReadOne(query, parameters, transaction);
        }

        public static int Save(this IDbTransaction transaction, string table, object values, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            return transaction.Connection.Save(table, values, whitelist, blacklist, transaction);
        }

        public static int Save(this IDbTransaction transaction, string table, object values, out int id, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            return transaction.Connection.Save(table, values, out id, whitelist, blacklist, transaction);
        }

        public static int Save(this IDbTransaction transaction, string table, object values, out long id, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            return transaction.Connection.Save(table, values, out id, whitelist, blacklist, transaction);
        }

        public static int SaveBatch(this IDbTransaction transaction, string table, IEnumerable<object> batch, int chunkSize = 0, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            return transaction.Connection.SaveBatch(table, batch, chunkSize, whitelist, blacklist, transaction);
        }

        public static int Upsert(this IDbTransaction transaction, string table, object values, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            return transaction.Connection.Upsert(table, values, whitelist, blacklist, transaction);
        }

        public static int Update(this IDbTransaction transaction, string table, object values, string where, object parameters = null, IEnumerable<string> whitelist = null, IEnumerable<string> blacklist = null)
        {
            return transaction.Connection.Update(table, values, where, parameters, whitelist, blacklist, transaction);
        }

        public static int Delete(this IDbTransaction transaction, string table, string where, object parameters = null)
        {
            return transaction.Connection.Delete(table, where, parameters, transaction);
        }
    }
}

