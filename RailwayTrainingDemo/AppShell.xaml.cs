namespace RailwayTrainingDemo;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes for navigation
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(FlashcardTopicPage), typeof(FlashcardTopicPage));
        Routing.RegisterRoute(nameof(FlashcardPage), typeof(FlashcardPage));
        Routing.RegisterRoute(nameof(FlashcardCompletionPage), typeof(FlashcardCompletionPage));
        Routing.RegisterRoute(nameof(MultipleChoice), typeof(MultipleChoice));
        Routing.RegisterRoute(nameof(QuizResultsPage), typeof(QuizResultsPage));
    }
}
