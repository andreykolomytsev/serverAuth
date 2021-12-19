using System.ComponentModel.DataAnnotations;

namespace Application.Parameters
{
    public class PaginationFilter
    {
        /// <summary>
        /// Номер страницы
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Кол-во элементов на странице
        /// </summary>
        public int PageSize { get; set; }

        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }

        public PaginationFilter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 100 ? 10 : pageSize;
        }
    }
}
