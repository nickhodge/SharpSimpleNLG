# SharpSimpleNLG
SharpSimpleNLG is a C# port of the [University of Aberdeen *simplenlg* project](https://github.com/simplenlg/simplenlg)ttps://nuget.org/packages/SharpSimpleNLG)

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
* Assembly created is .NET 4.5.2, but there is only use of generic HashSet<T>, Stack<T>, List<T> and Dictionary<K,V> with no async/await - so it should port to earlier .NET if you are into that sort of thing.


Questions? You can find me on Twitter [@RealNickHodge](https://twitter.com/RealNickHodge) or email [Nick Hodge](mailto:nhodge@mungr.com)
