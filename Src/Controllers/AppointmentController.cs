using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ILogger<AppointmentController> _logger;
        private readonly IFluentEmail _email;
        private readonly AppContext _context;

        public AppointmentController(ILogger<AppointmentController> logger, IFluentEmail email, AppContext context)
        {
            _logger = logger;
            _email = email;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateAppointment(string firstname, string lastname, string infix, string email, string phone , string subject, string message)
        {
            _context.Add(new AppointmentModel () {FirstName = firstname, LastName = lastname, Infix = infix, PhoneNumber = phone, Subject = subject , Message = message , Email = email});
            _context.SaveChanges();
            return RedirectToAction("Index","Home");
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        
    }
}
