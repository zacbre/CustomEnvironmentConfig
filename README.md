# CustomEnvironmentConfig
Enables binding environment variables and/or environment files to classes.

[![Nuget](https://img.shields.io/nuget/v/CustomEnvironmentConfig.svg)](https://www.nuget.org/packages/CustomEnvironmentConfig/)

## Changelog
```
03/01/2022 [v1.7.0]
- Add support for complex json objects
- Add .net 6 support
10/04/2021 [v1.6.3]
- Add support for POSIX style environment variables.
- Add builder extensions for POSIX.
11/20/2020 [v1.6.0]
- Add support for encrypting fields.
07/08/2020 [v1.5.0]
- Updated to netcoreapp3.1.
07/08/2020 [v1.4.1]
- Added support for enums.
```

## Examples

**(.env [environment variables])**
```
MyConfigItem=Test Value
Subclass_MyConfigSubItem=Test Subitem Value
```

**(MyConfiguration.cs)**
```c#
public class MyConfiguration 
{
    public string MyConfigItem { get; set; }
    
    public MyConfigSubClass Subclass { get; set; }
}

public class MyConfigSubClass 
{
    public string MyConfigSubItem { get; set; }
}
```

**(Program.cs)**
```c#
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseEnvironmentConfiguration<MyConfiguration>()               
            .....
```

**(Startup.cs)**
```c#
public class Startup 
{
    private readonly MyConfiguration _configuration;
    
    public Startup(MyConfiguration configuration) 
    {
        _configuration = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        Console.WriteLine(_configuration.MyConfigItem);
        Console.WriteLine(_configuration.Subclass.MyConfigSubItem);
        ...
    }
}
```

If you want, you can re-map the name of the items using [ConfigurationItem]:
(Do note, [ConfigurationItem] is not required and only needed if you want to set the requirement policy of a property
or change the name of the environment variable)
```c#
public class MyConfiguration
{
    [ConfigurationItem("MyItem")]
    public bool Item { get; set; }
    // OR
    [ConfigurationItem(Name = "MyOtherItem")]
    public int OtherItem { get; set; }
    
    [ConfigurationItem("Test")
    public SubConfiguration MySubClass { get; set; }
}
public class SubConfiguration
{
    [ConfigurationItem]
    public bool SubItem { get; set; }
}
```

If you want to ignore parsing certain properties in a class, you can use [ConfigurationItem(Ignore = true)]:
```c#
public class MyConfiguration
{
    [ConfigurationItem("MyItem")]
    public bool Item { get; set; }
    
    public int OtherItem { get; set; }
    
    [ConfigurationItem(Ignore = true)]
    public SubConfiguration MySubClass { get; set; }
}
```

The environment variables for this would look like as follows:
```c#
MyItem=Value
MyOtherItem=Value
Test_SubItem=Value
```

You can also set if the item is required to be set in the environment or not.
By default, items are required.
```c#
public class MyConfiguration
{
    [ConfigurationItem(Required = false)]
    public bool NotRequiredItem { get; set; }
    // OR
    [ConfigurationItem(Name = "MyOtherItem", Required = true)]
    public int OtherItem { get; set; }
}
```

If you want to specify a default value for an item if it's not required:
```c#
public class MyConfiguration
{
    [ConfigurationItem(Required = false, Default = true)]
    public bool NotRequiredItem { get; set; }
    // OR
    [ConfigurationItem(Required = false, Default = 123)]
    public int OtherItem { get; set; }
}
```
Conversions across types are supported for defaults, for instance:
```c#
(string)"123" => (int)123
(string)"true" => (bool)True
(string)"false" => (bool)False
(int)1 => (bool)True
(int)0 => (bool)False
...etc
```

## From an Env File:

Most of this applies from above, except instead your IWebHostBuilder would look like:
```c#
// Program.cs
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseEnvironmentConfiguration<MyConfiguration>(fileName: "filename.env", configurationTypeEnum: ConfigurationTypeEnum.PreferEnvironment)               
            .....
```

## Non-DI:

To parse environment variables directly:
```c#
public void MyFunction() 
{
    var output = ConfigurationParser.Parse<MyClass>();
    // Access your class via output variable
}
```

To parse from an environment file:
```c#
public void MyFunction() 
{
    var output = ConfigurationParser.Parse<MyClass>(fileName: "file.env");
    // Access your class via output variable
}
```

## Preferences
You can choose one of the following preferences:
- Prefer Environment Over File
- Prefer File Over Environment
- Use Environment Only
- Use File Only

The default functionality is Prefer Environment over File.

To use this functionality, there's two ways, both DI and non-DI:

### DI
```c#
// Program.cs
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseEnvironmentConfiguration<MyConfiguration>(fileName: "filename.env", configurationTypeEnum: ConfigurationTypeEnum.PreferEnvironment)               
            .....
```
### Non-DI
```c#
public void MyFunction() 
{
    var output = ConfigurationParser.Parse<MyClass>(fileName: "file.env", configurationTypeEnum: ConfigurationTypeEnum.PreferEnvironment);
    .....
}
```
### Manual-DI (for non asp.net core projects)
```c#
public void Main() 
{
    var servicesBuilder = new ServiceCollection();

    var output = ConfigurationParser.Parse<MyClass>(fileName: "file.env", configurationTypeEnum: ConfigurationTypeEnum.PreferEnvironment);
    servicesBuilder.AddSingleton(output);

    var services = servicesBuilder.BuildServiceProvider();

    // Access MyClass in constructor or like below
    var config = services.GetService<MyClass>();
    .....
}
```

## POSIX style variables
```c#
public void MyFunction() 
{
    var output = ConfigurationParser.ParseConfigurationPosix<MyClass>();
    .....
}
// or with options
public void MyFunction() 
{
    var output = ConfigurationParser.ParsePosix<MyClass>(fileName: "file.env", configurationTypeEnum: ConfigurationTypeEnum.PreferEnvironment);
    .....
}
```
or builder syntax:
```c#
// Program.cs
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseEnvironmentConfigurationPosix<MyConfiguration>(fileName: "filename.env", configurationTypeEnum: ConfigurationTypeEnum.PreferEnvironment)               
            .....
```

## Json Support
If you want to support json objects, you can do so via the `Json` attribute.
```c#
[ConfigurationItem(Json = true)]
public List<string> MyList { get; set; }
```
Config:
```
MyList = ["Item1","Item2","Item3","Item4"]
```
#### Multiline
You can also use multiline capability for more easily readable json objects.
```c#
public class MyItem 
{
    public string Name { get; set; }
    public int Value { get; set; }
}

// in your config class:
...
[ConfigurationItem(Json = true)]
public MyItem Item { get; set; }
```
Config:
```
Item = `{
    "Name": "My Item!",
    "Value": 42
}`
```