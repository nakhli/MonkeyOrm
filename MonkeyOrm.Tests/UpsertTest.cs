// <copyright file="UpsertTest.cs" company="Sinbadsoft">
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
using System.Collections.Generic;
using NUnit.Framework;

namespace MonkeyOrm.Tests
{
    public class UpsertTest : DbTestBase
    {
        public UpsertTest() : base(false)
        {
        }

        [TearDown]
        public new void TearDown()
        {
            this.ConnectionFactory().Execute(@"DROP TABLE IF EXISTS `Test`");
        }

        [Test]
        public void DuplicatePrimaryKey()
        {
            this.ConnectionFactory().Execute(
                @"CREATE TABLE `Test` (`Id` INT NOT NULL,`Data` INT,PRIMARY KEY (`Id`)) ENGINE=InnoDB");
            this.ConnectionFactory().Save("Test", new { Id = 10, Data = 5 });
            Assert.AreEqual(5, this.ConnectionFactory().ReadOne("SELECT * FROM Test").Data);
            this.ConnectionFactory().SaveOrUpdate("Test", new { Id = 10, Data = 34 });
            List<dynamic> all = this.ConnectionFactory().ReadAll("SELECT * FROM Test");
            Assert.AreEqual(1, all.Count);
            Assert.AreEqual(34, all[0].Data);
        }

        [Test]
        public void DuplicateUniqueConstraint()
        {
            this.ConnectionFactory().Execute(
                @"CREATE TABLE `Test` (`Id` INT NOT NULL AUTO_INCREMENT,`A` INT, `B` INT, `C` INT,
                  PRIMARY KEY (`Id`),
                  UNIQUE KEY `Uq` (`A`, `B`)) ENGINE=InnoDB");
            this.ConnectionFactory().Save("Test", new { A = 5, B = 3, C = 123 });
            Assert.AreEqual(123, this.ConnectionFactory().ReadOne("SELECT * FROM Test").C);
            this.ConnectionFactory().SaveOrUpdate("Test", new { A = 5, B = 3, C = -1 });
            List<dynamic> all = this.ConnectionFactory().ReadAll("SELECT * FROM Test");
            Assert.AreEqual(-1, all[0].C);
        }

        [Test]
        public void DuplicateUniqueConstraintAndPrimaryKey()
        {
            this.ConnectionFactory().Execute(
                @"CREATE TABLE `Test` (`Id` INT NOT NULL,`A` INT, `B` INT, `C` INT,
                  PRIMARY KEY (`Id`),
                  UNIQUE KEY `Uq` (`A`, `B`)) ENGINE=InnoDB");
            this.ConnectionFactory().Save("Test", new { Id = 10, A = 5, B = 3, C = 123 });
            Assert.AreEqual(123, this.ConnectionFactory().ReadOne("SELECT * FROM Test").C);
            this.ConnectionFactory().SaveOrUpdate("Test", new { Id = 10, A = 5, B = 3, C = -1 });
            List<dynamic> all = this.ConnectionFactory().ReadAll("SELECT * FROM Test");
            Assert.AreEqual(-1, all[0].C);
        }

        [Test]
        public void NoDuplicate()
        {
            this.ConnectionFactory().Execute(
                @"CREATE TABLE `Test` (`Id` INT NOT NULL,`Data` INT,PRIMARY KEY (`Id`)) ENGINE=InnoDB");
            this.ConnectionFactory().SaveOrUpdate("Test", new { Id = 10, Data = 34 });
            Assert.AreEqual(34, this.ConnectionFactory().ReadOne("SELECT * FROM Test").Data);
        }
    }
}