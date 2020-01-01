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
    [RoutePrefix("api")]
    public class ApiHelpController : ApiController
    {
        [HttpGet, Route("")]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage result= new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                string clientAddress = this.Request.GetOwinContext().Request.RemoteIpAddress;
                result.Content = new ByteArrayContent(Encoding.ASCII.GetBytes(clientAddress));
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            }
            catch{}
            return result;
        }

        [HttpPost, Route("")]
        public IHttpActionResult Post()
        {
            return Ok("API");
        }
    }
}
