// <copyright file="FunctionBasedConnectionFactory.cs" company="Sinbadsoft">
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
using System;
using System.Data;

namespace MonkeyOrm
{
    public class FunctionBasedConnectionFactory : IConnectionFactory
    {
        public FunctionBasedConnectionFactory(Func<IDbConnection> functionFactory)
        {
            this.FunctionFactory = functionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionBasedConnectionFactory"/> class.
        /// It's the overriding class responsibility to set properly the <see cref="FunctionFactory"/> property.
        /// </summary>
        protected FunctionBasedConnectionFactory()
        {
        }

        public Func<IDbConnection> FunctionFactory { get; private set; }

        public IDbConnection Create()
        {
            return this.FunctionFactory();
        }
    }
}