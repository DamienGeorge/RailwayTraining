namespace RailwayTrainingDemo;

public partial class Splash : ContentPage
{
    public Splash()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await Task.Delay(500); // Reduced from 2000ms to 500ms
        await Navigation.PushAsync(new MainPage());
        Navigation.RemovePage(this);
    }
}