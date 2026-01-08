using GranjaSystemProject.Models.User;
using GranjaSystemProject.Helpers;
using GranjaSystemProject.Services;

namespace GranjaSystemProject.Views;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authservice;
    public RegisterPage()
	{
		InitializeComponent();
        _authservice = ServiceProviderHelper.GetService<AuthService>();
	}

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Name.Text) || string.IsNullOrWhiteSpace(Email.Text) || string.IsNullOrWhiteSpace(Password.Text))
        {
            await DisplayAlert("Erro", "Por favor, preencha os campos obrigatórios.", "OK");
            return;
        }

        if (Password.Text != PasswordConfirm.Text)
        {
            await DisplayAlert("Erro", "As senhas não coincidem.", "OK");
            return;
        }

        var newUser = new User
        {
            Name = Name.Text,
            Email = Email.Text,
            PasswordHash = Password.Text, 
            BirthDate = BirthDate.Date,
            Cpf = Cpf.Text,
            State = State.Text,
            City = City.Text,
            Address = Address.Text,
            Phone = Phone.Text,
        };

        var result = await _authservice.RegisterUserAsync(newUser);

        if (result.Success)
        {
            await DisplayAlert("Sucesso", result.Message, "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Erro", result.Message, "OK");
        }
    }
}
