using GrammarLibrary;
using Irony.Parsing;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    public class Home : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {

            var mygrammar = new StartPageGrammar();

            var parser = new Parser(mygrammar);
            var visitor = new AngularJsVisitor();
            var testStatements =
                "Hide DoubleQuote Question when quote.product is 'superFundEasy' \r\n" +
                "Disable Product Question SuperFundCustom Answer when quote.riskBeingTransfered is 'yes'\r\n" +
                "Set questions.product.text to 'A new Value entirely' when (quote.riskBeingTransfered is 'yes') and (quote.product is 'superFundCustom')";

            var tree = parser.Parse(testStatements);
            var t = visitor.Visit(tree.Root);


            return View(model: t);
        }
    }
}
