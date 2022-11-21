//using CompanyEmployees..ActionFilters;
using CompanyEmployees.Extensions;
using CompanyEmployees.Presentation_;
using CompanyEmployees.Presentation.ActionFilters;
//using CompanyEmployees.Presentation_.ActionFilters;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using NLog;
using Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(),
"/nlog.config"));
//NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
//new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson(_ => _.SerializerSettings.DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTimeOffset)
//  .Services.BuildServiceProvider()
//  .GetRequiredService<IOptions<MvcOptions>>()
//  .Value
//  .InputFormatters
//  .OfType<NewtonsoftJsonPatchInputFormatter>()
//  .First();

//NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson().Services.BuildServiceProvider().GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters.OfType<NewtonsoftJsonPatchInputFormatter>().First();



builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddScoped<ValidationFilterAttribute>();



//builder.Services.AddControllers();
builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
}).AddXmlDataContractSerializerFormatters()
  .AddCustomCSVFormatter()
  //.AddNewtonsoftJson()
  .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);




var app = builder.Build();

// Configure the HTTP request pipeline.
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);
if (app.Environment.IsProduction())
    app.UseHsts();

//if (app.Environment.IsDevelopment())
//    app.UseDeveloperExceptionPage();
//else
//    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
