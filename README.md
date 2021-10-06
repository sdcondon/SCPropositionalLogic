**NB: Archived in favour of https://github.com/sdcondon/LinqToKB.FirstOrderLogic**
 
 # LinqToKB.PropositionalLogic

Very simple [propositional logic](https://en.wikipedia.org/wiki/Propositional_calculus) knowledge base implementations that use LINQ expressions for knowledge representation and queries.

Created just for fun while reading _Artificial Intelligence: A Modern Approach_ (3rd Edition - [ISBN 978-1292153964](https://www.google.com/search?q=isbn+978-1292153964)) - so may prove interesting to the .NET-inclined reading the same book.
The main goal here is for it to be a learning resource - as such, care has been taken to include decent XML documentation and explanatory inline comments where appropriate.
For real-world scenarios, there are other better inference engines out there that use more powerful logics than propositional logic, which is all that's implemented here (and - thus far at least - only in the most basic of ways).

Benefits of using LINQ expressions:
* Your sentences of propositional logic can be expressed as familiar, plain-old C#, using the operators you would probably expect - `&&`, `||` and `!` (NB not `&` and `|` yet..).
* Further, your rules are expressed in code that can be executed directly against your domain model - useful when reasoning about rules, and for verification purposes.
* LINQ already includes much of the plumbing to make this happen - expression trees, visitor classes etc - meaning that there isn't actually a huge amount that the library needs to add.

Drawbacks of using LINQ expressions:
* LINQ expressions are perhaps larger and slower to work with than custom-built expression classes could be. So this may not be the best choice where performance requirements are particularly stringent.
* By using C#, there is a danger in confusing C# operators with the elements of propositional logic that they are mapped to - creating a risk to learning outcomes.
That is, while it may be intuitive to map the C# `||` operator to a disjunction in PL, they do of course represent distinct things.
Compared to uses of LINQ such as LINQ to SQL (where the system being mapped to is very obviously distinct), it is perhaps less obvious that there IS still a different system (propositional logic) being mapped to here. This is important to bear in mind while working with this library.
I do appreciate that it would be useful to come up with a more rigorous definition of the mapping between the C# and PL worlds (which may include some renaming of things..), here - doing so would mitigate this risk somewhat.

## Usage

Here's a quick usage example:

```csharp
using LinqToKB.PropositionalLogic;
using LinqToKB.PropositionalLogic.KnowledgeBases;
using static LinqToKB.PropositionalLogic.PLExpression<MyModel>; // For the Implies static method

// ...

// TruthTableKnowledgeBase enumerates the truth table to answer queries
// (and is thus very slow for non-trivial knowledge bases).
// For the moment, it can only work with gettable & settable public bool-valued
// properties on classes with a parameterless public constructor. Would be
// nice to get it working with interfaces and read-only props. Shouldn't
// be too tough - just need a bit of dynamic type creation or translation
// of rules as they are added.
class MyModel
{
    public bool ItIsSaturday { get; set; }
    public bool ItIsTheWeekend { get; set; }
}

var kb = new TruthTableKnowledgeBase<MyModel>();
kb.Tell(Implies(m => m.ItIsSaturday, m => m.ItIsTheWeekend));
// NB: The above is exactly equivalent to (though hopefully easier to read than):
// kb.Tell(m => !m.ItIsSaturday || m.ItIsTheWeekend)
kb.Tell(m => m.ItIsSaturday);
var itIsTheWeekend = kb.Ask(m => m.ItIsTheWeekend); // evaluates to true

// ...

// ResolutionKnowledgeBase uses resolution (essentially proof-by-contradiction).
// It works fine with interfaces and read-only Boolean props or fields.
interface IMyModel
{
    bool ItIsSaturday { get; }
    bool ItIsTheWeekend { get; }
}

var kb = new ResolutionKnowledgeBase<IMyModel>();
kb.Tell(Implies(m => m.ItIsSaturday, m => m.ItIsTheWeekend));
kb.Tell(m => m.ItIsSaturday);
var itIsTheWeekend = kb.Ask(m => m.ItIsTheWeekend); // evaluates to true
```
