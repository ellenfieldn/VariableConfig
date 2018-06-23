Feel free to use the issue tracker for bugs reports, feature requests, and support.

For bug reports and support, please use the template below. To increase the chances of getting timely help for your issue, please include all of the information specified in the template!

### Description
A brief description of the problem you are having.

### Steps to Reproduce
#### How are you building your configuration?
A code snippet is preferred. Ex:
```C#
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddXmlFile("myConfig.xml")
    .AddVariableConfiguration();
Configuration = builder.Build();
```

#### What is your configuration?
A dump of your configuration file. If you have a large file, please shorten it to contain only the relevant bits. **Make sure to sanitize any sensitive information!!!**
Ex:
```xml
<AppConfiguration>
  <Variable>VarValue</Variable>
  <Composite>${Variable}Comp1</Composite>
</AppConfiguration>
```

#### How are you accessing your configuration?
Again, a code snippet showing exactly how you are getting your configuration is best:
```C#
var myValue = Configuration["Composite"];
```

#### What are the expected and actual results?
* **Expected:** ExpectedValue
* **Actual:** ActualValue
