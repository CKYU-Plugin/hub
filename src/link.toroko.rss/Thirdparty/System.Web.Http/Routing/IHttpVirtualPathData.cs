// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Web.Http.Routing
{
    public interface IHttpVirtualPathData
    {
        IHttpRoute Route { get; }

        string VirtualPath { get; set; }
    }
}
