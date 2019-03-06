# dotnetcore-custom-env
Enables binding environment variables and/or environment files to classes.

[![Nuget](https://img.shields.io/nuget/v/CustomEnvironmentConfig.svg)](https://www.nuget.org/packages/CustomEnvironmentConfig/)

### Example

**(.env [environment variables])**
```
MyConfigItem=Test Value
Subclass_MyConfigSubItem=Test Subitem Value
```

**(MyConfiguration.cs)**
```
public class MyConfiguration 
{
    [ConfigItem]
    public string MyConfigItem { get; set; }
    
    [ConfigItem]
    public MyConfigSubClass Subclass { get; set; }
}

public class MyConfigSubClass 
{
    [ConfigItem]
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

The environment variables for this would look like as follows:
```
MyItem=Value
MyOtherItem=Value
Test_SubItem=Value
```

You can also set if the item is required to be set in the environment or not.
By default, items are set to be required.
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

# From an Env File:

Most of this applies from above, except instead your IWebHostBuilder would look like:
```
// Program.cs
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseEnvironmentConfiguration<MyConfiguration>(fileName: "filename.env", requireFile: true)               
            .....
```

# Non-DI:

To parse environment variables directly:
```
public void MyFunction() 
{
    // Use environment variable repository or your own custom one that implements IEnvironmentVariableRepository
    var parser = new ConfigurationParser(new EnvironmentVariableRepository());
    var output = parser.ParseConfiguration<MyClass>();
    // Access your class via output variable
}
```

To parse from an environment file:
```
public void MyFunction() 
{
    // Use environment variable repository or your own custom one that implements IEnvironmentVariableRepository
    var parser = new ConfigurationParser(new EnvironmentFileRepository(fileName: "filename.env", requireFile: true));
    var output = parser.ParseConfiguration<MyClass>();
    // Access your class via output variable
}
```