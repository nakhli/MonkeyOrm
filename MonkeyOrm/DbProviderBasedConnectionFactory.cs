// <copyright file="DbProviderBasedConnectionFactory.cs" company="Sinbadsoft">
// Copyright (c) Chaker Nakhli 2012
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
using System.Data;
using System.Data.Common;

namespace MonkeyOrm
{
    /// <summary>
    /// An implementation of <see cref="IConnectionFactory"/> based on <see cref="DbProviderFactory"/>.
    /// </summary>
    public abstract class DbProviderBasedConnectionFactory : AbstractConnectionFactory
    {
        private readonly string providerName;

        protected DbProviderBasedConnectionFactory(string providerName, string connectionString)
            : base(connectionString)
        {
            this.providerName = providerName;
        }

        protected DbProviderFactory DbProviderFactory
        {
            get { return DbProviderFactories.GetFactory(this.providerName); }
        }

        public override IDbConnection Create()
        {
            var connection = this.DbProviderFactory.CreateConnection();
            if (connection != null)
            {
                connection.ConnectionString = this.ConnectionString;
                return connection;
            }

            return null;
        }
    }
}