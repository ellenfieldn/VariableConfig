# VariableConfig
VariableConfig allows you to reference other configuration properties and use them as variables. It is built on top of the Microsoft.Extensions.Configuration nuget package and supports .NET Core 2.0+ and .Net Framework 4.7+.

[![Build Status](https://ellenfieldn.visualstudio.com/_apis/public/build/definitions/d7ac75f4-5c38-4929-a555-46f4f500827f/3/badge)](https://ellenfieldn.visualstudio.com/VariableConfig/_build/index?&definitionId=3)  
[![VariableConfig](https://img.shields.io/nuget/v/VariableConfig.svg?maxAge=3600)](https://www.nuget.org/packages/VariableConfig/)
## Usage
### 1. Define the configuration
Define configuration using the pattern `${...}` to reference other configuration keys.
```xml
<AppConfiguration>
  <Variable>VarValue</Variable>
  <Composite>${Variable}Comp1</Composite>
</AppConfiguration>
```

### 2. Build the IConfiguration object
VariableConfig wraps any previously added configuration. This means that in most normal circumstances, it should be added last.
```C#
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddXmlFile("myConfig.xml")
    .AddVariableConfiguration();
Configuration = builder.Build();
```

### 3. Load values from configuration
Pull properties from configuration and any variable tokens will be replaced with the value of the configuration item that they refer to.
```C#
var myValue = Configuration["Composite"];
Assert.AreEqual("VarValueComp1", myValue);
```
## Installation
### Prerequisites
.netcore 2.0+ OR .NET 4.7+
### Installation
Nuget:
`PM> Install-Package VariableConfig`

## Configuration Rules
- Anything within `${ }` will be treated as a variable.
- Variables are resolved from configuration as if they were a normal configuration setting.
- A value can be composed of multiple variables.
- Variables can refer to other variables.
- Variables can reference properties in subsections.

### Examples
The following examples show the values that would be returned for a given key using the xml or json files below as the source configuration.
```C#
Configuration["Variable"];                      //"VarValue"
Configuration["Composite"];                     //"VarValueComp1"
Configuration["SuperComposite"]                 //"VarValueComp1ContainsVarValue"
Configuration["ComplexObject:PropertyOnObject"] //"VarValueInProperty"
Configuration["NestedComplexObject"]            //"PropertyIsVarValueInProperty!"
```

#### Xml
```xml
<AppConfiguration>
  <NormalSetting>Value1</NormalSetting>
  <Variable>VarValue</Variable>
  <Composite>${Variable}Comp1</Composite>
  <SuperComposite>${Composite}Contains${Variable}</SuperComposite>
  <ComplexObject>
    <PropertyOnObject>${Variable}</PropertyOnObject>
  </ComplexObject>
  <NestedComplexObject>PropertyIs${ComplexObject:PropertyOnObject}!</NestedComplexObject>
</AppConfiguration>
```
#### Json
```json
{
  "AppConfiguration": {
    "NormalSetting": "Value1",
    "Variable": "VarValue",
    "Composite": "${Variable}Comp1",
    "SuperComposite": "${Composite}Contains${Variable}",
    "ComplexObject":
    {
      "PropertyOnObject": "${Variable}InProperty"
    },
    "NestedComplexObject": "PropertyIs${ComplexObject:PropertyOnObject}!"
  }
}
```

## Advanced Usage

### Building Configuration
Alternatively, the underlying IConfiguration can be added manually by passing an IConfiguration object to AddVariableConfiguration():
```C#
var builder = new ConfigurationBuilder()
    .AddVariableConfiguration(sourceConfiguration);
Configuration = builder.Build();
```

### POCO Objects
It is also possible to use POCO objects in conjunction with Microsoft.Extensions.Configuration.Binder like so:
```C#
var appConfig = Configuration.GetSection("ComplexObject").Get<MyPoco>();
Assert.AreEqual("VarValueInProperty", appConfig.PropertyOnObject);

public class MyPoco
{
    public string PropertyOnObject { get; set; }
}
```
```xml
<AppConfiguration>
  <ComplexObject>
    <PropertyOnObject>${Variable}</PropertyOnObject>
  </ComplexObject>
</AppConfiguration>
```

### Escaping special characters
What if you want to have a setting value that contains `'${.*}'`. all you have to do is escape the sequence `${` by putting a `\` in front of it like `\${`.

If you want a `\` to precede a `${.*}` as in `\${a}`, simply escape it with another `\`. If this is starting to sound a little confusing, see the examples below.

Assuming that there is a variable named 'Z' that contains the value 'zzzz':

| Desired Output | Setting Value       |
| -------------- | ------------------- |
| asd${asdf}asdf  | asd\\${asdf}asdf |
| asd$${asdf}asd  | asd$\\${asdf}asd |
| asdf${zzzz}asd  | asdf\\${${Z}}asd |
| asd$${zzzz}asd  | asd$\\${${Z}}asd |
| asdf$zzzzasdf1    | asdf$$\{Z}asdf1   |
| asd\\${asdf}asd | asd\\\\\\${asdf}asd  |
| asd\\${zzzz}asd | asd\\\\\\${Z}asd  |
| asd\\$zzzzasd     |  asd$${Z}asd   |
| asd\\zzzzasd      | asd\\\\${Z}asd  |
| asd\\\\zzzzasd   | asd\\\\\\\\${Z}asd |
| \\\\server\\path | \\\\server\\path |

**Note:** Unfortunately for Json strings, you'll need twice the number of backslashes!  
I will consider choosing another delimiter for the next version.
