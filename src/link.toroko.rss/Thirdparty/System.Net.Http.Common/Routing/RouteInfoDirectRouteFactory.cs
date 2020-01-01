// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics.Contracts;
using TRouteInfoProvider = System.Web.Http.Routing.IHttpRouteInfoProvider;

namespace System.Web.Http.Routing
{
    /// <remarks>
    /// This class is an adapter that turns an IHttpRouteInfoProvider into an IDirectRouteFactory. We need it because
    /// we already shipped IHttpRouteInfoProvider but want to standardize the internal implementation around the more
    /// general IDirectRouteFactory interface.
    /// We can remove this class if we ever stop supporting custom attributes that implement IHttpRouteInfoProvider.
    /// </remarks>
    internal class RouteInfoDirectRouteFactory : IDirectRouteFactory
    {
        private readonly TRouteInfoProvider _infoProvider;

        public RouteInfoDirectRouteFactory(TRouteInfoProvider infoProvider)
        {
            if (infoProvider == null)
            {
                throw new ArgumentNullException("infoProvider");
            }

            _infoProvider = infoProvider;
        }

        public RouteEntry CreateRoute(DirectRouteFactoryContext context)
        {
            Contract.Assert(context != null);

            IDirectRouteBuilder builder = context.CreateBuilder(_infoProvider.Template);
            Contract.Assert(builder != null);

            builder.Name = _infoProvider.Name;
            builder.Order = _infoProvider.Order;

            return builder.Build();
        }
    }
}
