using System;
using System.Reflection;

namespace CustomEnvironmentConfig;

public class RequiredProperty(string? prefix, string name, PropertyInfo prop, Type type)
{
    public string? Prefix { get; } = prefix;
    public string Name { get; } = name;
    public PropertyInfo Prop { get; } = prop;
    public Type Type { get; } = type;       
}