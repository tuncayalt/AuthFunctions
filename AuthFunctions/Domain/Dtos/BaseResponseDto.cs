using System.Collections.Generic;

namespace AuthFunctions.Domain.Dtos
{
    public class BaseResponseDto
    {
        public IList<ErrorResponseDto> Errors { get; set; }
    }
}
