﻿using DXCarsCrud.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace DXCarsCrud.Controllers
{
    public class WeatherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> SearchByCityAsync(string? city)
        {
            if (city == null)
            {
                city = "cairo";
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //GET Method
                HttpResponseMessage response = await client.GetAsync(Configrations.Values.apiUrl + "q=" + city);
                WeatherResponse WeatherResponse = new();
                if (response.IsSuccessStatusCode)
                {
                    WeatherResponse = await response.Content.ReadFromJsonAsync<WeatherResponse>();

                    return View(WeatherResponse);
                }
                else
                {
                    ModelState.AddModelError("error", "enter a valid city name");
                    city = "cairo";
                    response = await client.GetAsync(Configrations.Values.apiUrl + "q=" + city);
                    if (response.IsSuccessStatusCode)
                    {
                        WeatherResponse = await response.Content.ReadFromJsonAsync<WeatherResponse>();

                        return View(WeatherResponse);
                    }

                    return BadRequest(WeatherResponse);
                }
            }

            return View();
        }
    }
}
