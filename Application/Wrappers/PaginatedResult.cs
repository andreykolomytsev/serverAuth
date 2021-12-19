using System;
using System.Collections.Generic;

namespace Application.Wrappers
{
    /// <summary>
    /// Обертка ответа контроллера для разбивки на страницы
    /// </summary>
    /// <typeparam name="T">Тип</typeparam>
    public class PaginatedResult<T> : Result
    {
        public PaginatedResult()
        {

        }

        public PaginatedResult(List<T> data)
        {
            Response = data;
        }

        public List<T> Response { get; set; }

        internal PaginatedResult(bool succeeded, List<T> data = default, List<string> messages = null, int count = 0, int page = 1, int pageSize = 10)
        {
            Succeeded = succeeded;
            Messages = messages;
            Response = data;
            CurrentPage = page < 1 ? 1 : page;        
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;         
        }

        public static PaginatedResult<T> Failure(List<string> messages)
        {
            return new PaginatedResult<T>(false, default, messages);
        }

        public static PaginatedResult<T> Success(List<T> data, int count, int page, int pageSize)
        {
            return new PaginatedResult<T>(true, data, null, count, page, pageSize);
        }

        /// <summary>
        /// Текущая страница
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Всего страниц
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Всего записей
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Элементов на странице
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Есть ли предыдущая страница
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// Есть ли следующая страница
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
