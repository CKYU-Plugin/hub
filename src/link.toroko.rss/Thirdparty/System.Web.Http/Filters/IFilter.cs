// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Web.Http.Filters
{
    public interface IFilter
    {
        bool AllowMultiple { get; }
    }
}
