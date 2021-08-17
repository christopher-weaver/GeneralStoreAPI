using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models.Request_Parameters
{
    public class CustomerParameters
    {
        const int maxPageSize = 20;
        public int _pageSize { get; set; } = 10;

        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }

            set
            {
                _pageSize = (maxPageSize <= value) ? maxPageSize : value;
            }
        }

    }
}