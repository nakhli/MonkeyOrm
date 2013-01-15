// <copyright file="SaveBatchTest.cs" company="Sinbadsoft">
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
// <date>2012/05/14</date>

using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace MonkeyOrm.Tests
{
    [TestFixture]
    public class SaveBatchTest : DbTestBase
    {
        [Test]
        public void SamePropertySetNoFilters(
            [Values(19, 50, 173, 200, 1000, 1049)] int batchSize,
            [Values(0, 1, 10, 17, 50, 51, 109)] int chunkSize)
        {
            var batch = GenerateBatch(batchSize).ToList();
            int insertedRowsCount = this.ConnectionFactory().SaveBatch("Test", batch, chunkSize);
            Assert.AreEqual(batchSize, insertedRowsCount);
            this.ReadbackAndCheck(batch);
        }

        [Test]
        public void SamePropertySetWithWhitelistFilter(
            [Values(19, 50, 173, 200, 1000, 1049)] int batchSize,
            [Values(0, 1, 10, 17, 50, 51, 109)] int chunkSize)
        {
            var batch = GenerateBatch(batchSize).ToList();
            int insertedRowsCount = this.ConnectionFactory().SaveBatch("Test", batch, chunkSize, new[] { "DataInt", "DataLong" });
            Assert.AreEqual(batchSize, insertedRowsCount);
            this.ReadbackAndCheck(batch, true);
        }

        [Test]
        public void SamePropertySetWithBlacklistFilter(
            [Values(19, 50, 173, 200, 1000, 1049)] int batchSize,
            [Values(0, 1, 10, 17, 50, 51, 109)] int chunkSize)
        {
            var batch = GenerateBatch(200).ToList();
            int insertedRowsCount = this.ConnectionFactory().SaveBatch("Test", batch, chunkSize, blacklist: new[] { "DataString" });
            Assert.AreEqual(batch.Count, insertedRowsCount);
            this.ReadbackAndCheck(batch, true);
        }

        [Test]
        public void DifferentPropertySetsNoFilters([Range(0, 5)] int chunkSize)
        {
            var batch = new List<object>
                {
                    new { DataInt = 5, DataLong = 3000000000L, DataString = "hello world1" },
                    new { DataInt = 5, DataLong = 3000000000L, DataString = "hello world2" },
                    new { DataInt = 5, DataLong = 3000000000L },
                    new { DataInt = 5, DataLong = 3000000000L, DataString = "hello world1" },
                };
            int insertedRowsCount = this.ConnectionFactory().SaveBatch("Test", batch, chunkSize);
            Assert.AreEqual(batch.Count, insertedRowsCount);
            var insertedData = this.ConnectionFactory().ReadAll("SELECT * FROM Test");
            for (var i = 0; i < batch.Count; i++)
            {
                DbTestBase.CheckTestObject(batch[i], insertedData[i], i == 2);
            }
        }

        [Test]
        public void InTransactionSavedEvenWithNoAutoCommit(
            [Values(19, 50, 173, 1049)] int batchSize,
            [Values(0, 1, 10, 17)] int chunkSize)
        {
            var batch = GenerateBatch(batchSize).ToList();
            int insertedRowsCount = this.ConnectionFactory().InTransaction(false).SaveBatch("Test", batch, chunkSize);
            Assert.AreEqual(batchSize, insertedRowsCount);
            this.ReadbackAndCheck(batch);
        }

        [Test]
        public void InTransactionNotCommitted(
            [Values(19, 50, 1049)] int batchSize,
            [Values(0, 17)] int chunkSize)
        {
            var batch = GenerateBatch(batchSize).ToList();
            int insertedRowsCount = this.ConnectionFactory().InTransaction().Do(t => t.SaveBatch("Test", batch, chunkSize));
            Assert.AreEqual(batchSize, insertedRowsCount);
            Assert.AreEqual(0, this.ConnectionFactory().ReadAll("SELECT * FROM Test").Count);
        }

        private void ReadbackAndCheck(IList<object> batch, bool defaultValueForString = false)
        {
            var insertedData = this.ConnectionFactory().ReadAll("SELECT * FROM Test");
            for (var i = 0; i < batch.Count; i++)
            {
                DbTestBase.CheckTestObject(batch[i], insertedData[i], defaultValueForString);
            }
        }
    }
}