namespace RailwayTrainingDemo;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void StartBtn_Clicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new MainPage());
    }
}