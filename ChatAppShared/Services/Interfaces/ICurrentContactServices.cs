using ChatAppCore.DTOs;
using ChatAppShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppShared.Services.Interfaces
{
    public interface ICurrentContactServices
    {
        public ConversationContactDTO? CurrentContact { get; set; }

        public event Action OnChange;

        public void Notify(ConversationContactDTO currentContact);

    }
}
