// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

namespace System.Web.Http.Routing.Constraints
{
    /// <summary>
    /// Constrains a route parameter to represent only 32-bit integer values.
    /// </summary>
    public class IntRouteConstraint : IHttpRouteConstraint

    {
        /// <inheritdoc />
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (parameterName == null)
            {
                throw Error.ArgumentNull("parameterName");
            }

            if (values == null)
            {
                throw Error.ArgumentNull("values");
            }

            object value;
            if (values.TryGetValue(parameterName, out value) && value != null)
            {
                if (value is int)
                {
                    return true;
                }

                int result;
                string valueString = Convert.ToString(value, CultureInfo.InvariantCulture);
                return Int32.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
            }
            return false;
        }
    }
}