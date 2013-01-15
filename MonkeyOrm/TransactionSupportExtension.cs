// <copyright file="TransactionSupportExtension.cs" company="Sinbadsoft">
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
using System.Data;

namespace MonkeyOrm
{
    public static class TransactionSupportExtension
    {
        public static ITransactionScope InTransaction(this IDbConnection connection, bool autocommit = false, IsolationLevel? isolation = null)
        {
            return new TransactionScope(connection, autocommit, isolation);
        }

        public static ITransactionScope InTransaction(this IConnectionFactory connectionFactory, bool autocommit = false, IsolationLevel? isolation = null)
        {
            return new ConnectionFactoryTransactionScope(connectionFactory.Create, autocommit, isolation);
        }

        public static ITransactionScope InTransaction(this Func<IDbConnection> connectionFactory, bool autocommit = false, IsolationLevel? isolation = null)
        {
            return new ConnectionFactoryTransactionScope(connectionFactory, autocommit, isolation);
        }

        internal abstract class AbstractTransactionScope : ITransactionScope
        {
            private readonly IsolationLevel? isolation;

            protected AbstractTransactionScope(bool autocommit = false, IsolationLevel? isolation = null)
            {
                this.AutoCommit = autocommit;
                this.isolation = isolation;
            }

            public bool AutoCommit { get; set; }

            public void Do(Action<IDbTransaction> action)
            {
                this.Do(t => { action(t); return 0; });
            }

            public abstract T Do<T>(Func<IDbTransaction, T> action);

            public T Do<T>(IDbConnection connection, Func<IDbTransaction, T> action)
            {
                using (var transaction = this.CreateTransaction(connection))
                {
                    T value = action(transaction);
                    if (this.AutoCommit)
                    {
                        CommitTransaction(transaction);
                    }

                    return value;
                }
            }

            protected void Do(IDbConnection connection, Action<IDbTransaction> action)
            {
                using (var transaction = this.CreateTransaction(connection))
                {
                    action(transaction);
                    if (this.AutoCommit)
                    {
                        CommitTransaction(transaction);
                    }
                }
            }

            protected IDbTransaction CreateTransaction(IDbConnection connection)
            {
                return this.isolation.HasValue
                    ? connection.BeginTransaction(this.isolation.Value)
                    : connection.BeginTransaction();
            }

            private static void CommitTransaction(IDbTransaction transaction)
            {
                try
                {
                    transaction.Commit();
                }
                catch (InvalidOperationException)
                {
                    // The transaction has already been committed or rolled back.
                    // -or- The connection is broken.
                    if (transaction.Connection.State == ConnectionState.Broken)
                    {
                        throw;
                    }
                }
            }
        }

        internal class ConnectionFactoryTransactionScope : AbstractTransactionScope
        {
            private readonly Func<IDbConnection> connectionFactory;

            public ConnectionFactoryTransactionScope(Func<IDbConnection> connectionFactory, bool autocommit = false, IsolationLevel? isolation = null)
                : base(autocommit, isolation)
            {
                this.connectionFactory = connectionFactory;
            }

            public override T Do<T>(Func<IDbTransaction, T> action)
            {
                using (var connection = this.connectionFactory())
                {
                    connection.Open();
                    return this.Do(connection, action);
                }
            }
        }

        internal class TransactionScope : AbstractTransactionScope
        {
            private readonly IDbConnection connection;

            public TransactionScope(IDbConnection connection, bool autocommit = false, IsolationLevel? isolation = null)
                : base(autocommit, isolation)
            {
                this.connection = connection;
            }

            public override T Do<T>(Func<IDbTransaction, T> action)
            {
                return this.Do(this.connection, action);
            }
        }
    }
}
