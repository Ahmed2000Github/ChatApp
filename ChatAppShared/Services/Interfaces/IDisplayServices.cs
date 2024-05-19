using ChatAppShared.Models;

namespace ChatAppShared.Services.Interfaces
{
    public interface IDisplayServices
    {
        public DisplayState DisplayState { get; set; }

        public event Action? OnChange;

        public void Notify(DisplayState displayState);

    }
}
