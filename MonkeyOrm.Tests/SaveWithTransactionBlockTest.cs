// <copyright file="SaveWithTransactionBlockTest.cs" company="Sinbadsoft">
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
using NUnit.Framework;

namespace MonkeyOrm.Tests
{
    [TestFixture]
    public class SaveWithTransactionBlockTest : DbTestBase
    {
        [Test]
        public void DoesntSaveWithoutCommit()
        {
            this.DoesntSaveWithoutCommit(false);
        }

        [Test]
        public void DoesntSaveOnRollback()
        {
            this.DoesntSaveWithoutCommit(true);
        }

        [Test]
        public void SaveAndReadBack()
        {
            var values = new { DataInt = 5, DataLong = 3000000000L, DataString = "hello world" };
            int id = -1;
            this.ConnectionFactory().InTransaction().Do(transaction =>
                {
                    transaction.Save("Test", values, out id);
                    Assert.AreEqual(1, id);
                    transaction.Commit();
                });

            this.ReadBackAndCheckValues(values, id);
        }

        [Test]
        public void SaveAndReadBackWithAutoCommit()
        {
            var values = new { DataInt = 5, DataLong = 3000000000L, DataString = "hello world" };
            int id = -1;
            this.ConnectionFactory().InTransaction(true).Do(
                transaction =>
                    {
                    transaction.Save("Test", values, out id);
                    Assert.AreEqual(1, id);
                });
            this.ReadBackAndCheckValues(values, id);
        }

        private void ReadBackAndCheckValues(dynamic values, object id)
        {
            var read = this.ConnectionFactory().ReadOne("SELECT * FROM Test");
            Assert.AreEqual(id, read.Id);
            Assert.AreEqual(values.DataInt, read.DataInt);
            Assert.AreEqual(values.DataLong, read.DataLong);
            Assert.AreEqual(values.DataString, read.DataString);
        }

        private void DoesntSaveWithoutCommit(bool explicitRollback)
        {
            var values = new { DataInt = 5, DataLong = 3000000000L, DataString = "hello world" };
            int id = -1;
            this.ConnectionFactory().InTransaction().Do(
                transaction =>
                {
                    transaction.Save("Test", values, out id);
                    Assert.AreEqual(1, id);
                    if (explicitRollback)
                    {
                        transaction.Rollback();
                    }
                });

            Assert.True(id > 0);
            Assert.IsNull(this.ConnectionFactory().ReadOne("SELECT * FROM Test WHERE Id=@id", new { id }));
        }
    }
}