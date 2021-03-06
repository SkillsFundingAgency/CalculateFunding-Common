﻿namespace CalculateFunding.Common.ApiClient.Models
{
    using System;
    using CalculateFunding.Common.Utility;

    public class SearchPagedResult<T> : PagedResult<T>
    {
        public SearchPagedResult(SearchFilterRequest filterOptions, int totalCount, int totalErrorCount = 0)
        {
            Guard.ArgumentNotNull(filterOptions, nameof(filterOptions));

            TotalItems = totalCount;
            TotalErrorItems = totalErrorCount;
            PageNumber = filterOptions.Page;
            PageSize = filterOptions.PageSize;

            if (totalCount == 0)
            {
                TotalPages = 0;
            }
            else
            {
                TotalPages = (int)Math.Ceiling((decimal)totalCount / filterOptions.PageSize);
            }
        }
    }
}
