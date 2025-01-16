namespace RailwayTrainingDemo;

[QueryProperty(nameof(CompletedFlashcards), "completedFlashcards")]
public partial class FlashcardCompletionPage : ContentPage
{
    private List<(string Term, string Definition)> flashcards;
    public string CompletionText { get; set; }

    public List<(string Term, string Definition)> CompletedFlashcards
    {
        set
        {
            flashcards = value;
            CompletionText = $"You've reviewed {flashcards.Count} flashcards!";
            OnPropertyChanged(nameof(CompletionText));
        }
    }

    public FlashcardCompletionPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private async void OnStudyAgainClicked(object sender, EventArgs e)
    {
        try
        {
            if (flashcards == null || !flashcards.Any())
            {
                await Shell.Current.GoToAsync("//MainPage");
                return;
            }

            // Shuffle the cards for a new session
            Random rng = new Random();
            var shuffledCards = flashcards
                .OrderBy(x => rng.Next())
                .ToList();

            var navigationParameter = new Dictionary<string, object>
            {
                { "flashcards", shuffledCards }
            };

            // Go back to FlashcardPage with shuffled cards
            await Shell.Current.GoToAsync($"../{nameof(FlashcardPage)}", navigationParameter);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error in OnStudyAgainClicked: {ex.Message}");
            await DisplayAlert("Error", "Unable to restart study session. Please try again.", "OK");
        }
    }

    private async void OnSelectTopicsClicked(object sender, EventArgs e)
    {
        try
        {
            // Navigate back to the topic page
            //await Shell.Current.GoToAsync(nameof(FlashcardTopicPage));
            await Shell.Current.GoToAsync($"../..");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error in OnSelectTopicsClicked: {ex.Message}");
            await DisplayAlert("Error", "Unable to return to topic selection. Please try again.", "OK");
        }
    }

    private async void OnReturnToMenuClicked(object sender, EventArgs e)
    {
        try
        {
           // Return to main menu
            await Shell.Current.GoToAsync(nameof(MainPage));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error in OnReturnToMenuClicked: {ex.Message}");
            await DisplayAlert("Error", "Unable to return to menu. Please try again.", "OK");
        }
    }
}