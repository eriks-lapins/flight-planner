using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using FlightPlanner.Attributes;
using FlightPlanner.Models;
using Microsoft.Ajax.Utilities;

namespace FlightPlanner.Controllers
{
    public class CustomerApiController : ApiController
    {
        [Route("api/airports"), HttpGet]
        public IHttpActionResult SearchAirports(string search)
        {
            string input = CleanText(search);
            var flight = FlightStorage.FindFlightByString(input);
            if (CleanText(flight.From.AirportName).Contains(input) || CleanText(flight.From.City).Contains(input) ||
                CleanText(flight.From.Country).Contains(input))
            {
                return Ok(new [] { NewAirport(flight.From) });
            }
            if (CleanText(flight.To.AirportName).Contains(input) || CleanText(flight.To.City).Contains(input) ||
                CleanText(flight.To.Country).Contains(input))
            {
                return Ok(new[] { NewAirport(flight.To)});
            }

            return NotFound();
        }

        [Route("api/flights/search"), HttpPost]
        public IHttpActionResult SearchFlights(SearchFlightsRequest search)
        {
            var page = new PageResult();
            if (IsWrongFormat(search) || HasSameAirport(search))
            {
                return BadRequest();
            }

            if (IsAirportObject(search.AsAirport))
            {
                return BadRequest();
            }

            foreach (Flight flight in FlightStorage.AllFlights)
            {
                if (search.DepartureTime == null)
                {
                    string flightDepartureDate = Convert.ToDateTime(flight.DepartureTime).ToString("yyyy-MM-dd");
                    if (flight.To.AirportName == search.To && flight.From.AirportName == search.From && flightDepartureDate == search.DepartureDate)
                    {
                        FlightStorage.AddSelectedFlight(search);
                    }
                }
                if (flight.To.AirportName == search.To && flight.From.AirportName == search.From && flight.DepartureTime == search.DepartureTime)
                {
                    FlightStorage.AddSelectedFlight(search);
                }
            }

            if (FlightStorage.SelectedFlights.Count == 0)
            {
                return Ok(page);
            }

            foreach (var flight in FlightStorage.SelectedFlights)
            {
                page.Items.Add(flight);
                page.TotalItems++;
            }
            return Ok(page);
        }

        [Route("api/flights/{id}"), HttpGet]
        public IHttpActionResult FindFlightById(int id)
        {
            var output = FlightStorage.FindFlightById(id);
            if (output == null)
            {
                return NotFound();
            }

            return Ok(output);
        }

        private Airport NewAirport(Airport input)
        {
            Airport output = new Airport
            {
                Country = input.Country,
                City =  input.City,
                AirportName = input.AirportName
            };

            return output;
        }
        private static string CleanText(string input)
        {
            return input.ToUpper().Trim();
        }

        private static bool IsWrongFormat(SearchFlightsRequest input)
        {
            if (input == null)
            {
                return true;
            }
            var unwantedSymbols = new Object[] { null, "" };
            var inputValues = new List<Object>();
            if (input.To != null && input.From != null)
            {
                inputValues.AddRange(new List<Object>
                {
                    input.DepartureDate,
                    input.To,
                    input.From,
                });
            }
            else
            {
                return true;
            }

            foreach (var value in unwantedSymbols)
            {
                if (inputValues.Contains(value))
                {
                    return true;
                }
            }


            return false;
        }

        private static bool HasSameAirport(SearchFlightsRequest input)
        {
            if (input == null)
            {
                return false;
            }

            if (input.To != null && input.From != null)
            {
                if (input.To.ToUpper().Trim() == input.From.ToUpper().Trim())
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsAirportObject(Airport input)
        {
            if (input == null)
            {
                return false;
            }
            if (input.AirportName != null || input.City != null || input.Country != null)
            {
                return true;
            }

            return false;
        }

    }
}