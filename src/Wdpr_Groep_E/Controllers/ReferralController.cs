using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wdpr_Groep_E.Models;
using Wdpr_Groep_E.Services;

namespace Wdpr_Groep_E.Controllers
{
    public class ReferralController : Controller
    {
        private readonly IZorgdomein _zorgdomein;
        public ReferralController(IZorgdomein zorgdomein)
         { 
           _zorgdomein = zorgdomein;
         }

        public IActionResult Index() 
        {
            return View(_zorgdomein.GetAllReferrals().Result);
        } 

        [HttpPost]
        public IActionResult IndividualReferral(string Bsn, string birthDate)
        {
            CultureInfo info = new CultureInfo("en-US");
            DateTime date = Convert.ToDateTime(birthDate,info);
            string newDate = date.ToString("dd MM yyyy");
            return View(_zorgdomein.GetReferralObject(newDate, Bsn).Result);
        }
        [HttpGet]
        public IActionResult BackToIndex()
        {
            return RedirectToAction("Index");
        }
        
        
    }
}
