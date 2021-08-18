using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightPlanner.Models
{
    public class FlightStorage
    {
        public static List<Flight> AllFlights = new List<Flight>() ;
        private static int _id;
        public static Flight AddFlight(Flight input)
        {
            input.Id = _id;
            _id++;
            AllFlights.Add(input);

            return input;
        }

        public static Flight FindFlightById(int id)
        {
            return AllFlights.FirstOrDefault(x =>
            {
                if (x != null)
                {
                    return x.Id == id;
                }

                return false;
            });
        }

        public static Flight FindFlightByString(string input)
        {
            return AllFlights.FirstOrDefault
            (
                x => 
                    CleanText(x.To.AirportName).Contains(CleanText(input)) ||
                    CleanText(x.To.Country).Contains(CleanText(input)) ||
                    CleanText(x.To.City).Contains(CleanText(input)) ||
                    CleanText(x.From.AirportName).Contains(CleanText(input)) ||
                    CleanText(x.From.Country).Contains(CleanText(input)) ||
                    CleanText(x.From.City).Contains(CleanText(input))
            );
        }

        private static string CleanText(string input)
        {
            return input.ToUpper().Trim();
        }
    }
}