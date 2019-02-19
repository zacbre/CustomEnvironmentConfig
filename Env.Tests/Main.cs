using System;
using System.Collections.Generic;
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
            Assert.Throws<StackOverflowException>(() =>
            {
                ConfigurationParser.ParseConfiguration<RecursiveClass>();                
            });
        }

        [Fact]
        public void Throws_Exception_When_Item_Is_Required_And_Not_Found()
        {
            var dict = new Dictionary<string, string>();
            // Set the environment variables we're going to use.
            EnvironmentVariableRepository.SetEnvironment(dict);
            
            Assert.Throws<Exception>(() =>
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