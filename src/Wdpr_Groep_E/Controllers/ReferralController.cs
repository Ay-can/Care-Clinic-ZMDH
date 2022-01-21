using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Wdpr_Groep_E.Services;

namespace Wdpr_Groep_E.Controllers
{
    public class ReferralController : Controller
    {
        private readonly IZorgdomein _zorgdomein;

        public ReferralController(IZorgdomein zorgdomein) => _zorgdomein = zorgdomein;

        public IActionResult Index() => View(_zorgdomein.GetAllReferrals().Result);

        [HttpPost]
        public IActionResult IndividualReferral(string Bsn, string birthDate)
        {
            CultureInfo info = new CultureInfo("en-US");
            DateTime date = Convert.ToDateTime(birthDate, info);
            string newDate = date.ToString("dd MM yyyy");
            return View(_zorgdomein.GetReferralObject(newDate, Bsn).Result);
        }

        [HttpGet]
        public IActionResult BackToIndex() => RedirectToAction("Index");
    }
}
