using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Share
{
    public static class IntHelper
    {        
        public static int StartRow(int page, int maxrowperpage, int rowcount)
        {
            int endrow;
            return StartRow(page, maxrowperpage, rowcount, out endrow);
        }
        public static int StartRow(int page, int maxrowperpage, int rowcount, out int endrow)
        {
            int fixpage;
            return StartRow(page, maxrowperpage, rowcount, out endrow, out fixpage);
        }
        public static int StartRow(int page, int maxrowperpage, int rowcount, out int endrow, out int fixpage)
        {
            if (rowcount == 0 || maxrowperpage == 0)
            {
                endrow = -1;
                fixpage = 0;
                return -1;
            }

            int maxpage = (int)Math.Ceiling((double)rowcount / (double)maxrowperpage);
            if (page < 1)
                page = 1;
            if (page > maxpage)
                page = maxpage;
            int startrow = ((page - 1) * maxrowperpage) + 1;
            endrow = (page * maxrowperpage);

            fixpage = page;

            return startrow;
        }

        public static int Parse(string str, int fail)
        {
            int parsed;

            if (int.TryParse(str, out parsed))
                return parsed;
            else
                return fail;
        }
    }
}
