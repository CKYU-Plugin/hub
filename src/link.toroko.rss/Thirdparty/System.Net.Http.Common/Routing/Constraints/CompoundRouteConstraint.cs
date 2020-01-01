// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net.Http;

namespace System.Web.Http.Routing.Constraints
{
    /// <summary>
    /// Constrains a route by several child constraints.
    /// </summary>
    public class CompoundRouteConstraint : IHttpRouteConstraint
    {
        public CompoundRouteConstraint(IList<IHttpRouteConstraint> constraints)
        {
            if (constraints == null)
            {
                throw Error.ArgumentNull("constraints");
            }

            Constraints = constraints;
        }

        /// <summary>
        /// Gets the child constraints that must match for this constraint to match.
        /// </summary>
        public IEnumerable<IHttpRouteConstraint> Constraints { get; private set; }

        /// <inheritdoc />
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            foreach (var constraint in Constraints)
            {
                if (!constraint.Match(request, route, parameterName, values, routeDirection))
                {
                    return false;
                }
            }
            return true;
        }
    }
}