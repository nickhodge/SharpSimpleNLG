# SharpSimpleNLG
SharpSimpleNLG is a C# port of the [University of Aberdeen *simplenlg* project](https://github.com/simplenlg/simplenlg)


## NuGet
[SharpSimpleNLG](https://nuget.org/packages/SharpSimpleNLG)

```
Install-Package SharpSimpleNLG
``` 

## Version News
Version 1.0.0 First commit/NuGet:
* Taken the current release of SimpleNLG (V4.4.8 API) version and generated a C# version. 
* Unit tests ported, with added tests for C# specifics (Java idioms in Regex, Collections, equals, strings.substring etc)
* Current release English, with no HTML generation nor NIHLexicon import

## How to Use SharpSimpleNLG?
I have to write documentation. One day. 

The [simplenlg Tutorial is a good resource.](https://github.com/simplenlg/simplenlg/wiki/Section-0-%E2%80%93-SimpleNLG-Tutorial)

Another approach to learning is to look at the Unit Tests and see what simplenlg can do: [SharpSimpleNLG Unit Tests](https://github.com/nickhodge/SharpSimpleNLG/blob/master/SharpSimpleNLGTests/)

## Building SharpSimpleNLG
* Visual Studio 2015 with C# 6.0 recommended (as this is what I used)
* I've only used [C# null conditionals](https://msdn.microsoft.com/en-au/library/dn986595.aspx) and [string interpolation](https://msdn.microsoft.com/en-us/library/dn961160.aspx) in a couple of places so C# 6.0 isn't a forced requirement
* Assembly created is .NET 4.5.2, but there is only use of generic ```HashSet<T>```, ```Stack<T>```, ```List<T>``` and ```Dictionary<K,V>``` with no async/await - so it should port to earlier .NET if you are into that sort of thing.

## Other .NET versions
* As at 20th June I added a .NET Core version. If you use this and need help - reach out. 
* Next up to test: Mono

## Unit Tests
* >270 Unit tests using NUnit are here.
* I've Unit tested my additional things
* Passing Unit tests from the original project have been ported
* 8 tests do not pass; I think this is related to the configuration of the Unit tests rather than a code issue.

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

Questions? You can find me on Twitter [@RealNickHodge](https://twitter.com/RealNickHodge) or email [Nick Hodge](mailto:nhodge@mungr.com)
