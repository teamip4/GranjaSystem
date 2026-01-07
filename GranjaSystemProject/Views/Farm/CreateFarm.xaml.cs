using GranjaSystemProject.Models.Farm;
using GranjaSystemProject.Services;

namespace GranjaSystemProject.Views.Farm;

public partial class CreateFarm : ContentPage
{
    private readonly FarmService _farmService;
    private readonly AuthService _authService;
	public CreateFarm(FarmService farmService, AuthService authService)
	{
		InitializeComponent();
        _farmService = farmService;
        _authService = authService;
	}
    private async void OnCreateFarm(object sender, EventArgs e)
    {
		if (string.IsNullOrWhiteSpace(FarmName.Text))
		{
            await DisplayAlert("Erro", "Por favor, preencha o nome da Granja.", "OK");
            return;
        }

        var newFarm = new Models.Farm.Farm
        {
            Name = FarmName.Text,
            OwnerId = _authService.CurrentUser.Id
        };

        var (success, message) = await _farmService.CreateFarmAsync(newFarm);

        if (success)
        {
            await DisplayAlert("Sucesso", "Granja cadastrada com sucesso!", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Erro", message, "OK");
        }
    }
}