using Microsoft.EntityFrameworkCore;
using Voya.Data;
using Voya.Services.Common;
using Voya.Services.Auth; 
using Microsoft.AspNetCore.Identity;
using Voya.Models; // تأكد أن كلاس User و SnowflakeIdGenerator موجودان هنا

var builder = WebApplication.CreateBuilder(args);

// 1. إضافة الـ Controllers
builder.Services.AddControllersWithViews();

// 2. إعداد قاعدة البيانات SQL Server
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connStr));

// 3. تسجيل الخدمات مع حل تضارب الأسماء
builder.Services.AddScoped<ITokenService, TokenService>();

// هنا نستخدم الاسم الكامل Voya.Models.User لكي لا يختلط الأمر على الـ Compiler مع IdentityUser
builder.Services.AddScoped<IPasswordHasher<Voya.Models.User>, PasswordHasher<Voya.Models.User>>();

builder.Services.AddScoped<LoginService>();

// 4. تسجيل الـ Singleton
// ملاحظة: تأكد أن SnowflakeIdGenerator موجود في الـ Namespace الصحيح
builder.Services.AddSingleton<IIdGenerator>(new SnowflakeIdGenerator(machineId: 1));

var app = builder.Build();

// --- إعدادات الـ Pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();