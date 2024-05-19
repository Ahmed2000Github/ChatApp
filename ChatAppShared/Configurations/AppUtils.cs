using MudBlazor;

namespace ChatAppShared.Configurations
{
    public static class AppUtils
    {
        public static Color GetBadgeColor(bool isActive)
        {
            return isActive ? Color.Success : Color.Error;
        }
        public static string GetConnectionStatus(bool isActive)
        {
            return isActive ? "Online" : "Offline";
        }


    }
}
