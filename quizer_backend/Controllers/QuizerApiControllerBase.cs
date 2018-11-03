using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace quizer_backend.Controllers {

    [Produces("application/json")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    [Authorize]
    public class QuizerApiControllerBase : ControllerBase {
        
        protected string UserId => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        protected long CurrentTime => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    }
}