﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Celerik.NetCore.Util.Test
{
    public enum PersonalityType
    {
        [System.ComponentModel.Description(":()")]
        Choleric = 1,

        [System.ComponentModel.Description(":)")]
        Phlegmatic = 2
    }

    public class Employee
    {
        [JsonConverter(typeof(EnumDescriptionJsonConverter))]
        public PersonalityType? Personality { get; set; }
    }

    [TestClass]
    public class EnumDescriptionJsonConverterTest
    {
        [TestMethod]
        public void SerializeSettedValue()
        {
            var me = new Employee { Personality = PersonalityType.Choleric };
            var meSerialized = JsonConvert.SerializeObject(me);

            Assert.AreEqual(true, meSerialized.ContainsInvariant(":()"));
        }

        [TestMethod]
        public void SerializeUnsettedValue()
        {
            var me = new Employee();
            var meSerialized = JsonConvert.SerializeObject(me);

            Assert.AreEqual(true, meSerialized.ContainsInvariant("null"));
        }

        [TestMethod]
        public void Deserialize()
        {
            var json = "{ Personality: 2 }";
            var deserialized = JsonConvert.DeserializeObject<Employee>(json);

            Assert.AreEqual(PersonalityType.Phlegmatic, deserialized.Personality);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteJsonNullWritter()
        {
            var converter = new EnumDescriptionJsonConverter();
            converter.WriteJson(null, "{}", new JsonSerializer());
        }

        [TestMethod]
        public void ReadJson()
        {
            var converter = new EnumDescriptionJsonConverter();
            Assert.AreEqual(null, converter.ReadJson(null, null, null, null));

        }

        [TestMethod]
        public void CanRead()
        {
            var converter = new EnumDescriptionJsonConverter();
            Assert.AreEqual(false, converter.CanRead);
        }

        [TestMethod]
        public void CanConvert()
        {
            var obj = "";
            var converter = new EnumDescriptionJsonConverter();

            Assert.AreEqual(false, converter.CanConvert(obj.GetType()));
        }
    }
}
