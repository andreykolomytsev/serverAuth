using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IResult
    {
        List<string> Messages { get; set; }

        bool Succeeded { get; set; }
    }

    public interface IResult<out T> : IResult
    {
        T Response { get; }
    }
}
