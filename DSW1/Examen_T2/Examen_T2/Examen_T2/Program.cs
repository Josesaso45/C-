using DSW1_T2_JOSE_MONTERO.Cliente.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<CursosApiService>((serviceProvider, httpClient) =>
{
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    string baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7195/";
    httpClient.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cursos}/{action=IndexCursos}/{id?}")
    .WithStaticAssets();

app.Run();
