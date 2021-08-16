using AutoMapper;
using CarSale.Data;
using CarSale.Models;
using CarSale.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICarsRepo _cars;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ICarsRepo cars, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _cars = cars;
            _mapper = mapper;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var res = _cars.GetAllCars().Take(9);

            var output = _mapper.Map<List<CarReadDTO>>(res);

            return View(output);
        }

        public async Task<IActionResult> CarInfo(string id)
        {
            var car = _cars.GetSingleCar(id);

            if (car == null)
            {
                throw new Exception("No car with given id");
            }

            var output = _mapper.Map<CarReadDTO>(car);

            if (User.Identity.IsAuthenticated)
            {
                ViewData["UserEmail"] = (await _userManager.FindByIdAsync(output.UserId)).Email;
            }
            return View(output);
        }

        public IActionResult CarsByMake(string make)
        {
            var cars = _cars.GetCarsByMake(make);

            if (cars == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var output = _mapper.Map<List<CarReadDTO>>(cars);

            return View("Index", output);
        }

        public IActionResult CarsByModel(string model)
        {
            var cars = _cars.GetAllCars().Where(c => c.Model == model);

            if (cars == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var output = _mapper.Map<List<CarReadDTO>>(cars);

            return View("Index", output);
        }

        public async Task<IActionResult> CarsByUser(string email)
        {
            var userId = (await _userManager.FindByNameAsync(email)).Id;

            if (userId == null)
            {
                throw new Exception("User cannot be found!");
            }

            var cars = _cars.GetCarsOfUser(userId);

            var output = _mapper.Map<List<CarReadDTO>>(cars);

            return View("Index", output);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
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
