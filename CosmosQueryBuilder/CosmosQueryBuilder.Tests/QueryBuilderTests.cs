using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

namespace CosmosQueryBuilder.Tests
{
    public class Tests
    {
        public static IEnumerable TestCases()
        {
            var testCases = new List<(string alias, Expression<Predicate<TestClass>> expression, string expected)>
            {
                ("c",(TestClass x) => x.StringProp1 == "test", "c.StringProp1 = 'test'"),
                ("x", (TestClass x) => x.StringProp1 == "test", "x.StringProp1 = 'test'"),
                ("c", (TestClass x) => x.IntProp == 1, "c.IntProp = 1"),
                ("c", (TestClass x) => x.LongProp == 1L, "c.LongProp = 1"),
                ("c", (TestClass x) => x.FloatProp == 1f, "c.FloatProp = 1"),
                ("c", (TestClass x) => x.FloatProp == 1.1f, "c.FloatProp = 1.1"),
                ("c", (TestClass x) => x.DoubleProp == 1d, "c.DoubleProp = 1"),
                ("c", (TestClass x) => x.DecimalProp == 1m, "c.DecimalProp = 1"),
                ("c", (TestClass x) => x.BoolProp, "c.BoolProp"),
                ("c", (TestClass x) => !x.BoolProp, "!(c.BoolProp)"),
                ("c", (TestClass x) => x.BoolProp == true, "c.BoolProp = true"),
                ("c", (TestClass x) => x.BoolProp == false, "c.BoolProp = false"),
                ("c", (TestClass x) => x.StringProp1 != "test", "c.StringProp1 != 'test'"),
                ("c", (TestClass x) => x.IntProp < 1, "c.IntProp < 1"),
                ("c", (TestClass x) => x.IntProp <= 1, "c.IntProp <= 1"),
                ("c", (TestClass x) => x.IntProp > 1, "c.IntProp > 1"),
                ("c", (TestClass x) => x.IntProp >= 1, "c.IntProp >= 1"),
                ("c", (TestClass x) => x.StringProp1 == "a" && x.StringProp2 == "b", "c.StringProp1 = 'a' AND c.StringProp2 = 'b'"),
                ("c", (TestClass x) => x.StringProp1 == "a" || x.StringProp2 == "b", "(c.StringProp1 = 'a') OR (c.StringProp2 = 'b')"),
                ("c", (TestClass x) => x.StringProp1 == "a" || (x.StringProp2 == "b" && x.StringProp1 == "c"), "(c.StringProp1 = 'a') OR (c.StringProp2 = 'b' AND c.StringProp1 = 'c')"),
                ("c", (TestClass x) => x.StringProp1 == "a" || x.StringProp2 == "b" && x.StringProp1 == "c", "(c.StringProp1 = 'a') OR (c.StringProp2 = 'b' AND c.StringProp1 = 'c')"),
            };

            foreach (var testCase in testCases)
            {
                yield return new TestCaseData(testCase.alias, testCase.expression, testCase.expected)
                    .SetName($"Where({testCase.expression.Body.ToString().Replace('(', '[').Replace(')', ']')})");
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void Where(string alias, Expression<Predicate<TestClass>> exp, string expected)
        {
            QueryBuilder<TestClass>
                .Where(alias, exp)
                .Should().Be(expected);
        }
    }

    public class TestClass
    {
        public string StringProp1 { get; set; }
        public string StringProp2 { get; set; }
        public int IntProp { get; set; }
        public long LongProp { get; set; }
        public double DoubleProp { get; set; }
        public float FloatProp { get; set; }
        public decimal DecimalProp { get; set; }
        public bool BoolProp { get; set; }
    }
}