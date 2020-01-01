// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Web.Http.Routing
{
    /// <summary>Defines a route prefix.</summary>
    public interface IRoutePrefix
    {
        /// <summary>Gets the route prefix.</summary>
        string Prefix { get; }
    }
}