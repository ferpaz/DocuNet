# Third-party notices and licenses

DocuNet is an independent project by Fernando Paz, distributed under [MIT](license.md). That license covers its original contributions and does not replace the terms of the following components.

## Docu: inspiration and templates

[Docu](https://github.com/jagregory/docu), created by James Gregory, inspired this project. The supplied templates and navigation/example scripts are based on its resources and have been adapted for DocuNet.

Copyright (c) 2009, James Gregory. BSD-3-Clause license: the [full text](docs/licenses/docu-BSD-3-Clause.txt) is preserved, including copyright, conditions and disclaimer. No endorsement by the original author is implied.

## Generated site resources

| Resource | Original authorship | Preserved license |
| --- | --- | --- |
| jQuery 1.3.2 | Copyright (c) 2009 John Resig | MIT, selected from the alternatives in the original header |
| Sizzle 0.9.3, included in jQuery | Copyright 2009, The Dojo Foundation | MIT, selected from the original alternatives |
| jQuery.ScrollTo 1.4.2 | Copyright (c) 2007–2009 Ariel Flesler | MIT, selected from the original alternatives |
| HTML5 Boilerplate CSS foundation | Copyright (c) HTML5 Boilerplate | MIT |

Texts: [jQuery/Sizzle/ScrollTo](docs/licenses/jQuery-Sizzle-ScrollTo-MIT.txt) and [HTML5 Boilerplate](docs/licenses/HTML5-Boilerplate-MIT.txt). Original JavaScript headers remain intact.

Each site generated with the supplied templates includes THIRD-PARTY-NOTICES.txt and a footer link. Preserve that file and the notices when distributing the site or its templates.

Sources: [jQuery](https://jquery.com/license/), [ScrollTo](https://github.com/flesler/jquery.scrollTo), [HTML5 Boilerplate](https://github.com/h5bp/html5-boilerplate).

## CLI production dependencies

The versions below match the packages locked in packages.lock.json files. Build/test packages have their own terms; a NuGet reference does not make them DocuNet's own code.

| Package or group | Version | Included license and notices |
| --- | --- | --- |
| Spark | 1.8.2.4-alpha | Copyright (c) Louis DeJardin 2008–2024; [Apache-2.0](docs/licenses/Spark-Apache-2.0.txt), according to the package text |
| MarkdownSharp | 2.0.5 | Copyright (c) 2018 Stack Exchange; [MIT](docs/licenses/MarkdownSharp-MIT.txt) |
| Microsoft.CodeAnalysis.Common, CSharp and VisualBasic | 4.14.0 | .NET Foundation and Contributors; [MIT](docs/licenses/Roslyn-MIT.txt) and [additional notices](docs/licenses/Roslyn-ThirdPartyNotices.rtf) |
| Microsoft.CodeAnalysis.Analyzers, build only | 3.11.0 | MIT, according to NuGet metadata; not distributed as a CLI assembly |
| Microsoft.Extensions.Caching.Abstractions/Memory, DependencyInjection.Abstractions, Logging.Abstractions, Options and Primitives | 9.0.9 | .NET Foundation and Contributors; [MIT](docs/licenses/NET-Foundation-MIT.txt) and [additional notices](docs/licenses/NET-ThirdPartyNotices.txt) |
| System.Configuration.ConfigurationManager, Diagnostics.EventLog and Security.Cryptography.ProtectedData | 9.0.9 | The same [MIT](docs/licenses/NET-Foundation-MIT.txt) and [.NET notices](docs/licenses/NET-ThirdPartyNotices.txt) |
| Microsoft.NETFramework.ReferenceAssemblies.net48 | 1.0.3 | Copyright (c) 2019 Microsoft; [MIT](docs/licenses/NET-Framework-ReferenceAssemblies-MIT.txt), specified by the package's licenseUrl |

net48 reference files are copied as metadata; they are not executed and are not the .NET Framework runtime.

Spark texts and additional .NET/Roslyn notices are preserved from the exact packages, without summarizing their terms. License sources: [Spark](https://github.com/SparkViewEngine/spark/blob/a33890e3edb8b9a5517135f42ab296375d39fcaa/license.txt), [MarkdownSharp](https://github.com/StackExchange/MarkdownSharp/blob/master/LICENSE), [Roslyn](https://github.com/dotnet/roslyn/blob/main/License.txt), [Microsoft references](https://github.com/Microsoft/dotnet/blob/main/LICENSE).

## Distribution and maintenance

When preparing a CLI distribution, include license.md, this document and docs/licenses alongside the executable. The generated site automatically receives its own resource notices, without CLI dependencies that are not part of the HTML.

If you update packages or replace resources, review their licenses, copyright holders and notices. Do not remove a notice just because the visual footer attribution changes. Legacy libraries will be modernized according to the roadmap; their inclusion here does not establish that they are up to date or free of vulnerabilities.