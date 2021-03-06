// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.ObjectModel;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace System.Web.Http
{
    /// <summary>
    /// Specifies that an action supports the HEAD HTTP method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HttpHeadAttribute : Attribute, IActionHttpMethodProvider
    {
        private static readonly Collection<HttpMethod> _supportedMethods = new Collection<HttpMethod>(new HttpMethod[] { HttpMethod.Head });

        public Collection<HttpMethod> HttpMethods
        {
            get
            {
                return _supportedMethods;
            }
        }
    }
}
