﻿using System;

namespace Bs.Calendar.Rules
{
    public class PageCounter
    {
        public static int GetTotalPages(int count, int pageSize) 
        {
            return (int)Math.Ceiling((decimal)count / pageSize);
        }

        public static int GetRangedPage(int page, int totalPages)
        {
            return page <= 1 ? 1 : page > totalPages ? totalPages : page;
        }
    }
}
