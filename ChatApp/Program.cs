

using ChatAppShared;
using ChatAppShared.Configurations.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<SharedApp>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSharedServices();

await builder.Build().RunAsync();
