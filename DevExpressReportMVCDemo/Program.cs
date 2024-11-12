using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.Extensions;

using DevExpressReportMVCDemo.Services;

using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Register reporting services in the application's dependency injection container.
builder.Services.AddDevExpressControls();
builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorage>();
// Use the AddMvc (or AddMvcCore) method to add MVC services.
builder.Services.AddMvcCore();
builder.Services.ConfigureReportingServices(configurator => {
    if (builder.Environment.IsDevelopment())
    {
        configurator.UseDevelopmentMode();
    }
    configurator.ConfigureReportDesigner(designerConfigurator => {
    });
    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        // Use cache for document generation and export.
        // This setting is necessary in asynchronous mode and when a report has interactive or drill down features.
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseDevExpressControls();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "node_modules")),
    RequestPath = "/node_modules"
});
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
