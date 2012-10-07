// <copyright file="ConnectionFactory.cs" company="Sinbadsoft">
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

namespace MonkeyOrm
{
    public class ConnectionFactory<T> : AbstractConnectionFactory
        where T : IDbConnection, new()
    {
        public ConnectionFactory(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFactory{T}"/> class.
        /// It's the overriding class responsibility to set properly the <see cref="AbstractConnectionFactory.ConnectionString"/> property.
        /// </summary>
        protected ConnectionFactory()
        {
        }

        public override IDbConnection Create()
        {
            var connection = new T { ConnectionString = this.ConnectionString };
            return connection;
        }
    }
}