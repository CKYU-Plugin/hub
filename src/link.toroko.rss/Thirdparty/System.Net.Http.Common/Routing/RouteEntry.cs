// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.
using TRoute = System.Web.Http.Routing.IHttpRoute;

namespace System.Web.Http.Routing
{
    /// <summary>Represents a named route.</summary>
    public class RouteEntry
    {
        private readonly string _name;
        private readonly TRoute _route;

        /// <summary>Initializes a new instance of the <see cref="RouteEntry"/> class.</summary>
        /// <param name="name">The route name, if any; otherwise, <see langword="null"/>.</param>
        /// <param name="route">The route.</param>
        public RouteEntry(string name, TRoute route)
        {
            if (route == null)
            {
                throw new ArgumentNullException("route");
            }

            _name = name;
            _route = route;
        }

        /// <summary>Gets the route name, if any; otherwise, <see langword="null"/>.</summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>Gets the route.</summary>
        public TRoute Route
        {
            get { return _route; }
        }
    }
}
