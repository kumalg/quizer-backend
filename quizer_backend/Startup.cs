using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using quizer_backend.Data;
using quizer_backend.Data.Repository;
using quizer_backend.Data.Services;
using quizer_backend.Services;

namespace quizer_backend {
    public class Startup {
        private readonly IConfiguration _config;

        public Startup(IConfiguration configuration) {
            _config = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            string domain = $"https://{_config["Auth0:Domain"]}/";
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.Authority = domain;
                options.Audience = _config["Auth0:ApiIdentifier"];
                options.Events = new JwtBearerEvents {
                    OnTokenValidated = context => {
                        // Grab the raw value of the token, and store it as a claim so we can retrieve it again later in the request pipeline
                        // Have a look at the ValuesController.UserInformation() method to see how to retrieve it and use it to retrieve the
                        // user's information from the /userinfo endpoint
                        if (context.SecurityToken is JwtSecurityToken token) {
                            if (context.Principal.Identity is ClaimsIdentity identity) {
                                identity.AddClaim(new Claim("access_token", token.RawData));
                            }
                        }

                        return Task.FromResult(0);
                    }
                };
            });

            services.AddCors(options => {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                );
            });
            services.AddDbContext<QuizerContext>(opt => {
                opt.UseSqlServer(_config.GetConnectionString("QuizerConnectionString"));
            });

            services.AddScoped<AnswersService, AnswersService>();
            services.AddScoped<LearningQuizzesService, LearningQuizzesService>();
            services.AddScoped<QuestionsService, QuestionsService>();
            services.AddScoped<QuizzesService, QuizzesService>();
            services.AddScoped<SolvingQuizzesService, SolvingQuizzesService>();
            services.AddScoped<UsersService, UsersService>();

            services.AddSingleton(new Auth0ManagementFactory(_config));
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => {
                     options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                 });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
