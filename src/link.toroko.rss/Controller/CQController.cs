using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace link.toroko.rsshub
{
    public class CQController : ApiController
    {

        [HttpGet, Route("api/values/RandomImage")]

        public HttpResponseMessage RandomImage()
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(new byte[] { });
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return result;
        }

        // Get api/values
        [HttpHead, Route("api/values")]
        public HttpResponseMessage Head()
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            string clientAddress = this.Request.GetOwinContext().Request.RemoteIpAddress;
            var qstring = this.Request.GetOwinContext().Request.QueryString;
            result.Content = new ByteArrayContent(Encoding.ASCII.GetBytes(clientAddress + qstring.Value[0].ToString()));
            return result;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var result = new
            {
                UserName = "Michael",
                City = "Taipei"
            };
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5 
        [HttpPut]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        [HttpDelete]
        public void Delete(int id)
        {
        }

        [Route("api/values/{id:int}")]
        public string Get(string id)
        {
            return id;
        }

        public HttpResponseMessage HandMadeJson()
        {
            var data = new Dictionary<string, string>() {
                {"message", "json"}
            };

            string json = JsonConvert.SerializeObject(data);
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StringContent(json);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return result;
        }

        //public Dictionary<string, string> Get()
        //{
        //    var result = new Dictionary<string, string>()
        //    {
        //        {"001", "Banana"},
        //        {"002", "Apple"},
        //        {"003", "Orange"}
        //    };
        //    return result;
        //}

    }
}
