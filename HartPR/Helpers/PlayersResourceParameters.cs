﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Helpers
{
    public class PlayersResourceParameters
    {
        const int maxPageSize = 2000;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public string State { get; set; }
        public string SearchQuery { get; set; }
        public string OrderBy { get; set; } = "Trueskill desc";
        public string Fields { get; set; }
    }
}
