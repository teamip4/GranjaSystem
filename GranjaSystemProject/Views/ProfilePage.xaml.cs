using GranjaSystemProject.Helpers;
using GranjaSystemProject.Services;

namespace GranjaSystemProject.Views;

public partial class ProfilePage : ContentPage
{
	private readonly AuthService _authservice;
	public ProfilePage()
	{
		InitializeComponent();

        _authservice = ServiceProviderHelper.GetService<AuthService>();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Reload();
    }
    public void Reload()
    {
        var user = _authservice.CurrentUser;

        if (user != null)
        {
            LabelName.Text = user.Name;
            LabelBirthDate.Text = user.BirthDate.ToString("dd/MM/yyyy");
            LabelEmail.Text = user.Email;
            LabelCpf.Text = user.Cpf;
            LabelState.Text = user.State;
            LabelCity.Text = user.City;
            LabelAddress.Text = user.Address;
            LabelPhone.Text = user.Phone;
        }
    }
    public async void UpdateUser(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditUser());
    }
}
