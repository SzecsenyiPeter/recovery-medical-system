using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Korhaz;


namespace RecoveryServer.Controllers
{
    public class LoginController : ApiController
    {
        // GET: api/Login
        //public IEnumerable<string> Get()
        //{

        //    return new string[] { "value1", "value2" };
        //}

        //GET: api/Login/5
        public UserInfo Get([FromBody]UserInfo Login)
        {
            
            return Login; 
        }



        // POST: api/Login
        public string Post([FromBody]UserInfo Login)
        {
            KapcsolatKezelo kapcs = new KapcsolatKezelo();
            return kapcs.Login(Login.User, Login.PassworldHash);
            
        }

        // PUT: api/Login/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Login/5
        public void Delete(int id)
        {
        }
    }
}
