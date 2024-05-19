using ChatAppCore.DTOs;
using ChatAppShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppShared.Services.Interfaces
{
    public interface ITokenManager
    {
        public Task<bool> CheckTokenExperatibility();
        public Task StoreTokens(LoginResponseDTO data);
        public Task EmptyLocalStorage();

    }
}
