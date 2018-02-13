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
    public class LocalGovernmentAnnualPowerInterruptionsController : ApiController
    {
        private PowerInterruptionsEntities db = new PowerInterruptionsEntities();

        // GET: api/LocalGovernmentAnnualPowerInterruptions
        public object GetInterruptions(int year)
        {
            var sql = db.Interruptions
                .Where(p => p.startDate.Year == year)
                .GroupBy(p => p.localGov)
                .Select(p => new
                {
                    lga = p.Key,
                    customers = p.Sum(p2 => p2.customers),
                    avgCustomers = Math.Round(p.Average(p2 => p2.customers)),
                    maxDuration = p.Max(p2 => p2.duration),
                    minDuration = p.Min(p2 => p2.duration),
                    avgDuration = Math.Round(p.Average(p2 => p2.duration), 2)
                })
            .OrderBy(p => p.lga);

            return sql.ToList();
        }

    }
}