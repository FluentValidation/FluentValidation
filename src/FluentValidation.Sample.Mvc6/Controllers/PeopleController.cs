using Microsoft.AspNet.Mvc;
using FluentValidation.Mvc;
using FluentValidation.Sample.Mvc6.Models;

namespace FluentValidation.Sample.Mvc6.Controllers
{
    /// <summary>
    /// Summary description for PersonController
    /// </summary>
    public class PeopleController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Person person)
        {
            if (!ModelState.IsValid)
            { // re-render the view when validation failed.
                return View("Create", person);
            }

            return RedirectToAction("Index");

        }
    }
}