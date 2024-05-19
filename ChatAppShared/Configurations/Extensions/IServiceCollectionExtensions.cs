using Blazored.LocalStorage;
using ChatAppShared.Services;
using ChatAppShared.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;

namespace ChatAppShared.Configurations.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddSharedServices(this IServiceCollection services)
        {

            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 5000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });

            services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(AppConfig.BASE_URL)
            });
            services.AddMudServices();
            services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            services.AddScoped<IContactsServices, ContactsServices>();
            services.AddScoped<ICurrentContactServices, CurrentContactServices>();
            services.AddScoped<IChatNotificationServices, ChatNotificationServices>();
            services.AddScoped<IDisplayServices, DisplayServices>();
            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<LayoutService>();
            services.AddScoped<MudThemeProvider>();
            services.AddAuthorizationCore();
            services.AddBlazoredLocalStorage();
        }
    }
}
