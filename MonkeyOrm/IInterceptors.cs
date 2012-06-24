// <copyright file="IInterceptors.cs" company="Sinbadsoft">
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
// <date>2012/06/01</date>
using System;
using System.Data;
using System.Data.Common;

namespace MonkeyOrm
{
    public interface IInterceptors
    {
        /// <summary>
        /// This interceptor is used when creating a <see cref="DbParameter"/> command parameter object
        /// for a value with a type that is not automatically managed by the ORM runtime.
        /// This can happen during during the creation of a <see cref="IDbCommand"/> commands for inserting,
        /// updating, reading or deleting data; when one of the query parameters or one of the values to be
        /// inserted is of an unknown type. This interceptor is then called to gives the user a chance to transform
        /// this value back to a known type before the runtime creates its associated sql parameter 
        /// <see cref="DbParameter"/> object.
        /// 
        /// Values of known types will be automatically managed and will <strong>not</strong> involve a call to 
        /// this interceptor. Known types are:
        /// <list type="bullet">        
        /// <item>integral or floating number</item>
        /// <item><see cref="String"/></item>
        /// <item><see cref="Guid"/></item>
        /// <item><see cref="DateTime"/> or <see cref="TimeSpan"/></item>
        /// <item><see cref="DBNull"/></item>
        /// <item>any <see langword="null"/> value</item>
        /// </list>
        /// 
        /// For example, this handler can be used, to give the user a chance to serialize POCOs on insertion to JSON, binary, XML etc.
        /// by transforming the intercepted value to a string or to a byte array.
        /// <remarks>
        /// This handler defaults to the identity function.
        /// The value passed to the hander can never be <see langword="null"/>
        /// </remarks>
        /// </summary>
        Func<object, object> UnknownValueType { get; set; }
    }
}
