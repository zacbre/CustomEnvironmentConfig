# dotnetcore-custom-env
Enables binding environment variables and/or environment files to classes.

[![Nuget](https://img.shields.io/nuget/v/CustomEnvironmentConfig.svg)](https://www.nuget.org/packages/CustomEnvironmentConfig/)

## Example

**(.env [environment variables])**
```
MyConfigItem=Test Value
Subclass_MyConfigSubItem=Test Subitem Value
```

**(MyConfiguration.cs)**
```
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
```
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseEnvironmentConfiguration<MyConfiguration>()               
            .....
```

**(Startup.cs)**
```
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

If you want, you can re-map the name of the items using [ConfigItem]:
(Do note, [ConfigItem] is not required and only needed if you want to set the requirement policy of a property
or change the name of the environment variable)
```
public class MyConfiguration
{
    [ConfigItem("MyItem")]
    public bool Item { get; set; }
    // OR
    [ConfigItem(Name = "MyOtherItem")]
    public int OtherItem { get; set; }
    
    [ConfigItem("Test")
    public SubConfiguration MySubClass { get; set; }
}
public class SubConfiguration
{
    [ConfigItem]
    public bool SubItem { get; set; }
}
```

If you want to ignore parsing certain properties in a class, you can use [IgnoreConfigItem]:
```
public class MyConfiguration
{
    [ConfigItem("MyItem")]
    public bool Item { get; set; }
    
    public int OtherItem { get; set; }
    
    [IgnoreConfigItem]
    public SubConfiguration MySubClass { get; set; }
}
```

The environment variables for this would look like as follows:
```
MyItem=Value
MyOtherItem=Value
Test_SubItem=Value
```

You can also set if the item is required to be set in the environment or not.
By default, items are required.
```
public class MyConfiguration
{
    [ConfigItem(Required = ConfigItemRequirement.NotRequired)]
    public bool NotRequiredItem { get; set; }
    // OR
    [ConfigItem(Name = "MyOtherItem", Required = ConfigItemRequirement.Required)]
    public int OtherItem { get; set; }
}
```

## From an Env File:

Most of this applies from above, except instead your IWebHostBuilder would look like:
```
// Program.cs
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseEnvironmentConfiguration<MyConfiguration>(fileName: "filename.env", configurationTypeEnum: ConfigurationTypeEnum.PreferEnvironment)               
            .....
```

## Non-DI:

To parse environment variables directly:
```
public void MyFunction() 
{
    var output = ConfigurationParser.Parse<MyClass>();
    // Access your class via output variable
}
```

To parse from an environment file:
```
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
```
// Program.cs
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseEnvironmentConfiguration<MyConfiguration>(fileName: "filename.env", configurationTypeEnum: ConfigurationTypeEnum.PreferEnvironment)               
            .....
```
### Non-DI
```
public void MyFunction() 
{
    var output = ConfigurationParser.Parse<MyClass>(fileName: "file.env", configurationTypeEnum: ConfigurationTypeEnum.PreferEnvironment);
    .....
}
```