# ExternalTemplates

Finds template files in a pre-configured directory and merges them into a list of script tags that can be added to a layout file. This way you can use and share the external template in all places without paying for an extra request per template file.

# Installation

### Mvc5

```
Install-Package ExternalTemplates.Mvc
```

You'll have to manually register a couple of services in your dependency resolver (autofac, ninject, ...):
- `IApplicationBasePathProvider` to `ApplicationBasePathProvider`
- `IGeneratorOptions` to `GeneratorOptions`
- `IFilesProvider` to `FilesProvider`
- `ICoreGenerator` to `CoreGenerator`

Register them all as singletons.

Next, just add the following anywhere in your layout file:
```
@ExternalTemplates.Generator.Generate()
```

###### Configuration
Register `IGeneratorOptions` to whatever you like. You can use `GeneratorOptions` and set its values.

An example with autofac:
```c#
builder.RegisterInstance<GeneratorOptions>(new GeneratorOptions
				{
					VirtualPath = "Views/templates", // Look for templates in Views/templates. Default is "Content/templates"
					Extension = ".template.html", // Look for files with extension ".template.html". Default is ".tmpl.html"
					PostString = "-template" // Append "-template" to the script tag's id. Default is "-tmpl"
				})
				.As<IGeneratorOptions>()
				.SingleInstance();
```

### Asp.Net

```
Install-Package ExternalTemplates.AspNet
```

Next, register the services:
`services.AddExternalTemplates()`

In your layout file:
```
@inject ExternalTemplates.IGenerator Generator
@Generator.Generate()
```

###### Configuration

```c#
services.AddExternalTemplates().ConfigureExternalTemplates(options => {
    options.VirtualPath = "Views/templates";
    options.PostString = "-template";
});
```

# Usage

The default path for templates is `Content/templates` with an extension `.tmpl.html`. So if you add a file `article.tmpl.html` you can be sure that there will be a script tag with an id of `article-tmpl` available to be used with your templating engine of choice (knockout, moustash, ...).

# License
MIT
