using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using FlightPlanner.Attributes;
using FlightPlanner.Models;

namespace FlightPlanner.Controllers
{
    public class CustomerApiController : ApiController
    {
        [Route("api/airports"), HttpGet]
        public IHttpActionResult SearchAirports(string input)
        {
            input = CleanText(input);
            var flight = FlightStorage.FindFlightByString(input);
            if (flight.From.AirportName.Contains(input) || flight.From.City.Contains(input) ||
                flight.From.Country.Contains(input))
            {
                return Created("", NewAirport(flight.From));
            }
            if (flight.To.AirportName.Contains(input) || flight.To.City.Contains(input) ||
                flight.To.Country.Contains(input))
            {
                return Created("", NewAirport(flight.To));
            }

            return Conflict();
        }

        public IHttpActionResult SearchFlights(Flight input)
        {
            return Ok();
        }

        public IHttpActionResult FindFlightById(int id)
        {
            return Ok();
        }

        private IHttpActionResult NewAirport(Airport input)
        {
            Airport output = new Airport
            {
                Country = input.Country,
                City =  input.City,
                AirportName = input.AirportName
            };

            return Created("", output);
        }
        private static string CleanText(string input)
        {
            return input.ToUpper().Trim();
        }

    }
}