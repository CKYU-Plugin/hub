// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Properties;
using TActionDescriptor = System.Web.Http.Controllers.HttpActionDescriptor;
using TParsedRoute = System.Web.Http.Routing.HttpParsedRoute;
using TResources = System.Web.Http.Properties.SRResources;
using TRoute = System.Web.Http.Routing.IHttpRoute;
using TRouteDictionary = System.Collections.Generic.IDictionary<string, object>;
using TRouteDictionaryConcrete = System.Web.Http.Routing.HttpRouteValueDictionary;

namespace System.Web.Http.Routing
{
    /// <summary>Represents a builder that creates direct routes to actions (attribute routes).</summary>
    internal class DirectRouteBuilder : IDirectRouteBuilder
    {
        private readonly TActionDescriptor[] _actions;
        private readonly bool _targetIsAction;

        private string _template;

        /// <summary>Initializes a new instance of the <see cref="DirectRouteBuilder"/> class.</summary>
        /// <param name="actions">The action descriptors to which to create a route.</param>
        /// <param name="targetIsAction">
        /// A value indicating whether the route is configured at the action or controller level.
        /// </param>
        public DirectRouteBuilder(IReadOnlyCollection<TActionDescriptor> actions, bool targetIsAction)
        {
            if (actions == null)
            {
                throw new ArgumentNullException("actions");
            }

            _actions = actions.ToArray();

            _targetIsAction = targetIsAction;
        }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Template
        {
            get
            {
                return _template;
            }
            set
            {
                ParsedRoute = null;
                _template = value;
            }
        }

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "Null and empty values are legitimate, separate options when constructing a route.")]
        public TRouteDictionary Defaults { get; set; }

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "Null and empty values are legitimate, separate options when constructing a route.")]
        public TRouteDictionary Constraints { get; set; }

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "Null and empty values are legitimate, separate options when constructing a route.")]
        public TRouteDictionary DataTokens { get; set; }

        internal TParsedRoute ParsedRoute { get; set; }

        /// <inheritdoc/>
        public int Order { get; set; }

        /// <inheritdoc/>
        public decimal Precedence { get; set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<TActionDescriptor> Actions
        {
            get { return _actions; }
        }

        /// <inheritdoc/>
        public bool TargetIsAction
        {
            get { return _targetIsAction; }
        }

        /// <inheritdoc/>
        public virtual RouteEntry Build()
        {
            if (ParsedRoute == null)
            {
                ParsedRoute = RouteParser.Parse(Template);
            }

            ValidateParameters(ParsedRoute);

            TRouteDictionaryConcrete defaults;
            defaults = Copy(Defaults);
            TRouteDictionaryConcrete constraints = Copy(Constraints);
            TRouteDictionaryConcrete dataTokens = Copy(DataTokens) ?? new TRouteDictionaryConcrete();

            dataTokens[RouteDataTokenKeys.Actions] = _actions;

            if (!TargetIsAction)
            {
                dataTokens[RouteDataTokenKeys.Controller] = _actions[0].ControllerDescriptor;
            }

            int order = Order;

            if (order != default(int))
            {
                dataTokens[RouteDataTokenKeys.Order] = order;
            }

            decimal precedence = Precedence;

            if (precedence != default(decimal))
            {
                dataTokens[RouteDataTokenKeys.Precedence] = precedence;
            }

            if (constraints != null)
            {
                foreach (var constraint in constraints)
                {
                    HttpRoute.ValidateConstraint(Template, constraint.Key, constraint.Value);
                }
            }

            HttpMessageHandler handler = null;
            IHttpRoute route = new HttpRoute(Template, defaults, constraints, dataTokens, handler, ParsedRoute);

            return new RouteEntry(Name, route);
        }

        // Accessible for tests
        internal virtual void ValidateParameters(TParsedRoute parsedRoute)
        {
            Contract.Assert(parsedRoute != null);

            if (parsedRoute.PathSegments != null)
            {
                foreach (var contentSegment in parsedRoute.PathSegments.OfType<PathContentSegment>())
                {
                    if (contentSegment != null && contentSegment.Subsegments != null)
                    {
                        foreach (var parameterSegment in contentSegment.Subsegments.OfType<PathParameterSubsegment>())
                        {
                            if (parameterSegment != null)
                            {
                                if (String.Equals(parameterSegment.ParameterName, "controller", StringComparison.OrdinalIgnoreCase))
                                {
                                    throw Error.InvalidOperation(TResources.DirectRoute_InvalidParameter_Controller);
                                }
                                else if (TargetIsAction && String.Equals(parameterSegment.ParameterName, "action", StringComparison.OrdinalIgnoreCase))
                                {
                                    throw Error.InvalidOperation(TResources.DirectRoute_InvalidParameter_Action);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static void ValidateRouteEntry(RouteEntry entry)
        {
            Contract.Assert(entry != null);

            TRoute route = entry.Route;
            Contract.Assert(route != null);

            TActionDescriptor[] targetActions = route.GetTargetActionDescriptors();

            if (targetActions == null || targetActions.Length == 0)
            {
                throw new InvalidOperationException(TResources.DirectRoute_MissingActionDescriptors);
            }
            if (route.Handler != null)
            {
                throw new InvalidOperationException(TResources.DirectRoute_HandlerNotSupported);
            }
        }

        private static TRouteDictionaryConcrete Copy(TRouteDictionary routeDictionary)
        {
            if (routeDictionary == null)
            {
                return null;
            }

            return new TRouteDictionaryConcrete(routeDictionary);
        }

    }
}
