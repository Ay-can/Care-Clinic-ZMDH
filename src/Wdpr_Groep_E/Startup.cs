using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Hubs;
using Wdpr_Groep_E.Models;
using Wdpr_Groep_E.Services;

namespace Wdpr_Groep_E
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            // services.AddHttpClient<Client>();
            // services.AddHttpClient<ZmdhApi>();
            services.AddControllersWithViews().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

            // Local databases
            // services.AddDbContext<WdprContext>(options => options.UseSqlite("Data Source=demo.db"));
            services.AddDbContext<WdprContext>(options => options.UseSqlServer(Configuration.GetConnectionString("WdprContext")));

            // Live database
            // services.AddDbContext<WdprContext>(options => options.UseSqlServer("Data Source=SQL5109.site4now.net;Initial Catalog=db_a7f252_zmdh;User Id=db_a7f252_zmdh_admin;Password=Aycan123!;MultipleActiveResultSets=true"));

            services.AddRazorPages();
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<WdprContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI()
            .AddErrorDescriber<CustomIdentityErrorDescriber>();
            services.AddFluentEmail("zmdh.hulp@gmail.com")
                .AddSmtpSender(new SmtpClient("smtp.gmail.com")
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Port = 587,
                    Credentials = new NetworkCredential("zmdh.hulp@gmail.com", "Zmdh123!")
                });
            services.AddSingleton<IZmdhApi, ZmdhApi>();
            services.AddSingleton<IZorgdomein, Zorgdomein>();
            services.AddSignalR();


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
