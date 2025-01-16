namespace RailwayTrainingDemo;

[QueryProperty(nameof(Flashcards), "flashcards")]
public partial class FlashcardPage : ContentPage
{
    private List<(string Term, string Definition)> flashcards;
    private int currentIndex = 0;
    private bool isShowingTerm = true;

    private string currentCardText;
    private double progress;
    private string progressText;
    private bool canGoBack;
    private bool canGoForward;

    public List<(string Term, string Definition)> Flashcards
    {
        set
        {
            flashcards = value;
            UpdateCard();
        }
    }

    public string CurrentCardText
    {
        get => currentCardText;
        set
        {
            if (currentCardText != value)
            {
                currentCardText = value;
                OnPropertyChanged();
            }
        }
    }

    public double Progress
    {
        get => progress;
        set
        {
            if (progress != value)
            {
                progress = value;
                OnPropertyChanged();
            }
        }
    }

    public string ProgressText
    {
        get => progressText;
        set
        {
            if (progressText != value)
            {
                progressText = value;
                OnPropertyChanged();
            }
        }
    }

    public bool CanGoBack
    {
        get => canGoBack;
        set
        {
            if (canGoBack != value)
            {
                canGoBack = value;
                OnPropertyChanged();
            }
        }
    }

    public bool CanGoForward
    {
        get => canGoForward;
        set
        {
            if (canGoForward != value)
            {
                canGoForward = value;
                OnPropertyChanged();
            }
        }
    }

    public FlashcardPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private void UpdateCard()
    {
        if (flashcards == null || !flashcards.Any())
        {
            CurrentCardText = "No flashcards available";
            Progress = 0;
            ProgressText = "No cards loaded";
            CanGoBack = false;
            CanGoForward = false;
            return;
        }

        var currentCard = flashcards[currentIndex];
        CurrentCardText = isShowingTerm ? currentCard.Term : currentCard.Definition;
        Progress = (currentIndex + 1.0) / flashcards.Count;
        ProgressText = $"Card {currentIndex + 1} of {flashcards.Count}";
        
        CanGoBack = currentIndex > 0;
        CanGoForward = currentIndex < flashcards.Count;
    }

    private void OnCardTapped(object sender, TappedEventArgs e)
    {
        isShowingTerm = !isShowingTerm;
        UpdateCard();
    }

    private void OnPreviousClicked(object sender, EventArgs e)
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            isShowingTerm = true;
            UpdateCard();
        }
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        if (currentIndex < flashcards.Count - 1)
        {
            currentIndex++;
            isShowingTerm = true;
            UpdateCard();
        }
        else
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "completedFlashcards", flashcards }
            };
            await Shell.Current.GoToAsync(nameof(FlashcardCompletionPage), navigationParameter);
        }
    }
}