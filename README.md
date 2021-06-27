# LinqToKnowledgeBase

Knowledge base & inference libraries that use LINQ expressions for both knowledge representation and queries.

This was created just for fun while reading _Artificial Intelligence: A Modern Approach_ (3rd Edition - ISBN 978-1292153964) - so may prove interesting to the .NET-inclined reading the same book.

The main goal here is for it to be a learning resource - as such, care has been taken to include decent XML documentation and explanatory inline comments where helpful.
For real-world scenarios, there are other better inference engines out there - that generally use more powerful logics than propositional logic, which is all that's implemented thus far (will probably have a crack at first-order logic too, but will likely stop there).

Benefits of using LINQ expressions:
* Your rules can be expressed in the familiar, plain-old C#
* Further, your rules are expressed in code that can be executed directly against the domain model (which is probably useful.. somehow)
* LINQ already includes much of the plumbing to make this happen - expression trees, visitor classes etc - meaning that there isn't actually a huge amount that the library needs to add.

## LinqToKnowledgeBase.PropositionalLogic

A very simple [propositional logic](https://en.wikipedia.org/wiki/Propositional_calculus) knowledge base and inference engine that uses LINQ expressions for knowledge representation and queries.
Here's a quick usage example:

```csharp
using LinqToKB
using LinqToKB.KnowledgeBases
using static LinqToKB.Propositional.PLExpression<MyModel>; // For Implies

..

// TruthTableKnowledgeBase enumerates the truth table to answer queries
// (and is thus very slow for non-trivial knowledge bases).
// For the moment, it can only work with gettable & settable public bool-valued
// properties on classes with a parameterless public constructor. Would
// nice to get it working with interfaces and read-only props.
class MyModel
{
    public bool ItIsSaturday { get; set; }
    public bool ItIsTheWeekend { get; set; }
}

var kb = new TruthTableKnowledgeBase<MyModel>();
// NB kb.Tell(m => !m.ItIsSaturday || m.ItIsTheWeekend) would work just as well as the below..
kb.Tell(Implies(m => m.ItIsSaturday, m => m.ItIsTheWeekend));
kb.Tell(m => m.ItIsSaturday);
var itIsTheWeekend = kb.Ask(m => m.ItIsTheWeekend); // evaluates to true

// ResolutionKnowledgeBase uses resolution (essentially proof-by-contradiction).
// It works fine with interfaces and read-only props (in fact doesn't actually
// have use prop-valued literals at all - though non-prop literals are untested as yet)
interface IMyModel
{
    bool ItIsSaturday { get; }
    bool ItIsTheWeekend { get; }
}

var kb = new ResolutionKnowledgeBase<MyModel>();
kb.Tell(Implies(m => m.ItIsSaturday, m => m.ItIsTheWeekend));
kb.Tell(m => m.ItIsSaturday);
var itIsTheWeekend = kb.Ask(m => m.ItIsTheWeekend); // evaluates to true
```
