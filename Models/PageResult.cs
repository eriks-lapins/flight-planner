using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightPlanner.Models
{
    public class PageResult
    {
        public int Page { get; set; } = 0;
        public int TotalItems { get; set; } = 0;
        public List<Object> Items { get; set; } = new List<Object>();
    }
}