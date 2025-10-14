namespace Peo.Web.Spa.Layout
{
    public partial class NavMenu
    {
        private bool _isAdmin = false;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            _isAdmin = user.IsInRole("Admin");
        }
    }
}
