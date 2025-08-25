using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Peo.Web.Spa.Pages.Identity.Login
{
    public partial class PerfilUsuario : ComponentBase
    {
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

        public string UserName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
           if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                UserName = user.Identity.Name!;
                Email = user.Identity.Name!;

            }
            else
            {
                UserName = "Guest";
                Email = string.Empty;
            }
        }
    }
}
