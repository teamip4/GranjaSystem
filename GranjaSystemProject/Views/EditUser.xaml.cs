using GranjaSystemProject.Helpers;
using GranjaSystemProject.Services;

namespace GranjaSystemProject.Views;

public partial class EditUser : ContentPage
{
    private readonly AuthService _authservice;
    public EditUser()
    {
        InitializeComponent();

        _authservice = ServiceProviderHelper.GetService<AuthService>();

        Reload();
    }
    public void Reload()
    {
        var user = _authservice.CurrentUser;

        EntryName.Text = user.Name;
        EntryBirthDate.Date = user.BirthDate;
        EntryCpf.Text = user.Cpf;
        EntryState.Text = user.State;
        EntryCity.Text = user.City;
        EntryAddress.Text = user.Address;
        EntryPhone.Text = user.Phone;
    }
    public async void OnUpdateClicked(object sender, EventArgs e)
    {
        var user = _authservice.CurrentUser;

        user.Name = EntryName.Text;
        user.BirthDate = EntryBirthDate.Date;
        user.Cpf = EntryCpf.Text;
        user.State = EntryState.Text;
        user.City = EntryCity.Text;
        user.Address = EntryAddress.Text;
        user.Phone = EntryPhone.Text;

        var result = await _authservice.UpdateUserAsync(user);

        if (result.Success)
        {
            await DisplayAlert("Sucesso", "Perfil atualizado!", "OK");
            await Navigation.PopAsync();
        }
    }
}