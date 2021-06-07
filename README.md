# LinqToKB

Knowledge base & inference libraries that use LINQ expressions for both knowledge representation and queries.
This was created just for fun while reading _Artificial Intelligence: A Modern Approach_ (3rd Edition - ISBN 978-1292153964) - so may prove interesting to the .NET-inclined reading the same book. 
For real-world scenarios, there are other better inference engines out there - that generally use more powerful logics than propositional logic, which is all that's implemented thus far (and maybe ever, but we'll see..).

Benefits of using LINQ expressions:
- Your logic can be expressed in the familiar, plain-old C#
- Your rules are expressed in code that can be executed directly against the domain model (which is probably useful.. somehow)
- LINQ already includes much of the plumbing to make this happen - expression trees, visitor classes etc - meaning that there isn't actually a huge amount that the library needs to add.

TODO: ..and any potential drawbacks?

Limitations:
- (TODO talk about atopping short of fully-fledged learning. Because the value of language-integrated knowledge is massively diminished when that knowledge isn't established by the code..)

## LinqToKB.Propositional

A very simple [propositional logic](https://en.wikipedia.org/wiki/Propositional_calculus) knowledge base and inference engine that uses LINQ expressions for knowledge representation and queries.
Here's a quick usage example:

```csharp
using LinqToKB
using static LinqToKB.Propositional.PLExpression<MyModel>; // For Implies

..

// For the mo, can only work with gettable & settable public bool-valued properties
// on classes with a parameterless public constructor. Want to target interfaces, 
// ultimately.
class MyModel
{
    public bool TodayIsSaturday { get; set; }
    public bool ItIsTheWeekend { get; set; }
}

..

var kb = new KnowledgeBase<MyModel>();
// NB kb.Tell(m => !m.TodayIsSaturday || m.ItIsTheWeekend) would work just as well as the below..
kb.Tell(Implies(m => m.TodayIsSaturday, m => m.ItIsTheWeekend));
kb.Tell(m => m.TodayIsSaturday);
var itIsTheWeekend = kb.Ask(m => m.ItIsTheWeekend); // evaluates to true
```
