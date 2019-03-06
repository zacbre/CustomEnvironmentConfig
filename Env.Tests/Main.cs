using System;
using System.Collections.Generic;
using System.Data;
using Env.Exceptions;
using Env.Repositories;
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
                { "SubItem_SubSubItem_Bool", "true" },
                { "SubItem_SubSubItem_Int", "10" },
                { "SubItem_SubSubItem_Long", "10000000" },
                { "SubItem_SubSubItem_Double", "2.2" },
                { "SubItem_SubSubItem_Float", "2.22" }
            };
            // Set the environment variables we're going to use.
            EnvironmentVariableRepository.SetEnvironment(dict);
            
            var instance = ConfigurationParser.ParseConfiguration<TestClass>();
            Assert.Equal("TEST_VAL_ITEM", instance.Item);
            Assert.Equal("TEST_VAL_SUBITEM_ITEM", instance.SubItem.Item);
            Assert.Equal("TEST_VAL_SUBITEM_SUBSUBITEM_ITEM", instance.SubItem.SubSubItem.Item);
            Assert.True(instance.SubItem.SubSubItem.Bool);
            Assert.Equal(10, instance.SubItem.SubSubItem.Int);
            Assert.Equal(10000000, instance.SubItem.SubSubItem.Long);
            Assert.Equal(2.2D, instance.SubItem.SubSubItem.Double);
            Assert.Equal(2.22F, instance.SubItem.SubSubItem.Float);
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

        [Fact]
        public void Can_Parse_A_File()
        {
            var values = new[]
            {
                "Test=test",
                "Test1=1",
                "Test2=true",
            };
            
            var parser = new ConfigurationParser(new EnvironmentFileRepository(values));
            var output = parser.ParseConfiguration<CanParseAFile>();
            
            Assert.Equal("test", output.Test);
            Assert.Equal(1, output.Test1);
            Assert.True(output.Test2);
        }

        [Fact]
        public void Duplicates_Are_Detected_From_File()
        {
            var values = new[]
            {
                "Test=test",
                "test=test1",
                "Test=test2",
            };
            
            // We make sure we can handle case sensitive items.
            Assert.Throws<DuplicateNameException>(() =>
            {
                var parser = new ConfigurationParser(new EnvironmentFileRepository(values));                
            });           
        }

        [Fact]
        public void Can_Ignore_Comments_And_Empty_Lines()
        {
            var values = new[]
            {
                "#Comment1=Test",
                "",
                "Test=test",
                "# comment 2",
                "test=test1",
                "",
                "#Comment 3",
            };
            
            var parser = new ConfigurationParser(new EnvironmentFileRepository(values));
            var output = parser.ParseConfiguration<CaseSensitiveClass>();
            Assert.Equal("test", output.Test);
            Assert.Equal("test1", output.test);
        }

        [Fact]
        public void Prefer_Environment_Over_File()
        {
            var values = new[]
            {
                "Test=fromFile",
                "test=test1",
            };

            var dict = new Dictionary<string, string>
            {
                {"Test", "fromEnv"}
            };
            
            EnvironmentVariableRepository.SetEnvironment(dict);
            var parser = new ConfigurationParser(new EnvironmentFileRepository(EnvironmentVariableRepository, values));
            var output = parser.ParseConfiguration<CaseSensitiveClass>();
            Assert.Equal("fromEnv", output.Test);
            Assert.Equal("test1", output.test);
        }
    }
}