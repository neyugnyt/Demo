using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Pagination
{
    public class SearchPaginationCategoryDTO<T>
    {
        public T Search { get; set; }
        public int PageIndex
        {
            get; set;
        }
        public int PageSize { get; set; } = 50;

    }
}
