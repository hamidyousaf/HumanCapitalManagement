using Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

#region Registering Services.
builder.Services.Register(builder.Configuration);
#endregion

var app = builder.Build();

#region Registering Apps.
app.Use();
#endregion