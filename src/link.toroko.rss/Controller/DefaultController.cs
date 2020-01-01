using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace link.toroko.rsshub.Controller
{
    [Route("[controller]")]
    public class DefaultController : ApiController
    {
        /// <summary>
        /// Default controller for render swagger UI 
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        public RedirectResult Index()
        {
            var requestUri = Request.RequestUri;
            return Redirect(requestUri.AbsoluteUri + "api");
        }
    }
}
