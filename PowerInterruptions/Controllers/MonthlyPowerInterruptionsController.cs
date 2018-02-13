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
    public class MonthlyPowerInterruptionsController : ApiController
    {
        private PowerInterruptionsEntities db = new PowerInterruptionsEntities();

        // GET: api/MonthlyPowerInterruptions/5
        //      /api/MonthlyPowerInterruptions/2015/
        [ResponseType(typeof(object))]
        public IHttpActionResult GetInterruption(int year)
        {
            int[] months = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            int[] days = { 0, 1, 2, 3, 4, 5, 6 };

            var sql =
                db.Interruptions
                .Where(p => p.startDate.Year == year)
                .GroupBy(p => p.startDate.Month)
                .Select(p => new
                {
                    month = p.Key,
                    totalEvents = p.Count(),
                    eventDetail = new
                    {
                        thirdParyEvent = p.Count(p2 => p2.reason.Contains("Third party")),
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
                    avgDuration = Math.Round(p.Average(p2 => p2.duration), 2),
                    maxDuration = p.Max(p2 => p2.duration),
                    minDuration = p.Min(p2 => p2.duration),
                    dailyStats = p
                })
           .OrderBy(p => p.month).ToList();

            var result = (from m in months
                          join d in sql
                          on m equals d.month
                          into gp
                          from allData in gp.DefaultIfEmpty()
                          select new
                          {
                              month = m,
                              monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
                              totalEvents = allData != null ? allData.totalEvents : 0, // or use -1 and show N/A
                              eventDetail = allData != null ? allData.eventDetail : null,
                              customers = allData != null ? allData.customers : 0,
                              avgCustomers = allData != null ? allData.avgCustomers : 0,
                              avgDuration = allData != null ? allData.avgDuration : 0,
                              maxDuration = allData != null ? allData.maxDuration : 0,
                              minDuration = allData != null ? allData.minDuration : 0,
                              dailyStats = allData != null ? (
                              from d in days
                              join data in allData.dailyStats.DefaultIfEmpty()
                              on d equals Convert.ToInt32(data.startDate.DayOfWeek)
                              into gp2
                              from dailyData in gp2.DefaultIfEmpty()
                              select dailyData).GroupBy(p => p.startDate.DayOfWeek)
                                             .Select(p => new
                                             {
                                                 day = p.Key,
                                                 dayName = Enum.GetName(typeof(DayOfWeek), p.Key),
                                                 dailyTotalEvents = p.Count(),
                                                 dailyEventDetail = new
                                                 {
                                                     thirdParyEvent = p.Count(p2 => p2.reason.Contains("Third party")),
                                                     customerEvent = p.Count(p2 => p2.reason == "Customer installation"),
                                                     digEvent = p.Count(p2 => p2.reason == "Cable dig"),
                                                     vandalEvent = p.Count(p2 => p2.reason == "Vandalism"),
                                                     enviroEvent = p.Count(p2 => p2.reason == "Environmental"),
                                                     equipEvent = p.Count(p2 => p2.reason == "Equipment fault"),
                                                     lightEvent = p.Count(p2 => p2.reason == "Lightning"),
                                                     directedEvent = p.Count(p2 => p2.reason.Contains("Directed")),
                                                     opEvent = p.Count(p2 => p2.reason == "Operating fault"),
                                                 },
                                                 dailyCustomers = p.Sum(p2 => p2.customers),
                                                 dailyAvgCustomers = Math.Round(p.Average(p2 => p2.customers)),
                                                 dailyAvgDuration = Math.Round(p.Average(p2 => p2.duration), 2),
                                                 dailyMaxDuration = p.Max(p2 => p2.duration),
                                                 dailyMinDuration = p.Min(p2 => p2.duration)
                                             })
                                             .OrderBy(p2 => p2.day)
                                             .ToList()
                              : null // no monthly data as it is misisng!
                          }).ToList();


            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }


}
