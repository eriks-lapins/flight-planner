using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Antlr.Runtime;
using FlightPlanner.Attributes;
using FlightPlanner.Models;
using Microsoft.Ajax.Utilities;

namespace FlightPlanner.Controllers
{
    [BasicAuthentication]
    public class AdminApiController : ApiController
    {
        [Route("admin-api/flights/{id}")]
        public IHttpActionResult GetFlights(int id)
        {
            var output = FlightStorage.FindFlightById(id);
            if (output == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [Route("admin-api/flights")]
        public IHttpActionResult PutFlight(AddFlightRequest input)
        {
            Flight output = new Flight
            {
                ArrivalTime = input.ArrivalTime,
                DepartureTime = input.DepartureTime,
                From = input.From,
                To = input.To,
                Carrier = input.Carrier
            };

            if (IsWrongFormat(input) || HasSameAirport(input) || HasStrangeDates(input))
            {
                return BadRequest();
            }
             
            if (IsDuplicateRequest(input))
            {
                return Conflict();
            }

            FlightStorage.AddFlight(output);

            return Created("", output);
        }

        [Route("admin-api/flights/{id}"), HttpDelete]
        public IHttpActionResult DeleteFlight(int id)
        {
            var output = FlightStorage.FindFlightById(id);

            if (output == null)
            {
                return Ok();
            }

            FlightStorage.AllFlights.Remove(output);
            return Ok();
        }

        private bool IsDuplicateRequest(AddFlightRequest input)
        {
            var tempList = new List<Flight>(FlightStorage.AllFlights);
            if (tempList != null)
            {
                foreach (Flight flight in tempList)
                {
                    if (flight != null)
                    {
                        if
                        (
                            flight.From != null &&
                            flight.To != null &&
                            input.To != null &&
                            input.From != null
                        )
                        {
                            if
                            (
                                input.ArrivalTime == flight.ArrivalTime &&
                                input.DepartureTime == flight.DepartureTime &&
                                input.From.AirportName == flight.From.AirportName &&
                                input.From.City == flight.From.City &&
                                input.From.Country == flight.From.Country &&
                                input.To.AirportName == flight.To.AirportName &&
                                input.To.City == flight.To.City &&
                                input.To.Country == flight.To.Country &&
                                input.Carrier == flight.Carrier
                            )
                            {
                                return true;
                            }

                        }
                    }
                }
            }
            
            return false;
        }

        private bool IsWrongFormat(AddFlightRequest input)
        {

            var unwantedSymbols = new Object[] {null, "" };
            var inputValues = new List<Object>();
            if (input.To != null && input.From != null)
            {
                inputValues.AddRange(new List<Object>
                    {
                        input.ArrivalTime,
                        input.DepartureTime,
                        input.To,
                        input.From,
                        input.Carrier,
                        input.From.AirportName,
                        input.From.City,
                        input.From.Country,
                        input.To.City,
                        input.To.Country,
                        input.Carrier
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

        private bool HasSameAirport(AddFlightRequest input)
        {
            if (input.To != null && input.From != null)
            {
                if (input.To.AirportName.ToUpper().Trim() == input.From.AirportName.ToUpper().Trim())
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasStrangeDates(AddFlightRequest input)
        {
            if (input.To != null && input.From != null)
            {
                long elapsedTicks = DateTime.Parse(input.ArrivalTime).Ticks - DateTime.Parse(input.DepartureTime).Ticks;
                var hours = Math.Round(elapsedTicks / double.Parse(TimeSpan.TicksPerHour.ToString()));
                if (hours < 0.5 || hours > 96)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
