using AutoMapper;
using CarSale.Data;
using CarSale.Models;
using CarSale.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CarSale.Controllers
{
    [Authorize(Policy = "UploadPolicy")]
    public class UploadController : Controller
    {
        private readonly ILogger<UploadController> _logger;
        private readonly ICarsRepo _cars;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UploadController(ILogger<UploadController> logger, ICarsRepo cars, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _cars = cars;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CarUpsertDTO md)
        {
            if (ModelState.IsValid)
            {
                var mdDB = _mapper.Map<CarModel>(md);
                mdDB.UserId = (await _userManager.FindByEmailAsync(User.Identity.Name)).Id;
                mdDB.Date = DateTime.Now;
                _cars.AddCar(mdDB);

                _cars.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Cannot add new car announcement");
            }

            return View(md);
        }

        [HttpGet]
        [Authorize(Policy = "EditPolicy")]
        public IActionResult Edit(string editId)
        {
            var car = _cars.GetSingleCar(editId);
            if (car == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var output = _mapper.Map<CarUpsertDTO>(car);

            return View(output);
        }

        [HttpPost]
        [Authorize(Policy = "EditPolicy")]
        public IActionResult Edit(string editId, CarUpsertDTO md)
        {
            if (ModelState.IsValid)
            {
                var car = _cars.GetSingleCar(editId);
                if (car == null)
                {
                    return Redirect("Error");
                }

                var edited = _mapper.Map(md, car);
                _cars.UpdateCar(edited);

                _cars.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(md);
        }

        [HttpGet]
        public IActionResult Delete(string deleteId)
        {
            var md = new DeleteCarModel { CarId = deleteId, ConfirmCarId = "" };

            return View(md);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(DeleteCarModel model)
        {
            if (ModelState.IsValid)
            {
                _cars.DeleteCar(model.CarId);
                _cars.SaveChanges();

                return RedirectToAction("CarsByUser", "Home", new { email = User.Identity.Name });
            }

            return View(model);
        }
    }
}
