# Stuff encountered on the simplenlg journey from Java to Csharp

A quick [search on StackOverflow](http://stackoverflow.com/questions/443010/where-can-i-find-a-java-to-c-sharp-converter?lq=1) on conversion of Java to C# says "use these tools but it is best to hand-convert"

Now with experience, I would agree with the "hand convert" approach: you learn so much more about how the underlying code works.

The simplenlg codebase is about 20KLOC; and I underestimated the time it would take, because I had not done this style of conversion. I had mentally estimated about 5 days upon my outset.

## Can't just rename from .java to .cs and recompile?
* like, der. don't know why you would expect this to just work. It doesn't
* both languages as being typed, C- curly braced languages with many common features made the work easier (than say, a Python project)
* process was made easier by flattening the namespace hierarchy into one
* although by flattening the hierarchy, this meant poisoning by Extension methods as soon as the base namespace was imported - fixed in subsequent releases
* good Tests meant that quality could be checked at the end

## Time Budget
* 3.5 days for 20KLOC across 100 files to "0 error build" (rename .java to .cs, fix all the textual stuff)
* 0.5 days for additional "glue" code (Extension methods) and Unit tests
* 0.5 days for build to actually pop out something sensible (fixing the big picture issues)
* 3.5 days of Unit tests (~270) working, finding and fixing lower-level idiomatic Java style issues eg: Regex
* Estimate of 2KLOC per day

This project had no external library dependencies (apart from JUnit) and used relatively well-known Java idioms and mechanisms. Taking something more complex and larger would probably blow this budget somewhat.

Thankfully, simplenlg had an internal "object dumper" (```realiser.setDebugMode(true)```) that generated a simple tree-like diagram of the object tree. This helped diagnose 25% of the bugs.

The text-editing and building portion of work was completed without building and instantiating the .java. Once Testing and debugging time came, it was easiest to fire up Eclipse and side by side debug line by line with Visual Studio. This as accomplished by ensuring all the .java passed the Unit tests; then tracing the same code through C#. Time consuming - but this helped diagnose 80% of the failing tests.

The bugs caught in this manner included the foreach looping over ```Stack<T>``` being different; the Regex differences; and the conundrum of .equals 

As a key part of this project is a 1Mb XML data file [default-lexicon.xml](https://github.com/nickhodge/SharpSimpleNLG/blob/master/SharpSimpleNLG/lexicon/default-lexicon.xml) -and- the initial use-case is a client-side application, using [IKVM](https://www.ikvm.net/) with its additional download weight was not an option.

## Enums are objects in Java 
* which means that enums can be subclassed
* when using as Keys in ```Dictionary<K,V>``` (expando-objects) all is well. 
* Cannot use different Enums in the same Dictionary as keys.
* had to put wrap Enums (given unique ranges of ```int``` values) into an object hierarchy; and use ```.ToString()``` on object to get a Key
* in a future version, would just use the ```int``` values as keys for faster processing
* be careful with placement of extension methods and their namespace. Don't clog the base namespace

## .equals
* mostly in [PhraseChecker.cs](https://github.com/nickhodge/SharpSimpleNLG/blob/master/SharpSimpleNLG/aggregation/PhraseChecker.cs) there are places ```INGLElement```, and their ```.getFeatures``` need to be checked to ensure they are equal.
* Rather than C# ```.Equals``` the object, which seems to generate a different result to Java (I have yet to research this in depth) I re-wrote the [.equals as extension methods](https://github.com/nickhodge/SharpSimpleNLG/blob/master/SharpSimpleNLG/helperextensions/EqualsExtensions.cs) in a handful of cases; which under Unit testing resulted in the correct equals result.

## Regex is slightly different
* .NET doesnt support [character class subtraction](http://www.rexegg.com/regex-class-operations.html#intersection_workaround) the same was as Java
* eg ```.*[b-z&&[^eiou]]y\b``` became ```.*[b-z-[eiou]]y\b```

## Extension methods in C# rock
* see [HelperExtensions](https://github.com/nickhodge/SharpSimpleNLG/blob/master/SharpSimpleNLG/helperextensions/HelperExtensions.cs)
* rather than changing lots of Java code to C#, just created Extension methods.
* for instance, Java ```list.get(0)``` maps to ```list[0]``` in C# without replacement
* if you do need to replace/fix in the future, you can look at the backreference from the Extension method to fix.

## But be careful with Semantics
* Java .put will create a new ```HashMap``` KV pair, or replace the Value if the key already exists
* In C#, you have to write around this - so the .put Extension method takes this into account
* Changing ```foreach``` over an Interable seems easy enough (and easier in C#!) but semantics of this over a ```Stack<T>``` is different in C# (first vs. last!) Solved by .ToList().Reverse()
* big difference in Java ```String.substring``` vs. C# ```string.SubString``` (length vs. position as second param)
* reading the Java docs, and ensuring the Extension methods follow the meanings became important for some deep bugs

## Object hierarchy calling convention
I must admit a little bit of defeat on this one. (this is for later research)

```SPPhraseSpec``` inherits from abstract class ```PhraseElement``` inherits from ```NGElement``` which implements ```INGElement```

Both ```SPPhraseSpec``` and ```NGElement``` have a ```.setFeature(object)``` method, but with different functions. In many places, a straight ```INGElement``` is passed around - and when you call ```.setFeature``` on an ```SPPhraseSpec``` cast as ```INGElement``` - the lower level class' method is called.

it seems this is not the case in Java?

This involved some checking and casting to work around this (and this fixed many a bug found in Testing)

## Resisting the Urge to *LINQ* and *Func<>* everything

I am glad I spent the time getting the base working, with Tests working - before attempting to optimise or start to use more modern C# style coding. 
