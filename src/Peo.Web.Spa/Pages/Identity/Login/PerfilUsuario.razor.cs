using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Peo.Web.Spa.Pages.Identity.Login
{
    public partial class PerfilUsuario : ComponentBase
    {
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public string UserName { get; private set; }
        public string Email { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            //var emailDireto = await _userManager.GetEmailAsync(user);
            

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                UserName = user.Identity.Name;
                Email = user.FindFirst(ClaimTypes.Email)?.Value;
            }
            else
            {
                UserName = "Guest";
                Email = string.Empty;
            }
        }
    }
}
