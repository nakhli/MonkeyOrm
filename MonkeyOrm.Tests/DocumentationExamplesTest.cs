// <copyright file="DocumentationExamplesTest.cs" company="Sinbadsoft">
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
// <date>2012/10/06</date>
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace MonkeyOrm.Tests
{
    public class DocumentationExamplesTest : DbTestBase
    {
        public DocumentationExamplesTest()
            : base(false)
        {
        }

        [SetUp]
        public new void SetUp()
        {
            this.ConnectionFactory().Execute(
                  @"CREATE TABLE `Users` (
                        `Id` INT NOT NULL AUTO_INCREMENT,
                        `Name` VARCHAR(250),
                        `Age` INT,
                        `CanBuyAlchohol` BOOLEAN DEFAULT False,
                        PRIMARY KEY (`Id`),
                        UNIQUE (`Name`)
                    ) AUTO_INCREMENT=0 ENGINE=InnoDB");

            this.ConnectionFactory().Execute(
                @"CREATE TABLE `Profiles` (
                      `Id` INT NOT NULL AUTO_INCREMENT,
                      `UserId` INT NOT NULL,
                      `Bio` Text,
                      PRIMARY KEY (`Id`),
                      FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
                  ) AUTO_INCREMENT=0 ENGINE=InnoDB");
        }

        [TearDown]
        public new void TearDown()
        {
            this.ConnectionFactory().Execute(@"DROP TABLE IF EXISTS `Profiles`");
            this.ConnectionFactory().Execute(@"DROP TABLE IF EXISTS `Users`");
        }

        [Test]
        public void SaveAnything()
        {
            var connection = this.ConnectionFactory();

            connection.Save("Users", new User { Name = "Anne", Age = 31 });

            // Get back the saved object Id:
            int pabloId;
            connection.Save("Users", new User { Name = "Pablo", Age = 49 }, out pabloId);


            // Anonymous
            connection.Save("Users", new { Name = "Jhon", Age = 26 });

            // Dictionary<>, Dictionary, ExpandoObject or NameValueCollection
            connection.Save(
                "Users",
                new Dictionary<string, object>
                {
                    { "Name", "Fred" },
                    { "Age", 22 }
                });


            connection
                .ReadAll("Select * From Users")
                .ForEach(user => Console.WriteLine("Name: {0}, Age: {1}", user.Name, user.Age));
        }

        [Test]
        public void ReadOneOrAll()
        {
            var connection = this.ConnectionFactory();
            connection.Save("Users", new User { Name = "Anne", Age = 31 });

            // Read all: Bulk fetches all items matching query and store them in a List
            // Query parameters are automatically matched in the provided parameters object.
            // Parameter object can be anonymous or -- as for the Save method -- a POCO, a dictionary etc.
            var users = connection.ReadAll("Select * From Users Where Age > @age", new { age = 30 });

            // Read one: Read the first item, if any, matching the query
            dynamic joe = connection.ReadOne("Select * From Users Where @Id=id", new { id = 1 });

            // Another example of read one
            dynamic stats = connection.ReadOne("Select Max(Age) As Max, Min(Age) As Min From Users");
            Console.WriteLine("Max {0} - Min {1}", stats.Max, stats.Min);

            var userStream = connection.ReadStream("Select * From Users");
            using (var fileStream = new StreamWriter(Path.GetTempFileName()))
            {
                foreach (dynamic user in userStream)
                {
                    fileStream.WriteLine("{0} - {1}", user.Name, user.Age);
                }
            }
        }

        [Test]
        public void Update()
        {
            var connection = this.ConnectionFactory();
            var daenrys = new User { Name = "Daenerys", Age = 18 };
            connection.Save("Users", daenrys);

            connection.Update("Users", new { CanBuyAlchohol = true }, "Age >= @age", new { age = 21 });

            Assert.IsTrue(!connection.ReadOne("Select * From Users Where Name=@Name", daenrys).CanBuyAlchohol);
        }

        [Test]
        public void SaveOrUpdate()
        {
            var connection = this.ConnectionFactory();

            connection.Save("Users", new User { Name = "Anne", Age = 31 });

            connection.SaveOrUpdate("Users", new User { Name = "Anne", Age = 32 });

            var users = connection.ReadAll("Select * From Users Where Name = @name", new { name = "Anne" });
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual(32, users.First().Age);
        }


        [Test]
        public void Delete()
        {
            var connection = this.ConnectionFactory();
            connection.Save("Users", new User { Name = "Sauron", Age = 27061 });

            connection.Delete("Users", "Name=@name", new { name = "Sauron" });

            Assert.IsTrue(connection.ReadOne("Select Count(*) As Count From Users").Count == 0);
        }

        [Test]
        public void Transactions()
        {
            var connection = this.ConnectionFactory();
            connection.Save("Users", new User { Name = "James", Age = 42 });
            connection.Save("Users", new User { Name = "Ged", Age = 38 });

            // Transaction with manual commit and returning a value from the transaction block
            int spockId = connection.InTransaction(autocommit: true).Do(t =>
            {
                int id;
                t.Save("Users", new { Name = "Spock", Age = 55 }, out id);
                t.Save("Profiles", new { UserId = id, Bio = "Federation Ambassador" });
                return id;
            });

            // Auto commit 
            connection.InTransaction(true, IsolationLevel.Serializable).Do(t =>
            {
                var james = t.ReadOne("Select * From Users Where Name=@name", new { name = "James" });
                t.Update("Users", new { Age = james.Age + 15 }, "Id=@Id", new { james.Id });
            });

            Assert.AreEqual(
                57,
                connection.ReadOne("Select * From Users Where Name=@name", new { name = "James" }).Age);

            // Auto commit + Isolation level
            int deletedId = connection.InTransaction(true, IsolationLevel.Serializable).Do(
            t =>
            {
                int gedId = t.ReadOne("Select Id From Users Where Name=@name", new { name = "Ged" }).Id;
                t.Delete("Users", "Id=@gedId", new { gedId });
                return gedId;
            });
        }

        [Test]
        public void SaveBatch()
        {
            var connection = this.ConnectionFactory();

            IEnumerable<User> users = new List<User> 
            {
                new User { Name = "Fred", Age = 24 },
                new User { Name = "Joe", Age = 26 },
                /* ... */ 
            };

            // A prepared insert command is created and is executed for each object in the enumerable
            connection.SaveBatch("Users", users);
            Assert.AreEqual(2, connection.ReadOne("Select Count(Id) As Count From Users").Count);

            connection.Execute("Truncate Users");

            // Ten objects inserted at once for each command execution.
            connection.SaveBatch("Users", users, 10);
            Assert.AreEqual(2, connection.ReadOne("Select Count(Id) As Count From Users").Count);

            connection.Execute("Truncate Users");

            // Batch save can be wrapped in a transaction
            connection.InTransaction().SaveBatch("Users", users);

            Assert.AreEqual(2, connection.ReadOne("Select Count(Id) As Count From Users").Count);
        }

        [Test]
        public void Execute()
        {
            var connection = this.ConnectionFactory();

            connection.Execute("Insert Into Users (Name, Age) Values (@Name, @Age)", new { Name = "Philip", Age = 55 });
            connection.Execute("Truncate Table Users");
        }

        public class User
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }
    }
}
