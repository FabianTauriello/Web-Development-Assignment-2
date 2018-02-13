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
    public class LocalGovernmentMonthlyPowerInterruptionsController : ApiController
    {
        private PowerInterruptionsEntities db = new PowerInterruptionsEntities();

        [ResponseType(typeof(object))]
        public IHttpActionResult GetInterruption(int year, string lga)
        {
            int[] months = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            int[] days = { 0, 1, 2, 3, 4, 5, 6 };

            var sql =
                db.Interruptions
                .Where(p => p.startDate.Year == year)
                .Where(p => p.localGov == lga)
                .GroupBy(p => p.startDate.Month)
                .Select(p => new
                {
                    month = p.Key,
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
                              customers = allData.customers,
                              avgCustomers = allData.avgCustomers,
                              avgDuration = allData.avgDuration,
                              maxDuration = allData.maxDuration,
                              minDuration = allData.minDuration,
                              monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
                              dailyStats = (from d in days
                                            join data in allData.dailyStats
                                            on d equals Convert.ToInt32(data.startDate.DayOfWeek)
                                            into gp2
                                            from dailyData in gp2.DefaultIfEmpty()
                                            select dailyData
                                            )
                                              .GroupBy(p => p.startDate.DayOfWeek)
                                           .Select(p => new
                                           {
                                               day = p.Key,
                                               dayName = Enum.GetName(typeof(DayOfWeek), p.Key),
                                               dailyCustomers = p.Sum(p2 => p2.customers),
                                               dailyAvgCustomers = Math.Round(p.Average(p2 => p2.customers)),
                                               dailyAvgDuration = Math.Round(p.Average(p2 => p2.duration), 2),
                                               dailyMaxDuration = p.Max(p2 => p2.duration),
                                               dailyMinDuration = p.Min(p2 => p2.duration)
                                           })
                                           .OrderBy(p2 => p2.day)
                                           .ToList()
                          }).ToList();


            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}