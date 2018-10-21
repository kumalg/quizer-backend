using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using quizer_backend.Data;

namespace quizer_backend.Controllers {

    [Produces("application/json")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    [Authorize]
    public class QuizerApiControllerBase : ControllerBase {
        protected IQuizerRepository _repository;

        public QuizerApiControllerBase(IQuizerRepository repository) {
            _repository = repository;
        }

        protected string UserId(ClaimsPrincipal User) => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

        protected ContentResult ToJsonContentResult(object item) {
            JsonSerializerSettings settings = new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var json = JsonConvert.SerializeObject(item, settings);
            return Content(json, "application/json");
        }
    }
}