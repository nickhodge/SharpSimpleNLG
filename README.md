[![Build status](https://ci.appveyor.com/api/projects/status/bjwkwoqdlpcl1jir?svg=true)](https://ci.appveyor.com/project/Arash-Sabet/sharpsimplenlg-olve3)
# SharpSimpleNLG
SharpSimpleNLG is a C# port of the [University of Aberdeen *simplenlg* project](https://github.com/simplenlg/simplenlg). All thanks and acknowledgement for hard work goes to this team.

> Please note that this port is behind the most current original *simplenlg* project.

## NuGet
[SharpSimpleNLG](https://nuget.org/packages/SharpSimpleNLG)

```
Install-Package SharpSimpleNLG
``` 

## Version News
Version 1.3.0:
* Thanks to [PR from Michael Paulus](https://github.com/nickhodge/SharpSimpleNLG/pull/4) Updated extensions to be internal so consuming projects don't pick them up. 


Version 1.2.0:
* Thanks to [PR from Arash-Sabet](https://github.com/nickhodge/SharpSimpleNLG/pull/3) now supports .NET Standard 1.6 and .NET 4.6.2; along with VS2017 support. Added to NuGet package (I still hope my packaging is OK!)

Version 1.1.1:
* Thanks to [PR from Andriy Svyrd](https://github.com/nickhodge/SharpSimpleNLG/pull/2) now supports .NET Standard 1.3. Added to NuGet package (I hope my packaging is OK!)
* Renaming .csproj to ensure naming of assemblies is OK
* Move all extension methods (on Enums, mainly) into the SimpleNLG.Extensions namespace to stop pollution (thanks for recommendation Andriy)
* Fix pathing to Lexicon
* NuGet packaging fixes

Version 1.0.2:
* Permit passing of path to ```default-lexicon.xml``` into the ```XMLLexicon``` constructor
* thereby permitting a rough-draft (and untested) .NET Core port

Version 1.0.0 First commit/NuGet:
* Taken the current release of SimpleNLG (V4.4.8 API) version and generated a C# version. 
* Unit tests ported, with added tests for C# specifics (Java idioms in Regex, Collections, equals, strings.substring etc)
* Current release English, with no HTML generation nor NIHLexicon import

## How to Use SharpSimpleNLG?
I have to write some good documentation.

Look at the code in the [Syntax Tutorials Unit Test](https://github.com/nickhodge/SharpSimpleNLG/blob/master/SharpSimpleNLGTests/syntax/TutorialTest.cs). All these pass, so it is a good starter.

The [simplenlg Tutorial is a good resource.](https://github.com/simplenlg/simplenlg/wiki/Section-0-%E2%80%93-SimpleNLG-Tutorial)

Or look at the general Unit Tests and see what simplenlg can do: [SharpSimpleNLG Unit Tests](https://github.com/nickhodge/SharpSimpleNLG/blob/master/SharpSimpleNLGTests/)


## Building SharpSimpleNLG
* Visual Studio 2022 with C# 6.0+ idioms used (as this is what I used)
* I've only used [C# null conditionals](https://msdn.microsoft.com/en-au/library/dn986595.aspx) and [string interpolation](https://msdn.microsoft.com/en-us/library/dn961160.aspx) in a couple of places so C# 6.0 isn't a forced requirement
* Assembly created has a minimum of .NET Standard 2.0 / .NET 4.6, but there is only use of generic ```HashSet<T>```, ```Stack<T>```, ```List<T>``` and ```Dictionary<K,V>``` with no async/await - so it should port to earlier .NET if you are into that sort of thing.

## Unit Tests
* >270 Unit tests using NUnit included.
* Passing Unit tests from the original project have been ported
* Added Unit tests for this particular port
* 7 tests do not pass; I think this is related to the configuration of the Unit tests rather than a code issue.

## Learnings

I am sure someone somewhere at sometime (most likely me) will find my [Learnings on Java to C#](https://github.com/nickhodge/SharpSimpleNLG/blob/master/JavaToCsharpLearnings.md) interesting and useful.

## Outstanding and Future Work
* contribute _back_ to parent!
* implement NIHLexicon and MultipleLexicon 
* implement HTMLFormatter
* re-work Unit test hierarchy
* continue to track [*simplenlg*](https://github.com/simplenlg/simplenlg) changes and additions
* fluent-style sentence and phrase creation
* More C#/functional style
* Enums.ToString() change to Enum "side" hierarchy (because of C#/Java differences)
* smarter types (eg: widely used ```Dictionary<string, object>``` to subclassed to make life easier)
* Documentation specific to this port
