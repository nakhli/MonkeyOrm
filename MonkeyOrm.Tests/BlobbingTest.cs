// <copyright file="BlobbingTest.cs" company="Sinbadsoft">
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
// <date>2012/05/20</date>

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

using NUnit.Framework;

namespace MonkeyOrm.Tests
{
    [TestFixture]
    public class BlobbingTest : DbTestBase
    {
        private Func<object, object> oldUnknownValueTypeInterceptor;

        public BlobbingTest() : base(false)
        {
        }

        [SetUp]
        public new void SetUp()
        {
            this.oldUnknownValueTypeInterceptor = MonkeyOrm.Settings.Interceptors.UnknownValueType;
        }

        [TearDown]
        public new void TearDown()
        {
            this.ConnectionFactory().Execute(@"DROP TABLE IF EXISTS `Test`");
            MonkeyOrm.Settings.Interceptors.UnknownValueType = this.oldUnknownValueTypeInterceptor;
        }

        [Test]
        public void XmlBlob()
        {
            MonkeyOrm.Settings.Interceptors.UnknownValueType = SerializeToXml;
            this.ConnectionFactory().Execute(
                @"CREATE TABLE `Test` (`Id` INT NOT NULL,`Data` INT,`Poco` TEXT, PRIMARY KEY (`Id`)) ENGINE=InnoDB");
            var containsPoco = new SamplePoco { Astring = "hello" };
            this.ConnectionFactory().Save("Test", new { Id = 10, Data = 5, Poco = containsPoco });
            dynamic result = this.ConnectionFactory().ReadOne("SELECT Id, Data, Poco FROM Test");
            Assert.AreEqual(10, result.Id);
            Assert.AreEqual(5, result.Data);
            Assert.AreEqual(SerializeToXml(containsPoco), result.Poco);
        }

        [Test]
        public void BinaryBlob()
        {
            MonkeyOrm.Settings.Interceptors.UnknownValueType = SerializeToBinary;
            this.ConnectionFactory().Execute(
                @"CREATE TABLE `Test` (`Id` INT NOT NULL,`Data` INT,`Poco` VARBINARY(512), PRIMARY KEY (`Id`)) ENGINE=InnoDB");
            var containsPoco = new SamplePoco { Astring = "hello" };
            this.ConnectionFactory().Save("Test", new { Id = 10, Data = 5, Poco = containsPoco });
            dynamic result = this.ConnectionFactory().ReadOne("SELECT Id, Data, Poco FROM Test");
            Assert.AreEqual(10, result.Id);
            Assert.AreEqual(5, result.Data);
            Assert.AreEqual(SerializeToBinary(containsPoco), result.Poco);
        }

        private static string SerializeToXml(object o)
        {
            var writer = new StringWriter();
            new XmlSerializer(o.GetType()).Serialize(writer, o);
            return writer.ToString();
        }

        private static object SerializeToBinary(object o)
        {
            if (!o.GetType().IsSerializable)
            {
                return o;
            }

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, o);
                return stream.ToArray();
            }
        }

        [Serializable]
        public class SamplePoco
        {
            public int AnInt { get; set; }

            public string Astring { get; set; }
        }
    }
}