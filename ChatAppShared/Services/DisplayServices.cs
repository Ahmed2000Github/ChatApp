using ChatAppCore.DTOs;
using ChatAppShared.Models;
using ChatAppShared.Services.Interfaces;
using System.Net.Http.Json;

namespace ChatAppShared.Services
{
    public class DisplayServices : IDisplayServices
    {
        public DisplayState DisplayState { get; set; } = DisplayState.BothState;

        public event Action? OnChange;

        public void Notify(DisplayState displayState)
        {
            DisplayState = displayState;
            OnChange?.Invoke();
        }
    }
}
