using GranjaSystemProject.Views.Farm;
using GranjaSystemProject.Views.Lots.Race;

namespace GranjaSystemProject.Views;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
	}
	public async void OnProfileClicked(Object sender, EventArgs e)
	{
        await Navigation.PushAsync(new ProfilePage());
    }
	public async void OnCreateFarm(Object sender, EventArgs e)
	{
		await Navigation.PushAsync(new CreateFarm());
	}
	public async void ViewRace(Object sender, EventArgs e)
	{
		await Navigation.PushAsync(new ViewRace());
	}
	public async void OnRegisterRace(Object sender, EventArgs e)
	{
		await Navigation.PushAsync(new RegisterRace());
	}
}
