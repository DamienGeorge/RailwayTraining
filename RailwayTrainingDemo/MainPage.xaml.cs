namespace RailwayTrainingDemo;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AppShell.SetBackgroundColor(this, Color.FromArgb("#582F0E"));
    }
    private async void OnMultipleChoiceClicked(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(MultipleChoice));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error in OnMultipleChoiceClicked: {ex.Message}");
            await DisplayAlert("Error", "Unable to start quiz. Please try again.", "OK");
        }
    }

    private async void OnFlashcardsClicked(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(FlashcardTopicPage));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error in OnFlashcardsClicked: {ex.Message}");
            await DisplayAlert("Error", "Unable to start flashcards. Please try again.", "OK");
        }
    }

    private async void OnProgressClicked(object sender, EventArgs e)
    {
        try
        {
            await DisplayAlert("Coming Soon", "Progress tracking will be available in a future update!", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnProgressClicked: {ex.Message}");
        }
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        try
        {
            await DisplayAlert("Coming Soon", "Settings will be available in a future update!", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnSettingsClicked: {ex.Message}");
        }
    }

    private async void OnMockTestsClicked(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(MockTestIntroPage));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error in OnMockTestsClicked: {ex.Message}");
            await DisplayAlert("Error", "Unable to start mock test. Please try again.", "OK");
        }
    }
} 