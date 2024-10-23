using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using DXCarsCrud.Data;
using DXCarsCrud.Models;
using DXCarsCrud.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace DXCarsCrud.Controllers
{
    public class CarsController : Controller
    {
        private AppDbContext _context;

        public CarsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Car car;
            if (id == 0)
            {
                car = await _context.Cars.AsNoTracking().FirstOrDefaultAsync();
            }
            else
            {
                car = await _context.Cars.AsNoTracking().Where(c => c.CarNo == id).FirstOrDefaultAsync();
            }

            var First = await _context.Cars.
               AsNoTracking().
               MinAsync(c => c.CarNo);

            var Previous = await _context.Cars
                    .AsNoTracking()
                    .Where(c => c.CarNo < id)
                    .OrderByDescending(c => c.CarNo)
                    .FirstOrDefaultAsync();

            var Next = await _context.Cars
                   .AsNoTracking()
                   .Where(c => c.CarNo > id)
                   .OrderBy(c => c.CarNo)
                   .FirstOrDefaultAsync();

            var Last = await _context.Cars.
               AsNoTracking().
               MaxAsync(c => c.CarNo);

            var CarDetailsViewModel = new CarDetailsViewModel()
            {
                Car = car,
                Paginition = new Paginition(First: First,
                                    Previous: (Previous?.CarNo ?? 0),
                                    Current: (int)id,
                                    Next: (Next?.CarNo ?? 0),
                                    Last: Last)
            };

            return View(CarDetailsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var cars = _context.Cars.Select(i => new {
                i.CarNo,
                i.UserNo,
                i.ArName,
                i.EnName,
                i.CardNo,
                i.BeginDate,
                i.EndDate,
                i.Company,
                i.ColorNo,
                i.Model
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "CarNo" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(cars, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Car car)
        {
            if (car == null)
            {
                return BadRequest();
            }
            var existsCar = await _context.Cars.AsNoTracking().Where(c => c.CarNo == car.CarNo).FirstOrDefaultAsync();

            if(existsCar == null)
            {
                return NotFound();
            }

            existsCar = car;

            _context.Cars.Update(car);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = car.CarNo });
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {
            var model = new Car();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Cars.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.CarNo });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values)
        {
            var model = await _context.Cars.FirstOrDefaultAsync(item => item.CarNo == key);
            if (model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(int key)
        {
            var model = await _context.Cars.FirstOrDefaultAsync(item => item.CarNo == key);

            _context.Cars.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> ColorsLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from i in _context.Colors
                         orderby i.ColorName
                         select new
                         {
                             Value = i.ColorNo,
                             Text = i.ColorName
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(Car model, IDictionary values)
        {
            string CAR_NO = nameof(Car.CarNo);
            string USER_NO = nameof(Car.UserNo);
            string AR_NAME = nameof(Car.ArName);
            string EN_NAME = nameof(Car.EnName);
            string CARD_NO = nameof(Car.CardNo);
            string BEGIN_DATE = nameof(Car.BeginDate);
            string END_DATE = nameof(Car.EndDate);
            string COMPANY = nameof(Car.Company);
            string COLOR_NO = nameof(Car.ColorNo);
            string MODEL = nameof(Car.Model);

            if (values.Contains(CAR_NO))
            {
                model.CarNo = Convert.ToInt32(values[CAR_NO]);
            }

            if (values.Contains(USER_NO))
            {
                model.UserNo = Convert.ToString(values[USER_NO]);
            }

            if (values.Contains(AR_NAME))
            {
                model.ArName = Convert.ToString(values[AR_NAME]);
            }

            if (values.Contains(EN_NAME))
            {
                model.EnName = Convert.ToString(values[EN_NAME]);
            }

            if (values.Contains(CARD_NO))
            {
                model.CardNo = Convert.ToString(values[CARD_NO]);
            }

            if (values.Contains(BEGIN_DATE))
            {
                model.BeginDate = Convert.ToDateTime(values[BEGIN_DATE]);
            }

            if (values.Contains(END_DATE))
            {
                model.EndDate = Convert.ToDateTime(values[END_DATE]);
            }

            if (values.Contains(COMPANY))
            {
                model.Company = Convert.ToString(values[COMPANY]);
            }

            if (values.Contains(COLOR_NO))
            {
                model.ColorNo = Convert.ToInt32(values[COLOR_NO]);
            }

            if (values.Contains(MODEL))
            {
                model.Model = Convert.ToString(values[MODEL]);
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState)
        {
            var messages = new List<string>();

            foreach (var entry in modelState)
            {
                foreach (var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}
