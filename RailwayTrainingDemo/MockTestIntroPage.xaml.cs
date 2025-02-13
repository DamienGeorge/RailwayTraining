namespace RailwayTrainingDemo;

public partial class MockTestIntroPage : ContentPage
{
    public MockTestIntroPage()
    {
        InitializeComponent();
    }

    private async void OnStartTestClicked(object sender, EventArgs e)
    {
        try
        {
            bool answer = await DisplayAlert("Confirm Start", 
                "Are you ready to begin the mock test? The timer will start immediately.", 
                "Yes, Start Test", "No, Wait");
            
            if (answer)
            {
                await Shell.Current.GoToAsync(nameof(MockTestPage));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error in OnStartTestClicked: {ex.Message}");
            await DisplayAlert("Error", "Unable to start test. Please try again.", "OK");
        }
    }

    private async void OnReturnToMenuClicked(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error in OnReturnToMenuClicked: {ex.Message}");
            await DisplayAlert("Error", "Unable to return to menu. Please try again.", "OK");
        }
    }
} 