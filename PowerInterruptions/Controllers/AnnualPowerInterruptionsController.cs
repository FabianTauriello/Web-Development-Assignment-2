using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PowerInterruptions.Models;

namespace PowerInterruptions.Controllers
{
    public class AnnualPowerInterruptionsController : ApiController
    {
        private PowerInterruptionsEntities db = new PowerInterruptionsEntities();

        // GET: api/AnnualPowerInterruptions
        public object GetInterruptions()
        {

            var sql = db.Interruptions.GroupBy(p => p.startDate.Year).Select(p => new
            {
                year = p.Key,
                totalEvents = p.Count(),
                eventDetail = new
                {
                    thirdPartyEvent = p.Count(p2 => p2.reason.Contains("Third party")),
                    customerEvent = p.Count(p2 => p2.reason == "Customer installation"),
                    digEvent = p.Count(p2 => p2.reason == "Cable dig"),
                    vandalEvent = p.Count(p2 => p2.reason == "Vandalism"),
                    enviroEvent = p.Count(p2 => p2.reason == "Environmental"),
                    equipEvent = p.Count(p2 => p2.reason == "Equipment fault"),
                    lightEvent = p.Count(p2 => p2.reason == "Lightning"),
                    directedEvent = p.Count(p2 => p2.reason.Contains("Directed")),
                    opEvent = p.Count(p2 => p2.reason == "Operating fault"),
                },
                customers = p.Sum(p2 => p2.customers),
                avgCustomers = Math.Round(p.Average(p2 => p2.customers)),
                maxDuration = p.Max(p2 => p2.duration),
                minDuration = p.Min(p2 => p2.duration),
                avgDuration = Math.Round(p.Average(p2 => p2.duration), 2)
            })
            .OrderBy(p => p.year);


            return sql.ToList();
        }
    }
}