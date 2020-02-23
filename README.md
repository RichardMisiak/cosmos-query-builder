# cosmos-query-builder
Playing around with converting LINQ to Cosmos DB SQL syntax

Example 
``` c#
  QueryBuilder<TestClass>
      .Where("c", x => x.StringProp1 == "a" || (x.StringProp2 == "b" && x.StringProp1 == "c"));
  // (c.StringProp1 = 'a') OR (c.StringProp2 = 'b' AND c.StringProp1 = 'c')
```
