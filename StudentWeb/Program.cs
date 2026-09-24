using StudentManagement.Grpc;
using StudentWeb.AutoMapper;
using StudentWeb.Components;
using StudentWeb.Resilience;
using StudentWeb.Services.Implementation;
using StudentWeb.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAntDesign();

builder.Services.AddGrpcRpcRetry(builder.Configuration);

var grpcServerAddress = builder.Configuration["Grpc:ServerAddress"]
    ?? throw new InvalidOperationException("Grpc:ServerAddress is not configured.");

builder.Services.AddGrpcClient<StudentService.StudentServiceClient>(options =>
{
    options.Address = new Uri(grpcServerAddress);
})
.AddGrpcTransientRetry();

builder.Services.AddGrpcClient<ClassService.ClassServiceClient>(options =>
{
    options.Address = new Uri(grpcServerAddress);
})
.AddGrpcTransientRetry();

builder.Services.AddGrpcClient<AnalyticsService.AnalyticsServiceClient>(options =>
{
    options.Address = new Uri(grpcServerAddress);
})
.AddGrpcTransientRetry();

builder.Services.AddScoped<IStudentService, GrpcStudentService>();
builder.Services.AddScoped<IClassService, GrpcClassService>();
builder.Services.AddScoped<IAnalyticsService, GrpcAnalyticsService>();
builder.Services.AddLogging();
builder.Services.AddAutoMapper(
cfg => { },
typeof(StudentMappingProfile));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
