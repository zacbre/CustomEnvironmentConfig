using System;
using System.Collections.Generic;
using Env.Exceptions;
using Env.Tests.Configuration;
using Xunit;

namespace Env.Tests
{
    public class Main : BaseTest
    {
        [Fact]
        public void Can_Properly_Deserialize()
        {
            var dict = new Dictionary<string, string>
            {
                { "Item", "TEST_VAL_ITEM" },
                { "SubItem_Item", "TEST_VAL_SUBITEM_ITEM" },
                { "SubItem_SubSubItem_Item", "TEST_VAL_SUBITEM_SUBSUBITEM_ITEM" },
            };
            // Set the environment variables we're going to use.
            EnvironmentVariableRepository.SetEnvironment(dict);
            
            var instance = ConfigurationParser.ParseConfiguration<TestClass>();
            Assert.Equal("TEST_VAL_ITEM", instance.Item);
            Assert.Equal("TEST_VAL_SUBITEM_ITEM", instance.SubItem.Item);
            Assert.Equal("TEST_VAL_SUBITEM_SUBSUBITEM_ITEM", instance.SubItem.SubSubItem.Item);
        }

        [Fact]
        public void Does_Not_Allow_Recursive_Items()
        {
            Assert.Throws<RecursiveClassException>(() =>
            {
                ConfigurationParser.ParseConfiguration<RecursiveClass>();                
            });
        }
        
        [Fact]
        public void Allows_Class_To_Be_Used_More_Than_Once()
        {
            var dict = new Dictionary<string, string>
            {
                { "SubClass1_Item", "Val1" },
                { "SubClass2_Item", "Val2" },
                { "SubClass3_Item", "Val3" },
            };
            // Set the environment variables we're going to use.
            EnvironmentVariableRepository.SetEnvironment(dict);
            var instance = ConfigurationParser.ParseConfiguration<ReUsableClass>();
            Assert.Equal("Val1", instance.SubClass1.Item);
            Assert.Equal("Val2", instance.SubClass2.Item);
            Assert.Equal("Val3", instance.SubClass3.Item);
        }

        [Fact]
        public void Throws_Exception_When_Item_Is_Required_And_Not_Found()
        {
            var dict = new Dictionary<string, string>();
            // Set the environment variables we're going to use.
            EnvironmentVariableRepository.SetEnvironment(dict);
            
            Assert.Throws<KeyNotFoundException>(() =>
            {
                ConfigurationParser.ParseConfiguration<RequiredItemClass>();
            });
        }

        [Fact]
        public void Allows_Items_To_Be_Missing_If_Not_Required()
        {
            var dict = new Dictionary<string, string>
            {
                { "RequiredItem", "Test" },
            };
            // Set the environment variables we're going to use.
            EnvironmentVariableRepository.SetEnvironment(dict);
            
            var instance = ConfigurationParser.ParseConfiguration<NotRequiredItemClass>();
            Assert.Null(instance.NotRequiredItem);
            Assert.Equal("Test", instance.RequiredItem);
        }
    }
}