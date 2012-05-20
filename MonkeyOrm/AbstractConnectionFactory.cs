// <copyright file="AbstractConnectionFactory.cs" company="Sinbadsoft">
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
// <date>2012/05/04</date>
using System;
using System.Data;

namespace MonkeyOrm
{
    public abstract class AbstractConnectionFactory : IConnectionFactory
    {
        protected AbstractConnectionFactory(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        protected AbstractConnectionFactory()
        {
        }

        protected string ConnectionString { get; set; }

        public static implicit operator Func<IDbConnection>(AbstractConnectionFactory factory)
        {
            return () => factory.Create();
        }

        public abstract IDbConnection Create();
    }
}