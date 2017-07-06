using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Korhaz;

namespace RecoveryServer.App_Start
{
    public class HistoryController : ApiController
    {
        // GET: api/History
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/History/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/History
        public List<Elozmeny> Post([FromBody]string[] Params)
        {

            KapcsolatKezelo kapcs = new KapcsolatKezelo();
            List<Elozmeny> elo = kapcs.ElozmenyKerdezes(int.Parse(Params[0]), Params[1], Params[2], Params[3]);
            return elo;

        }

        // PUT: api/History/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/History/5
        public void Delete(int id)
        {
        }
    }
}
