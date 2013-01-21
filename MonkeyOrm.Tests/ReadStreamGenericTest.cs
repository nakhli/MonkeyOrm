namespace MonkeyOrm.Tests
{
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class ReadStreamGenericTest : DbTestBase
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
                foreach (var actual in connection.ReadStream<TestData>("SELECT * FROM Test"))
                {
                    DbTestBase.CheckTestObject(batch[i++], actual);
                }
            }
        }

        [Test]
        public void EnumerableResult([Values(1, 5, 300)] int size)
        {
            var batch = GenerateBatch(size).ToList();
            this.ConnectionFactory().SaveBatch("Test", batch);

            int i = 0;
            foreach (var actual in this.ConnectionFactory().ReadStream<TestData>("SELECT * FROM Test"))
            {
                DbTestBase.CheckTestObject(batch[i++], actual);
            }
        }

        [Test]
        public void EnumerableResultWithLinq([Values(1, 5, 300)] int size)
        {
            var batch = GenerateBatch(size).ToList();
            this.ConnectionFactory().SaveBatch("Test", batch);

            int i = 0;
            var strings = this.ConnectionFactory().ReadStream<TestData>("SELECT * FROM Test")
                .Where(obj => obj.DataInt > 15 && obj.DataInt < 100)
                .Select(obj => obj.DataString);

            while (i < batch.Count && batch[i++].DataInt < 15)
            {
                /* nop */
            }

            foreach (var value in strings)
            {
                dynamic expected = batch[i++];
                Assert.AreEqual(expected.DataString, value);
            }

            var result = this.ConnectionFactory().ReadStream<TestData>("SELECT * FROM Test").First();
            DbTestBase.CheckTestObject(batch[0], result);
        }

        [Test]
        public void EnumerateResultMultipleTimes()
        {
            var result = this.ConnectionFactory().ReadStream<TestData>("SELECT * FROM Test");

            Assert.AreEqual(result, this.ConnectionFactory().ReadStream<TestData>("SELECT * FROM Test"));
        }

        [Test]
        public void WithAction([Values(1, 5, 300)] int size)
        {
            var batch = GenerateBatch(size).ToList();
            this.ConnectionFactory().SaveBatch("Test", batch);

            int i = 0;
            this.ConnectionFactory().ReadStream<TestData>(
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
            this.ConnectionFactory().ReadStream<TestData>(
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