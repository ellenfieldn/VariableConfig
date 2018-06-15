# VariableConfig
VariableConfig allows you to reference other configuration properties and use them as variables.

## Usage
VariableConfig wraps any previously added configuration. This means that in most normal circumstances, it should be added last.
```C#
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddXmlFile("myConfig.xml")
    .AddVariableConfiguration();
Configuration = builder.Build();
```

Alternatively, the underlying IConfiguration can be added manually by passing an IConfiguration object to AddVariableConfiguration():
```C#
var builder = new ConfigurationBuilder()
    .AddVariableConfiguration(sourceConfiguration);
Configuration = builder.Build();
```

## Configuration
- Anything within `${ }` will be treated as a variable.
- Variables are resolved from configuration as if they were a normal configuration setting.
- A value can be composed of multiple variables.
- Variables can refer to other variables.
- Variables can reference properties in subsections.

.NetCore Json
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
.NetCore Xml
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
