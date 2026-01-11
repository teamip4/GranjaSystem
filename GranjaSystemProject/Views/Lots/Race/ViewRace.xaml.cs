using GranjaSystemProject.Helpers;
using GranjaSystemProject.Services;

namespace GranjaSystemProject.Views.Lots.Race;

public partial class ViewRace : ContentPage
{
    private readonly RaceService _raceService;

    public ViewRace()
    {
        InitializeComponent();
        _raceService = ServiceProviderHelper.GetService<RaceService>();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadRaces();
    }
    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadRaces();
        RefreshList.IsRefreshing = false;
    }
    private async Task LoadRaces()
    {
        var races = await _raceService.GetRacesAsync();
        RacesCollection.ItemsSource = races;
    }
}
