// <copyright file="Massive.cs" company="Sinbadsoft">
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
// <date>2010/11/04</date>
using System;
using System.Collections.Generic;
using System.Data;

using Sinbadsoft.Lib.Model.ToExpando;

namespace MonkeyOrm
{
    public static class Massive
    {
        /// <summary>
        /// Extension method for adding parameters based on object list.
        /// Parameters are named pX where X is the position of the parameter
        /// in the command's parameters list.
        /// </summary>
        public static void AddObjectParameters(this IDbCommand cmd, params object[] args)
        {
            foreach (var item in args)
            {
                AddObjectParameter(cmd, string.Format("p{0}", cmd.Parameters.Count), item);
            }
        }

        /// <summary>
        /// Adds a parameter with the given name and value. <see cref="Guid"/> are translated to
        /// a 16 bytes array. Null values will insert <see cref="DBNull.Value"/>.
        /// </summary>
        public static void AddObjectParameter(this IDbCommand cmd, string name, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else if (value is Guid)
            {
                var guidItem = (Guid)value;
                parameter.Value = guidItem.ToByteArray();
                parameter.DbType = DbType.Binary;
                parameter.Size = 16;
            }
            else
            {
                parameter.Value = value;
            }

            cmd.Parameters.Add(parameter);
        }

        /// <summary>
        /// Adds a parameter for each property of the provided object, with its corresponding value
        /// as the parameter value. <see cref="Guid"/> are translated to
        /// a 16 bytes array. Null values will insert <see cref="DBNull.Value"/>.
        /// </summary>
        public static void AddPropertiesAsParameters(this IDbCommand command, object parameters)
        {
            foreach (var parameter in parameters.ToExpando())
            {
                command.AddObjectParameter(parameter.Key, parameter.Value);
            }
        }

        /// <summary>
        /// Turns an IDataReader to a Dynamic list of things
        /// </summary>
        public static List<dynamic> ToExpandoList(this IDataReader reader)
        {
            var result = new List<dynamic>();
            while (reader.Read())
            {
                result.Add(reader.ToExpando());
            }

            return result;
        }
    }
}

// This class is partially based on the Massive database access tool. Massive has the following license:
// New BSD License
// http://www.opensource.org/licenses/bsd-license.php
// Copyright (c) 2009, Rob Conery (robconery@gmail.com)
// All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// Neither the name of the SubSonic nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
