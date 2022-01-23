using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wdpr_Groep_E.Models;
using Wdpr_Groep_E.Services;

namespace Wdpr_Groep_E.Controllers
{
    [Authorize(Roles = "Moderator, Orthopedagoog")]
    public class ReferralController : Controller
    {
        private readonly IZorgdomein _zorgdomein;

        public ReferralController(IZorgdomein zorgdomein) => _zorgdomein = zorgdomein;

        public IActionResult Index(string search, int page, int size, string sort)
        {
            var referrals = _zorgdomein.GetAllReferrals().Result.AsQueryable();

            if (sort == null) sort = "bsn_oplopend";
            ViewData["sort"] = sort;

            ViewData["search"] = search;

            if (page == 0) page = 1;
            ViewData["page"] = page;

            if (size == 0) size = 10;
            ViewData["size"] = size;

            ViewData["previous"] = page > 1;
            ViewData["next"] = (page * size) < referrals.Count();

            return View(Paginate(Search(Sort(referrals, sort), search), page, size).ToList());
        }

        public IQueryable<ReferralOverview> Search(IQueryable<ReferralOverview> referrals, string search)
        {
            if (search != null) referrals = referrals
                .Where(c => c.bsn
                    .Contains(search) || c.dt
                        .Contains(search));
            return referrals;
        }

        public IQueryable<ReferralOverview> Paginate(IQueryable<ReferralOverview> referrals, int page, int size)
        {
            return referrals.Skip((page - 1) * size).Take(size);
        }

        public IQueryable<ReferralOverview> Sort(IQueryable<ReferralOverview> referrals, string sort)
        {
            switch (sort)
            {
                case "bsn_oplopend":
                    return referrals.OrderBy(c => c.bsn);
                case "bsn_aflopend":
                    return referrals.OrderByDescending(c => c.bsn);
                case "geboortedatum_oplopend":
                    return referrals.OrderBy(c => c.dt);
                case "geboortedatum_aflopend":
                    return referrals.OrderByDescending(c => c.dt);
                default:
                    return referrals.OrderBy(c => c);
            }
        }

        [HttpPost]
        public IActionResult IndividualReferral(string Bsn, string birthDate)
        {
            CultureInfo info = new CultureInfo("en-US");
            DateTime date = Convert.ToDateTime(birthDate, info);
            string newDate = date.ToString("dd MM yyyy");
            return View(_zorgdomein.GetReferralObject(newDate, Bsn).Result);
        }
    }
}
