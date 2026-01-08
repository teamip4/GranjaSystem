using GranjaSystemProject.Helpers;
using GranjaSystemProject.Models.Farm;
using GranjaSystemProject.Services;

namespace GranjaSystemProject.Views.Lots.Race;

public partial class RegisterRace : ContentPage
{
    private readonly RaceService _raceservice;
	public RegisterRace()
	{
		InitializeComponent();
        
        _raceservice = ServiceProviderHelper.GetService<RaceService>();
	}

	private async void OnSaveClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(NameRace.Text))
		{
            await DisplayAlert("Erro", "Por favor, preencha o campo nome.", "OK");
            return;
        }

		var newRace = new Models.Farm.Race
		{
			Name = NameRace.Text 
		};

		 var result = await _raceservice.RegisterRaceAsync(newRace);

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