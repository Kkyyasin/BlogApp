using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Entity;

namespace BlogApp.ExternalServices.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateJwtToken(User user);
    }
}