// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Web.Http.Routing
{
    /// <summary>
    /// Defines a factory that creates a route directly to a set of action descriptors (an attribute route).
    /// </summary>
    public interface IDirectRouteFactory
    {
        /// <summary>Creates a direct route entry.</summary>
        /// <param name="context">The context to use to create the route.</param>
        /// <returns>The direct route entry.</returns>
        RouteEntry CreateRoute(DirectRouteFactoryContext context);
    }
}
