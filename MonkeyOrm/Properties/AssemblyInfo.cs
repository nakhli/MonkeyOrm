// <copyright file="AssemblyInfo.cs" company="Sinbadsoft">
// Copyright (c) Chaker Nakhli 2010
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
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("MonkeyOrm.MySql")]
[assembly: AssemblyDescription(@"MonkeyOrm is a small, powerful SQL-based ORM for .NET.
Website: http://www.monkeyorm.com .
Source code: https://github.com/Sinbadsoft/MonkeyOrm.
Some of MonkeyOrm features and design choices:
* Doesn’t pollute your code: no base classes to inherit from, no attributes and no xml config.
* Easy to adopt, easy to get rid of: No lock-in, it’s just your POCOs and plain old SQL.
* Stateless, no caching.
* Built-in CRUD: Create, Read, Update, Delete and Create-or-Update (aka Upsert).
* Transactions.
* Objects slicing on insertion and update with a black list or a white list.
* Bulk-fetching.
* Data streaming.
* Batch insertion, with fine-grained control on number of inserted objects per query.
* Blobbing.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Sinbadsoft")]
[assembly: AssemblyProduct("MonkeyOrm.MySql")]
[assembly: AssemblyCopyright("Copyright © Sinbadsoft 2011-2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1c06e2cd-9c28-416c-8f51-b95b0d4b0af9")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.4.9.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
