// <copyright file="ReadStreamTest.cs" company="Sinbadsoft">
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
// <date>2012/05/15</date>

using System.Linq;

using NUnit.Framework;

namespace MonkeyOrm.Tests
{
    [TestFixture]
    public class ReadStreamTest : DbTestBase
    {
        [Test]
        public void RawConnectionWithEnumerableResult([Values(1, 5, 300)] int size)
        {
            var batch = GenerateBatch(size).ToList();
            this.ConnectionFactory().SaveBatch("Test", batch);
            using (var connection = this.ConnectionFactory().Create())
            {
                connection.Open();
                int i = 0;
                foreach (var actual in connection.ReadStream("SELECT * FROM Test"))
                {
                    CheckTestObject(batch[i++], actual);
                }
            }
        }

        [Test]
        public void EnumerableResult([Values(1, 5, 300)] int size)
        {
            var batch = GenerateBatch(size).ToList();
            this.ConnectionFactory().SaveBatch("Test", batch);

            int i = 0;
            foreach (var actual in this.ConnectionFactory().ReadStream("SELECT * FROM Test"))
            {
                CheckTestObject(batch[i++], actual);
            }
        }

        [Test]
        public void EnumerableResultWithLinq([Values(1, 5, 300)] int size)
        {
            var batch = GenerateBatch(size).ToList();
            this.ConnectionFactory().SaveBatch("Test", batch);

            int i = 0;
            var stream = this.ConnectionFactory().ReadStream("SELECT * FROM Test")
                .Where(obj => obj.DataInt > 15 && obj.DataInt < 100)
                .Select(obj => (string)obj.DataString);

            while (i < batch.Count && ((dynamic)batch[i++]).DataInt < 15)
            {
                /* nop */
            }

            foreach (var value in stream)
            {
                dynamic expected = batch[i++];
                Assert.AreEqual(expected.DataString, value);
            }

            var result = this.ConnectionFactory().ReadStream("SELECT * FROM Test").First();
            CheckTestObject(batch[0], result);
        }


        [Test]
        public void WithAction([Values(1, 5, 300)] int size)
        {
            var batch = GenerateBatch(size).ToList();
            this.ConnectionFactory().SaveBatch("Test", batch);

            int i = 0;
            this.ConnectionFactory().ReadStream(
                "SELECT * FROM Test",
                actual =>
                {
                    CheckTestObject(batch[i++], actual);
                    return true;
                });
        }

        [Test]
        public void EnsureActionStops()
        {
            var batch = GenerateBatch(100).ToList();
            this.ConnectionFactory().SaveBatch("Test", batch);

            int i = 0;
            this.ConnectionFactory().ReadStream(
                "SELECT * FROM Test",
                actual =>
                {
                    CheckTestObject(batch[i++], actual);
                    return i < 65;
                });
            Assert.AreEqual(65, i);
        }
    }
}