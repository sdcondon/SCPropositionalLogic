# LinqToKB

Knowledge base & inference libraries that use LINQ expressions for both knowledge representation and queries.
This was created just for fun while reading _Artificial Intelligence: A Modern Approach_ (3rd Edition - ISBN 978-1292153964) - so may prove interesting to the .NET-inclined reading the same book. 
For real-world scenarios, there are other better inference engines out there - that generally use more powerful logics than propositional logic, which is all that's implemented thus far (and maybe ever, but we'll see..).

## LinqToKB.Propositional

A very simple [propositional logic](https://en.wikipedia.org/wiki/Propositional_calculus) knowledge base and inference engine that uses LINQ expressions for knowledge representation and queries.
Here's a quick usage example:

```csharp
using LinqToKB
using static LinqToKB.Propositional.PLExpression<MyModel>; // For Implies

..

// For the mo, can only work with gettable & settable public bool-valued properties
// on classes with a parameterless public constructor.
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
