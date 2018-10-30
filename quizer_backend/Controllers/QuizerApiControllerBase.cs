using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using quizer_backend.Data;
using quizer_backend.Data.Repository.Interfaces;

namespace quizer_backend.Controllers {

    [Produces("application/json")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    [Authorize]
    public class QuizerApiControllerBase : ControllerBase {
        protected readonly IQuizerRepository Repository;

        public QuizerApiControllerBase(IQuizerRepository repository) {
            Repository = repository;
        }

        protected string UserId(ClaimsPrincipal user) => user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        protected ContentResult ToJsonContentResult(object item) {
            var settings = new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var json = JsonConvert.SerializeObject(item, settings);
            return Content(json, "application/json");
        }
    }
}