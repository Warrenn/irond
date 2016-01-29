using GrammarLibrary;
using Irony.Parsing;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Runtime;
using WebApp.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    public class Home : Controller
    {
        private readonly IApplicationEnvironment environment;

        public Home(IApplicationEnvironment environment)
        {
            this.environment = environment;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var model = new QAModel();
            var mygrammar = new StartPageGrammar();

            var parser = new Parser(mygrammar);
            var visitor = new AngularJsVisitor();

            var rules = System.IO.File.ReadAllText(environment.ApplicationBasePath + "\\QARules.txt");
            var tree = parser.Parse(rules);

            model.Rules = visitor.Visit(tree.Root);
            model.Questions = System.IO.File.ReadAllText(environment.ApplicationBasePath + "\\questions.json");


            return View(model);
        }
    }
}
