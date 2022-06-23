# SCPropositionalLogic

Very simple [propositional logic](https://en.wikipedia.org/wiki/Propositional_calculus) knowledge base implementations.

Created just for fun while reading _Artificial Intelligence: A Modern Approach_ (3rd Edition - [ISBN 978-1292153964](https://www.google.com/search?q=isbn+978-1292153964)) - so may prove interesting to the .NET-inclined reading the same book.
The main goal here is for it to be a learning resource - as such, care has been taken to include decent XML documentation and explanatory inline comments where appropriate.
For real-world scenarios, there are other better inference engines out there that use more powerful logics than propositional logic, which is all that's implemented here (and - thus far at least - only in the most basic of ways).

In truth, the main purpose this repo serves is as a stepping stone to [SCFirstOrderLogic](https://github.com/sdcondon/SCFirstOrderLogic). 

For usage examples, see the tests.

## Language Integration

No doubt there are countless propositional logic libraries out there for .NET. The only perhaps non-obvious part of this one is the classes in the LanguageIntegration namespace, which allow for specifying PL sentences as LINQ expressions.

Benefits of using LINQ expressions:
* Your sentences of propositional logic can be expressed as familiar, plain-old C#, using the operators you would probably expect - `&&`, `||` and `!` (NB not `&` and `|` yet..).
* Further, your rules are expressed in code that can be executed directly against a model instance - useful when reasoning about rules, and for verification purposes.

Drawbacks of using LINQ expressions:
* LINQ expressions are perhaps larger and slower to work with than custom-built expression classes could be. So this may not be the best choice where performance requirements are particularly stringent.
* By using C#, there is a danger in confusing C# operators with the elements of propositional logic that they are mapped to - creating a risk to learning outcomes.
That is, while it may be intuitive to map the C# `||` operator to a disjunction in PL, they do of course represent distinct things.
Compared to uses of LINQ such as LINQ to SQL (where the system being mapped to is very obviously distinct), it is perhaps less obvious that there IS still a different system (propositional logic) being mapped to here. This is important to bear in mind while working with this library.
I do appreciate that it would be useful to come up with a more rigorous definition of the mapping between the C# and PL worlds (which may include some renaming of things..), here - doing so would mitigate this risk somewhat.
