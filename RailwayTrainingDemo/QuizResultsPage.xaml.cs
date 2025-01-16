using RailwayTrainingDemo.Models;
using System.Collections.ObjectModel;

namespace RailwayTrainingDemo;

[QueryProperty(nameof(QuizAnswers), "answers")]
public partial class QuizResultsPage : ContentPage
{
    public ObservableCollection<QuizAnswer> Answers { get; } = new ObservableCollection<QuizAnswer>();
    public string ResultSummary { get; private set; }

    public List<QuizAnswer> QuizAnswers
    {
        set
        {
            Answers.Clear();
            foreach (var answer in value)
            {
                Answers.Add(answer);
            }
            
            int correctCount = value.Count(a => a.IsCorrect);
            double percentage = (double)correctCount / value.Count * 100;
            ResultSummary = $"You got {correctCount} out of {value.Count} correct! ({percentage:F1}%)";
            OnPropertyChanged(nameof(ResultSummary));
        }
    }

    public QuizResultsPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private async void OnNewQuizClicked(object sender, EventArgs e)
    {
        try
        {
            // Go back to quiz page which will reset with new questions
            await Shell.Current.GoToAsync(nameof(MultipleChoice));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error returning to MultipleChoice: {ex.Message}");
            await DisplayAlert("Error", "Unable to start new quiz. Please try again.", "OK");
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