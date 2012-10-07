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
    public class DbProviderBasedConnectionFactory : AbstractConnectionFactory
    {
        public DbProviderBasedConnectionFactory(string providerName, string connectionString)
            : base(connectionString)
        {
            this.ProviderInvariantName = providerName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbProviderBasedConnectionFactory"/> class.
        /// It's the overriding class responsibility to set properly the <see cref="AbstractConnectionFactory.ConnectionString"/>
        /// and <see cref="ProviderInvariantName"/> properties.
        /// </summary>
        protected DbProviderBasedConnectionFactory()
        {
        }

        public string ProviderInvariantName { get; protected set; }

        /// <summary>
        /// Gets a <see cref="DbProviderFactory"/> from <see cref="DbProviderFactories"/> using
        /// <see cref="ProviderInvariantName"/>.
        /// </summary>
        public DbProviderFactory DbProviderFactory
        {
            get { return DbProviderFactories.GetFactory(this.ProviderInvariantName); }
        }

        /// <summary>
        /// Creates a new <see cref="IDbConnection"/> using <see cref="DbProviderFactory"/> and the
        /// provided connections string stored in <see cref="AbstractConnectionFactory.ConnectionString"/>.
        /// </summary>
        /// <returns></returns>
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