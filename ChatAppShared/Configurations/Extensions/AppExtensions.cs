using ChatAppCore.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using System.Security.Claims;
using System.Text;

namespace ChatAppShared
{
    public static class AppExtensions
    {
        public static Color GetBadgeColor(this ConversationContactDTO info)
        {
            return info.IsActive ? Color.Success : Color.Error;
        }
        public static string GetConnectionStatus(this ConversationContactDTO info)
        {
            return info.IsActive ? "Online" : "Offline";
        }

        public static async Task WindowChangeListener(
            this IBreakpointService BreakpointListener,
            Action<Breakpoint> action
            )
        {
            action(await BreakpointListener.GetBreakpoint());
            await BreakpointListener.SubscribeAsync(async breakpoint =>
            {
                action(breakpoint);
            });

        }

        public static string ToMegabytes(this long bytes)
        {
            const double bytesInMegabyte = 1024.0 * 1024.0;
            double megabytes = bytes / bytesInMegabyte;
            return $"{megabytes:F2} MB"; 
        }
        public static string GetClaim(this ClaimsPrincipal user,string claimType)
        {
            return user.Claims.Where(c => c.Type == claimType).FirstOrDefault().Value.ToString();
        }

    }
}
