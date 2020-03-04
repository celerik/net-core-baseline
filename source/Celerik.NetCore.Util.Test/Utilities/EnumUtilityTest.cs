﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Celerik.NetCore.Util.Test
{
    public enum SouthParkCharacterType
    {
        [Code("EC")]
        [System.ComponentModel.Description("Eric Cartman")]
        [System.ComponentModel.Category("Cool")]
        Cartman = 1,

        [Code("KM")]
        [System.ComponentModel.Description("Kenny McCormick")]
        [System.ComponentModel.Category("Poor")]
        Kenny = 2,

        [Code("KB")]
        [System.ComponentModel.Description("Kyle Broflovski")]
        [System.ComponentModel.Category("Killjoy")]
        Kyle = 3,

        [Code("SM")]
        [System.ComponentModel.Description("Stan Marsh")]
        [System.ComponentModel.Category("Normal")]
        Stan = 4,

        Chef = 5
    }

    public class SouthParkCharacter
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class SouthParkCharacterWithCode : SouthParkCharacter
    {
        public string Code { get; set; }
    }

    [TestClass]
    public class EnumUtilityTest : UtilBaseTest
    {
        [TestMethod]
        public void GetValue()
        {
            var enumDescription = "Eric Cartman";
            var enumValue = EnumUtility.GetValue<SouthParkCharacterType>(enumDescription);
            Assert.AreEqual(SouthParkCharacterType.Cartman, enumValue);

            enumDescription = "Cartman";
            enumValue = EnumUtility.GetValue<SouthParkCharacterType>(enumDescription);
            Assert.AreEqual(SouthParkCharacterType.Cartman, enumValue);

            enumDescription = null;
            enumValue = EnumUtility.GetValue(enumDescription, defaultVal: SouthParkCharacterType.Chef);
            Assert.AreEqual(SouthParkCharacterType.Chef, enumValue);

            TestUtility.AssertThrows<InvalidOperationException>(() => {
                EnumUtility.GetValue<int>(null);
            });
        }

        [TestMethod]
        public void GetAttribute()
        {
            var enumValue = SouthParkCharacterType.Kenny;
            var category = enumValue.GetAttribute<System.ComponentModel.CategoryAttribute>();
            Assert.AreEqual("Poor", category.Category);

            enumValue = SouthParkCharacterType.Chef;
            category = enumValue.GetAttribute<System.ComponentModel.CategoryAttribute>();
            Assert.AreEqual(null, category);
        }

        [TestMethod]
        public void GetCodeFromEnum()
        {
            var enumValue = SouthParkCharacterType.Kyle;
            var enumCode = enumValue.GetCode();
            Assert.AreEqual("KB", enumCode);

            enumValue = SouthParkCharacterType.Chef;
            enumCode = enumValue.GetDescription();
            Assert.AreEqual("Chef", enumCode);
        }

        [TestMethod]
        public void GetCodeFromInt()
        {
            var enumValue = 4;
            var enumCode = EnumUtility.GetCode<SouthParkCharacterType>(enumValue);
            Assert.AreEqual("SM", enumCode);

            enumValue = 5;
            enumCode = EnumUtility.GetCode<SouthParkCharacterType>(enumValue);
            Assert.AreEqual("Chef", enumCode);

            enumValue = 6;
            enumCode = EnumUtility.GetCode<SouthParkCharacterType>(enumValue, defaultVal: "Randy");
            Assert.AreEqual("Randy", enumCode);

            TestUtility.AssertThrows<InvalidOperationException>(() => {
                EnumUtility.GetCode<int>(0);
            });
        }

        [TestMethod]
        public void GetDescriptionFromEnum()
        {
            var enumValue = SouthParkCharacterType.Kyle;
            var enumDescription = enumValue.GetDescription();
            Assert.AreEqual("Kyle Broflovski", enumDescription);

            enumValue = SouthParkCharacterType.Chef;
            enumDescription = enumValue.GetDescription();
            Assert.AreEqual("Chef", enumDescription);
        }

        [TestMethod]
        public void GetDescriptionFromInt()
        {
            var enumValue = 4;
            var enumDescription = EnumUtility.GetDescription<SouthParkCharacterType>(enumValue);
            Assert.AreEqual("Stan Marsh", enumDescription);

            enumValue = 5;
            enumDescription = EnumUtility.GetDescription<SouthParkCharacterType>(enumValue);
            Assert.AreEqual("Chef", enumDescription);

            enumValue = 6;
            enumDescription = EnumUtility.GetDescription<SouthParkCharacterType>(enumValue, defaultVal: "Randy");
            Assert.AreEqual("Randy", enumDescription);

            TestUtility.AssertThrows<InvalidOperationException>(() => {
                EnumUtility.GetDescription<int>(0);
            });
        }

        [TestMethod]
        public void GetMin()
        {
            Assert.AreEqual(1, EnumUtility.GetMin<SouthParkCharacterType>());

            TestUtility.AssertThrows<InvalidOperationException>(() => {
                EnumUtility.GetMin<int>();
            });
        }

        [TestMethod]
        public void GetMax()
        {
            Assert.AreEqual(5, EnumUtility.GetMax<SouthParkCharacterType>());

            TestUtility.AssertThrows<InvalidOperationException>(() => {
                EnumUtility.GetMax<int>();
            });
        }

        [TestMethod]
        public void ToStringList()
        {
            var southParkList = EnumUtility.ToList<SouthParkCharacterType>();

            Assert.AreEqual(5, southParkList.Count);
            Assert.AreEqual("Eric Cartman", southParkList[0]);
            Assert.AreEqual("Kenny McCormick", southParkList[1]);
            Assert.AreEqual("Kyle Broflovski", southParkList[2]);
            Assert.AreEqual("Stan Marsh", southParkList[3]);
            Assert.AreEqual("Chef", southParkList[4]);
        }

        [TestMethod]
        public void ToObjectListNoCodes()
        {
            var southParkList = EnumUtility.ToList<SouthParkCharacterType, SouthParkCharacter>("Id", "Name");

            Assert.AreEqual(5, southParkList.Count);

            Assert.AreEqual(1, southParkList[0].Id);
            Assert.AreEqual("Eric Cartman", southParkList[0].Name);

            Assert.AreEqual(2, southParkList[1].Id);
            Assert.AreEqual("Kenny McCormick", southParkList[1].Name);

            Assert.AreEqual(3, southParkList[2].Id);
            Assert.AreEqual("Kyle Broflovski", southParkList[2].Name);

            Assert.AreEqual(4, southParkList[3].Id);
            Assert.AreEqual("Stan Marsh", southParkList[3].Name);

            Assert.AreEqual(5, southParkList[4].Id);
            Assert.AreEqual("Chef", southParkList[4].Name);

            TestUtility.AssertThrows<InvalidOperationException>(() => {
                EnumUtility.ToList<int, SouthParkCharacter>("Id", "Name");
            });
        }

        [TestMethod]
        public void ToObjectListWithCodes()
        {
            var southParkList = EnumUtility.ToList<SouthParkCharacterType, SouthParkCharacterWithCode>("Id", "Name", "Code");

            Assert.AreEqual(5, southParkList.Count);

            Assert.AreEqual(1, southParkList[0].Id);
            Assert.AreEqual("Eric Cartman", southParkList[0].Name);
            Assert.AreEqual("EC", southParkList[0].Code);

            Assert.AreEqual(2, southParkList[1].Id);
            Assert.AreEqual("Kenny McCormick", southParkList[1].Name);
            Assert.AreEqual("KM", southParkList[1].Code);

            Assert.AreEqual(3, southParkList[2].Id);
            Assert.AreEqual("Kyle Broflovski", southParkList[2].Name);
            Assert.AreEqual("KB", southParkList[2].Code);

            Assert.AreEqual(4, southParkList[3].Id);
            Assert.AreEqual("Stan Marsh", southParkList[3].Name);
            Assert.AreEqual("SM", southParkList[3].Code);

            Assert.AreEqual(5, southParkList[4].Id);
            Assert.AreEqual("Chef", southParkList[4].Name);
            Assert.AreEqual(null, southParkList[4].Code);
        }
    }
}