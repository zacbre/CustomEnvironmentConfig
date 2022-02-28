using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CustomEnvironmentConfig.Exceptions;
using Newtonsoft.Json;

namespace CustomEnvironmentConfig
{
    public class ConfigurationWriter
    {
        public static void WriteToFile<T>(T src, string filePath, bool generateComments = false)
        {
            var configurationWriter = new ConfigurationWriter();
            configurationWriter.Write(src, filePath, generateComments);
        }
        
        public static void WriteToFile<T>(T src, string filePath, Func<string, string, string> encryptHandler, bool generateComments = false)
        {
            var configurationWriter = new ConfigurationWriter();
            configurationWriter.Write(src, filePath, encryptHandler, generateComments);
        }

        public void Write<T>(T src, string filePath, bool generateComments = false)
        {
            var file = File.CreateText(filePath);
            WriteProperties(src, typeof(T), null, new Stack<Type>(), file, encryptHandler: null, generateComments);
            file.Close();
        }

        public void Write<T>(T src, string filePath, Func<string, string, string> encryptHandler, bool generateComments = false)
        {
            var file = File.CreateText(filePath);
            WriteProperties(src, typeof(T), null, new Stack<Type>(), file, encryptHandler, generateComments);
            file.Close();
        }
        
        private void WriteProperties<T>(T instance, Type type, string? prefix, Stack<Type> recursive, StreamWriter fileStream, Func<string,string,string>? encryptHandler, bool generateComments)
        {
            Push(recursive, type);

            var subItems = new Dictionary<string, PropertyInfo>();
            
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.MemberType != MemberTypes.Property)
                {
                    continue;
                }

                var itemName = prop.Name;
                var jsonEncode = false;

                var configItemAttr = prop.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(ConfigurationItem));
                if (configItemAttr is ConfigurationItem cAttr)
                {
                    itemName = cAttr.Name;
                    jsonEncode = cAttr.Json;

                    if (cAttr.Ignore)
                    {
                        continue;
                    }
                }

                var isNullable = Nullable.GetUnderlyingType(prop.PropertyType);
                
                if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string) || prop.PropertyType.IsEnum || isNullable is {} || jsonEncode)
                {
                    var value = prop.GetValue(instance);

                    if (value is {})
                    {
                        if (jsonEncode)
                        {
                            var serialized = JsonConvert.SerializeObject(value);
                            fileStream.WriteLine($"{(prefix is {} ? $"{prefix + "_"}{itemName}" : itemName)} = {serialized}");
                            continue;
                        }
                        
                        // try to cast env to that type.
                        string? convertedVal = (string?)Convert.ChangeType(value, typeof(string));
                        
                        // if valueHandler is set, process using this method, to encrypt values, etc.
                        if (encryptHandler is {} && convertedVal is {} && configItemAttr is ConfigurationItem { Encrypt: true })
                        {
                            convertedVal = encryptHandler($"{(prefix is { } ? $"{prefix + "_"}{itemName}" : itemName)}", convertedVal);
                        }

                        fileStream.WriteLine($"{(prefix is {} ? $"{prefix + "_"}{itemName}" : itemName)} = {convertedVal}");
                    }
                    else
                    {
                        if (generateComments)
                        {
                            fileStream.WriteLine($"# {(prefix is {} ? $"{prefix + "_" ?? ""}{itemName}" : itemName)} = null");
                        }
                    }
                }
                else
                {
                    subItems.Add(itemName, prop);
                }
                
            }

            foreach (var item in subItems)
            {
                var itemName = item.Key;
                var subInstance = item.Value.GetValue(instance);
                
                if (generateComments)
                {
                    fileStream.WriteLine();
                    fileStream.Write($"#\n# {(prefix is {} ? prefix + "_" : "")}{itemName}\n#\n");
                }
                
                WriteProperties(subInstance, item.Value.PropertyType, $"{(prefix is {} ? prefix + "_" : "")}{itemName}", recursive, fileStream, encryptHandler, generateComments);
            }

            recursive.Pop();
        }
        
        private void Push(Stack<Type> stack, Type item)
        {
            if (stack.Contains(item))
            {
                throw new RecursiveClassException($"Class instantiation loop detected... Type already initialized: {item.ToString()}");
            }

            stack.Push(item);
        }
    }
}