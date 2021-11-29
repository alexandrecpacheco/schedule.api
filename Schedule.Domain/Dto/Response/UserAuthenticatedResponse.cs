using System;
using System.Collections.Generic;
using System.Text;

namespace Schedule.Domain.Dto.Response
{
    public class UserAuthenticatedResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
