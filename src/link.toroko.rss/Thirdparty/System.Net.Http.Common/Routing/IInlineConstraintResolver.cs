﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.
using TConstraint = System.Web.Http.Routing.IHttpRouteConstraint;

namespace System.Web.Http.Routing
{
    /// <summary>
    /// Defines an abstraction for resolving inline constraints as instances of <see cref="TConstraint"/>.
    /// </summary>
    public interface IInlineConstraintResolver
    {
        /// <summary>
        /// Resolves the inline constraint.
        /// </summary>
        /// <param name="inlineConstraint">The inline constraint to resolve.</param>
        /// <returns>The <see cref="TConstraint"/> the inline constraint was resolved to.</returns>
        TConstraint ResolveConstraint(string inlineConstraint);
    }
}