using System;
using System.Collections.Generic;
using System.Data;
using CustomEnvironmentConfig.Exceptions;
using CustomEnvironmentConfig.Tests.Configuration;
using Xunit;

namespace CustomEnvironmentConfig.Tests
{
    public class UnitTests : BaseTest
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
            EnvironmentVariableSource.SetEnvironment(dict);
            
            var instance = ConfigurationParser.ParseConfiguration<TestClass>();
            Assert.Equal("TEST_VAL_ITEM", instance.Item);
            Assert.Equal("TEST_VAL_SUBITEM_ITEM", instance.SubItem?.Item);
            Assert.Equal("TEST_VAL_SUBITEM_SUBSUBITEM_ITEM", instance.SubItem?.SubSubItem?.Item);
            Assert.True(instance.SubItem?.SubSubItem?.Bool);
            Assert.Equal(10, instance.SubItem?.SubSubItem?.Int);
            Assert.Equal(10000000, instance.SubItem?.SubSubItem?.Long);
            Assert.Equal(2.2D, instance.SubItem?.SubSubItem?.Double);
            Assert.Equal(2.22F, instance.SubItem?.SubSubItem?.Float);
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
            EnvironmentVariableSource.SetEnvironment(dict);
            var instance = ConfigurationParser.ParseConfiguration<ReUsableClass>();
            Assert.Equal("Val1", instance?.SubClass1?.Item);
            Assert.Equal("Val2", instance?.SubClass2?.Item);
            Assert.Equal("Val3", instance?.SubClass3?.Item);
        }

        [Fact]
        public void Throws_Exception_When_Item_Is_Required_And_Not_Found()
        {
            var dict = new Dictionary<string, string>();
            // Set the environment variables we're going to use.
            EnvironmentVariableSource.SetEnvironment(dict);
            
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
            EnvironmentVariableSource.SetEnvironment(dict);
            
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

            FileSource.SetEnvironment(values);
            var output = ConfigurationParser.Parse<CanParseAFile>(EnvironmentVariableRepository);
            
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
                FileSource.SetEnvironment(values);
                ConfigurationParser.Parse<CanParseAFile>(EnvironmentVariableRepository);    
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
            
            FileSource.SetEnvironment(values);
            var output = ConfigurationParser.Parse<CaseSensitiveClass>(EnvironmentVariableRepository);
            Assert.Equal("test", output.Test);
            Assert.Equal("test1", output.test);
        }

        [Fact]
        public void Can_Trim_Quotes_And_Spaces()
        {
            var values = new[]
            {
                "Test = 'test'",
                "Test1=\"1\"",
                "Test2='true'",
            };
            
            FileSource.SetEnvironment(values);
            var output = ConfigurationParser.Parse<CanParseAFile>(EnvironmentVariableRepository);
            Assert.Equal("test", output.Test);
            Assert.Equal(1, output.Test1);
            Assert.True(output.Test2);
        }

        [Fact]
        public void Prefer_Environment_Over_File()
        {
            var values = new[]
            {
                "Test=fromFile",
            };

            var dict = new Dictionary<string, string>
            {
                {"Test", "fromEnv"},
                {"test", "fromEnv"}
            };
            
            EnvironmentVariableSource.SetEnvironment(dict);
            FileSource.SetEnvironment(values);
            EnvironmentVariableRepository.SetConfigurationType(ConfigurationTypeEnum.PreferEnvironment);
            var output = ConfigurationParser.Parse<PreferClass>(EnvironmentVariableRepository);
            Assert.Equal("fromEnv", output.Test);
            Assert.Equal("fromEnv", output.test);
        }
        
        [Fact]
        public void Prefer_File_Over_Environment()
        {
            var values = new[]
            {
                "Test=fromFile",
                "test=fromFile"
            };

            var dict = new Dictionary<string, string>
            {
                {"Test", "fromEnv"},
            };
            
            EnvironmentVariableSource.SetEnvironment(dict);
            FileSource.SetEnvironment(values);
            EnvironmentVariableRepository.SetConfigurationType(ConfigurationTypeEnum.PreferFile);
            var output = ConfigurationParser.Parse<PreferClass>(EnvironmentVariableRepository);
            Assert.Equal("fromFile", output.Test);
            Assert.Equal("fromFile", output.test);
        }
        
        [Fact]
        public void File_Only()
        {
            var values = new[]
            {
                "Test=fromFile",
            };

            var dict = new Dictionary<string, string>
            {
                {"Test", "fromEnv"},
            };
            
            EnvironmentVariableSource.SetEnvironment(dict);
            FileSource.SetEnvironment(values);
            EnvironmentVariableRepository.SetConfigurationType(ConfigurationTypeEnum.FileOnly);
            var output = ConfigurationParser.Parse<ConfigurationClass>(EnvironmentVariableRepository);
            Assert.Equal("fromFile", output.Test);
        }
        
        [Fact]
        public void Environment_Only()
        {
            var values = new[]
            {
                "Test=fromFile",
            };

            var dict = new Dictionary<string, string>
            {
                {"Test", "fromEnv"}
            };
            
            EnvironmentVariableSource.SetEnvironment(dict);
            FileSource.SetEnvironment(values);
            EnvironmentVariableRepository.SetConfigurationType(ConfigurationTypeEnum.EnvironmentOnly);
            var output = ConfigurationParser.Parse<ConfigurationClass>(EnvironmentVariableRepository);
            Assert.Equal("fromEnv", output.Test);
        }

        [Fact]
        public void Check_For_Nullable()
        {
            var dict = new Dictionary<string, string>
            {
                {"Test", "20"},
                {"TestBool", "true"}
            };
            
            EnvironmentVariableSource.SetEnvironment(dict);
            EnvironmentVariableRepository.SetConfigurationType(ConfigurationTypeEnum.PreferEnvironment);
            var output = ConfigurationParser.Parse<NullableClass>(EnvironmentVariableRepository);
            Assert.Equal(20, output.Test);
            Assert.True(output.TestBool);
            Assert.Null(output.NotRequired);
        }

        [Fact]
        public void Can_Write_To_File()
        {
            var dict = new Dictionary<string, string>
            {
                {"Test", "20"},
                {"TestBool", "true"}
            };
            
            EnvironmentVariableSource.SetEnvironment(dict);
            EnvironmentVariableRepository.SetConfigurationType(ConfigurationTypeEnum.PreferEnvironment);
            var output = ConfigurationParser.Parse<NullableClass>(EnvironmentVariableRepository);
            
            ConfigurationWriter.WriteToFile(output, "cwtf.txt", true);
            
            // Read in that file.
            var parsedConfig = ConfigurationParser.Parse<NullableClass>("cwtf.txt");
            Assert.Equal(20, parsedConfig.Test);
            Assert.True(parsedConfig.TestBool);
        }
        
        [Fact]
        public void Can_Write_Recursive_Class_To_File()
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
            EnvironmentVariableSource.SetEnvironment(dict);
            
            var parsed = ConfigurationParser.ParseConfiguration<TestClass>();
            
            ConfigurationWriter.WriteToFile(parsed, "cwrctf.txt", true);
            
            // Read in that file.
            var instance = ConfigurationParser.Parse<TestClass>("cwrctf.txt");
            Assert.Equal("TEST_VAL_ITEM", instance.Item);
            Assert.Equal("TEST_VAL_SUBITEM_ITEM", instance?.SubItem?.Item);
            Assert.Equal("TEST_VAL_SUBITEM_SUBSUBITEM_ITEM", instance?.SubItem?.SubSubItem?.Item);
            Assert.True(instance?.SubItem?.SubSubItem?.Bool);
            Assert.Equal(10, instance?.SubItem?.SubSubItem?.Int);
            Assert.Equal(10000000, instance?.SubItem?.SubSubItem?.Long);
            Assert.Equal(2.2D, instance?.SubItem?.SubSubItem?.Double);
            Assert.Equal(2.22F, instance?.SubItem?.SubSubItem?.Float);
        }

        [Fact]
        public void Can_Set_Default_Value()
        {
            var parsed = ConfigurationParser.ParseConfiguration<DefaultValueClass>();
            
            ConfigurationWriter.WriteToFile(parsed, "csdv.txt", true);
            
            // Read in that file.
            var instance = ConfigurationParser.Parse<DefaultValueClass>("csdv.txt");
            Assert.True(instance.HasDefault);
            Assert.False(instance.DoesNotHaveDefault);
        }

        [Fact]
        public void Can_Set_Default_String_Value()
        {
            var parsed = ConfigurationParser.ParseConfiguration<DefaultValueStringClass>();
            
            Assert.Equal("i have a value!", parsed.DoIHaveAValue);
        }
        
        [Fact]
        public void Can_Convert_Weird_Value_Types()
        {
            var parsed = ConfigurationParser.ParseConfiguration<DefaultValueWeirdTypesClass>();
            
            ConfigurationWriter.WriteToFile(parsed, "ccwvt.txt", true);
            
            // Read in that file.
            var instance = ConfigurationParser.Parse<DefaultValueWeirdTypesClass>("ccwvt.txt");
            Assert.True(instance.HasDefault);
            Assert.True(instance.DoesNotHaveDefault);
        }
        
        [Fact]
        public void Cannot_Set_Value_If_Wrong_Type()
        {
            var msg = Assert.Throws<FormatException>(() =>
            {
                ConfigurationParser.ParseConfiguration<DefaultValueBrokenClass>();
            });

            Assert.Contains("Input string was not in a correct format", msg.Message);
        }

        [Fact]
        public void Can_Read_And_Write_And_Parse_Enums()
        {
            var config = new EnumClass
            {
                Enum = TestEnum.Item2,
            };

            ConfigurationWriter.WriteToFile(config, "crawape.txt", true);
            
            var configParsed = ConfigurationParser.Parse<EnumClass>("crawape.txt");
            Assert.Equal(TestEnum.Item2, configParsed.Enum);
        }
        
        [Fact]
        public void Can_Read_Default_Enum_Values()
        {
            var dict = new Dictionary<string, string>
            {
                { "Enum", "Item2" },
            };
            // Set the environment variables we're going to use.
            EnvironmentVariableSource.SetEnvironment(dict);
            
            var configParsed = ConfigurationParser.ParseConfiguration<EnumClass>();
            Assert.Equal(TestEnum.Item1, configParsed.NotRequired);
            Assert.Equal(TestEnum.Item2, configParsed.Enum);
        }

        [Fact]
        public void Throws_When_Enum_Fails_To_Parse()
        {
            var dict = new Dictionary<string, string>
            {
                { "Enum", "Item3" },
            };
            // Set the environment variables we're going to use.
            EnvironmentVariableSource.SetEnvironment(dict);

            var msg = Assert.Throws<Exception>(() => ConfigurationParser.ParseConfiguration<EnumClass>());
            Assert.Contains("Could not parse", msg.Message);
        }
    }
}