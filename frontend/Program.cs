global using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using frontend;
using Blazored.Modal;
using Blazored.LocalStorage;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// tähän kun vaihtaa osoitteen sen mukaan missä portissa lokaalisti ajetaan backendia / mikä on pilven osoite niin ei tarvii jokaisella pagella erikseen
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7106/") });
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("POISTETTU") }); // POISTETTU AZURE SQL CONNECTION
builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();



await builder.Build().RunAsync();

