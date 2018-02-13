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
    public class LocalGovernmentsController : ApiController
    {
        private PowerInterruptionsEntities db = new PowerInterruptionsEntities();

        // GET: api/LocalGovernments
        public object GetILocalGovernments()
        {
            return db.Interruptions
                .OrderBy(p => p.localGov)
                .Select(p => p.localGov)
                .ToList();
        }

        // GET: api/LocalGovernments/2015
        public object GetInterruption(int year)
        {
            return db.Interruptions
                .Where(p => p.startDate.Year == year)
                 .OrderBy(p => p.localGov)
                 .Select(p => p.localGov)
                 .ToList();
        }

    }
}