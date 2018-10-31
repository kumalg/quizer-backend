using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data.Repository.Interfaces;

namespace quizer_backend.Controllers {

    [Produces("application/json")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    [Authorize]
    public class QuizerApiControllerBase : ControllerBase {

        protected readonly IQuizerRepository Repository;
        protected string UserId => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        protected long CurrentTime => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public QuizerApiControllerBase(IQuizerRepository repository) {
            Repository = repository;
        }
    }
}