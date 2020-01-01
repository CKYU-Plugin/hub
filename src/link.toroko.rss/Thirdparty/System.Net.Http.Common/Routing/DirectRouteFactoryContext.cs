// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Properties;
using TActionDescriptor = System.Web.Http.Controllers.HttpActionDescriptor;
using TParsedRoute = System.Web.Http.Routing.HttpParsedRoute;
using TRouteDictionary = System.Web.Http.Routing.HttpRouteValueDictionary;

namespace System.Web.Http.Routing
{
    /// <summary>Represents a context that supports creating a direct route.</summary>
    public class DirectRouteFactoryContext
    {
        private readonly string _actionName;

        private readonly string _prefix;

        private readonly IReadOnlyCollection<TActionDescriptor> _actions;
        private readonly IInlineConstraintResolver _inlineConstraintResolver;
        private readonly bool _targetIsAction;

        /// <summary>Initializes a new instance of the <see cref="DirectRouteFactoryContext"/></summary>
        /// <param name="prefix">The route prefix, if any, defined by the controller.</param>
        /// <param name="actions">The action descriptors to which to create a route.</param>
        /// <param name="inlineConstraintResolver">The inline constraint resolver.</param>
        /// <param name="targetIsAction">
        /// A value indicating whether the route is configured at the action or controller level.
        /// </param>
        public DirectRouteFactoryContext(string prefix, IReadOnlyCollection<HttpActionDescriptor> actions,
            IInlineConstraintResolver inlineConstraintResolver, bool targetIsAction)
        {
            if (actions == null)
            {
                throw new ArgumentNullException("actions");
            }

            if (inlineConstraintResolver == null)
            {
                throw new ArgumentNullException("inlineConstraintResolver");
            }

            _prefix = prefix;
            _actions = actions;
            _inlineConstraintResolver = inlineConstraintResolver;

            TActionDescriptor firstDescriptor = actions.FirstOrDefault();

            if (firstDescriptor != null)
            {
                _actionName = firstDescriptor.ActionName;
            }

            _targetIsAction = targetIsAction;
        }

        /// <summary>Gets the route prefix, if any, defined by the controller.</summary>
        public string Prefix
        {
            get { return _prefix; }
        }

        /// <summary>Gets the action descriptors to which to create a route.</summary>
        public IReadOnlyCollection<TActionDescriptor> Actions
        {
            get { return _actions; }
        }

        /// <summary>Gets the inline constraint resolver.</summary>
        public IInlineConstraintResolver InlineConstraintResolver
        {
            get { return _inlineConstraintResolver; }
        }

        /// <summary>
        /// Gets a value indicating whether the route is configured at the action or controller level.
        /// </summary>
        /// <remarks>
        /// <see langword="true"/> when the route is configured at the action level; otherwise <see langword="false"/>
        /// (if the route is configured at the controller level).
        /// </remarks>
        public bool TargetIsAction
        {
            get { return _targetIsAction; }
        }

        /// <summary>Creates a route builder that can build a route matching this context.</summary>
        /// <param name="template">The route template.</param>
        /// <returns>A route builder that can build a route matching this context.</returns>
        public IDirectRouteBuilder CreateBuilder(string template)
        {
            return CreateBuilderInternal(template);
        }

        internal virtual IDirectRouteBuilder CreateBuilderInternal(string template)
        {
            return CreateBuilder(template, _inlineConstraintResolver);
        }

        /// <summary>Creates a route builder that can build a route matching this context.</summary>
        /// <param name="template">The route template.</param>
        /// <param name="constraintResolver">
        /// The inline constraint resolver to use, if any; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>A route builder that can build a route matching this context.</returns>
        public IDirectRouteBuilder CreateBuilder(string template, IInlineConstraintResolver constraintResolver)
        {
            DirectRouteBuilder builder = new DirectRouteBuilder(_actions, _targetIsAction);

            string prefixedTemplate = BuildRouteTemplate(_prefix, template);
            ValidateTemplate(prefixedTemplate);

            if (constraintResolver != null)
            {
                TRouteDictionary defaults = new TRouteDictionary();
                TRouteDictionary constraints = new TRouteDictionary();

                string detokenizedTemplate = InlineRouteTemplateParser.ParseRouteTemplate(prefixedTemplate, defaults,
                    constraints, constraintResolver);
                TParsedRoute parsedRoute = RouteParser.Parse(detokenizedTemplate);
                decimal precedence = RoutePrecedence.Compute(parsedRoute, constraints);

                builder.Defaults = defaults;
                builder.Constraints = constraints;
                builder.Template = detokenizedTemplate;
                builder.Precedence = precedence;
                builder.ParsedRoute = parsedRoute;
            }
            else
            {
                builder.Template = prefixedTemplate;
            }

            return builder;
        }

        private static string BuildRouteTemplate(string routePrefix, string routeTemplate)
        {
            if (String.IsNullOrEmpty(routeTemplate))
            {
                return routePrefix ?? String.Empty;
            }

            // If the provider's template starts with '~/', ignore the route prefix
            if (routeTemplate.StartsWith("~/", StringComparison.Ordinal))
            {
                return routeTemplate.Substring(2);
            }
            else if (String.IsNullOrEmpty(routePrefix))
            {
                return routeTemplate;
            }
            else
            {
                // template and prefix both not null - combine them
                return routePrefix + '/' + routeTemplate;
            }
        }

        private void ValidateTemplate(string template)
        {
            if (template != null && template.StartsWith("/", StringComparison.Ordinal))
            {
                string errorMessage = Error.Format(SRResources.AttributeRoutes_InvalidTemplate, template, _actionName);
                throw new InvalidOperationException(errorMessage);
            }
        }
    }
}
