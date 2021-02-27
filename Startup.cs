using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OSU_CS467_Software_Quiz.Data;
using OSU_CS467_Software_Quiz.Extensions;
using OSU_CS467_Software_Quiz.IdentityPolicy;
using OSU_CS467_Software_Quiz.Models;
using OSU_CS467_Software_Quiz.Services;
using OSU_CS467_Software_Quiz.Settings;
using System;

namespace OSU_CS467_Software_Quiz
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
      services.AddTransient<IMailService, MailService>();

      services.AddTransient<IPasswordValidator<AppUser>, AppPasswordPolicy>();

      // string sqlConfig = Environment.GetEnvironmentVariable("NPGSQL");
      // services.AddDbContext<AppDbContext>(options =>
      // {
      //   options.UseNpgsql(sqlConfig);
      // });
      //-----To tets locally
      services.AddDbContext<AppDbContext>(options => {
          options.UseNpgsql("Host=ec2-52-205-61-60.compute-1.amazonaws.com;Database=dbaeken4vohlpm;Username=jgkmoxhgmrhpqt;Password=e85e3226df24ea5a0f9a732a44fb4824aeb7288abfffc0ba0764a10b6f70a55d;Port=5432;sslmode=Prefer;Trust Server Certificate=true");
      });
      //-----To test locally
      services.AddIdentity<AppUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

      services.Configure<IdentityOptions>(options =>
      {
        options.User.RequireUniqueEmail = true;
      });

      // string googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
      // string googleClientSecret = Environment.GetEnvironmentVariable("GOOGLE_SECRET");
      // services.AddAuthentication()
      //   .AddGoogle(options =>
      //   {
      //     options.ClientId = googleClientId;
      //     options.ClientSecret = googleClientSecret;
      //     options.SignInScheme = IdentityConstants.ExternalScheme;
      //   });

      services.AddControllersWithViews();

      services.AddRepositories();

      // In production, the React files will be served from this directory
      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ClientApp/build";
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller}/{action=Index}/{id?}");
      });

      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment())
        {
          spa.UseReactDevelopmentServer(npmScript: "start");
        }
      });
    }
  }
}
