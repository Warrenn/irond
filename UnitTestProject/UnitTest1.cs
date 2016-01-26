using System;
using GrammarLibrary;
using GrammarLibrary.Gammars;
using irond;
using Irony.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mygrammar = new StartPageGrammar();

            //mygrammar.
            var parser = new Parser(mygrammar);
            var visitor = new Visitor();
            var testStatements =
                "Hide a when Quote.QuoteType.super is not 'new Business' \r\n" +
                "Hide Question2 when Quote.Salary <= 1000 and Quote.QuoteType is SuperFund\r\n" +
                "Disable WhichProduct.SuperFundCustom when Quote.QuoteType is 'Requote'\r\n" +
                "Set WhichProduct.SuperFundCustom to WhichProduct.SuperFundCustom when Quote.QuoteType is 'Requote'\r\n" +
                "Set SuperFundCustom to SuperFundCustom when Quote.QuoteType is 'Requote'\r\n";

            var tree = parser.Parse(testStatements);
            var t = visitor.Visit(tree.Root);

            //var compiler = new Irony.Interpreter.Evaluator.
            Assert.IsTrue(string.IsNullOrEmpty(t));
        }
    }
}
