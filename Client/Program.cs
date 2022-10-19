using IXAge_IHM.Client.Data;
using IXAge_IHM.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt/QHFqVVhkW1pFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF9jTH5VdkJjUHtccndWQw==;NjMxNzg5QDMyMzAyZTMxMmUzMEZUdEs0VnR5dWQ4NlJ2eUJsNTVtc1kxdEZmQ2dFT1lGamlPUEFURGgvdnc9");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSyncfusionBlazor();
            builder.Services.AddSingleton<PdfService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });
await builder.Build().RunAsync();
