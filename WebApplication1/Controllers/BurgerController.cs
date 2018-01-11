using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class BurgerController : Controller
    {
        // GET: Burger
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dictionary(int? iLength)
        {
            Dictionary<int, string> dBurgers = new Dictionary<int, string>();
            Random rnd = new Random();
            string sAnswer;

            for (int iCount = 0; iCount < iLength; iCount++)
            {
                if (iCount % 2 == 0)

                {
                    dBurgers.Add(rnd.Next(iCount, iCount*300), "Patrik");
                }
                else
                {
                    dBurgers.Add(rnd.Next(iCount*1000, iCount *5000), "Rachel");
                }
            }


            if (dBurgers.TryGetValue(2, out string sSearchValue))
            {
                sAnswer = sSearchValue;
            }
            // use myValue, still in scope, null if not found
            dBurgers.Remove(2);

            return View(dBurgers);
        }
    }
}