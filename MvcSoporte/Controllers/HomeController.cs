using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcSoporte.Data;
using MvcSoporte.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSoporte.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MvcSoporteContexto _context;

        public HomeController(ILogger<HomeController> logger, MvcSoporteContexto context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Busca empleado correspondiente al usuario actual. Si existe
            // va a View y en caso contrario, va a crear el empleado.
            string emailUsuario = User.Identity.Name;
            Empleado empleado = _context.Empleados.Where(e => e.Email == emailUsuario)
            .FirstOrDefault();
            if (User.Identity.IsAuthenticated &&
            User.IsInRole("Usuario") &&
            empleado == null)
            {
                return RedirectToAction("Create", "MisDatos");
            }
            return View();
        }
        //[Authorize(Roles = "Usuario")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
