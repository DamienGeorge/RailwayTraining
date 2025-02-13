namespace RailwayTrainingDemo;
public partial class MultipleChoice : BaseQuizPage
{
    public MultipleChoice() : base(10) // 10 questions for regular quiz
    {
        try
        {
            InitializeComponent();
            BindingContext = this;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in MultipleChoice constructor: {ex.Message}");
        }
    }

    protected override void ClearOptions()
    {
        if (OptionsStack != null)
        {
            OptionsStack.Clear();
        }
    }

    protected override void DisplayOptions(List<string> options)
    {
        if (options == null || !options.Any())
        {
            System.Diagnostics.Debug.WriteLine("Warning: No options provided to display");
            return;
        }

        if (OptionsStack == null)
        {
            System.Diagnostics.Debug.WriteLine("Warning: OptionsStack is null");
            return;
        }

        OptionsStack.Clear();
        base.currentRadioButtons?.Clear();
        
        foreach (var option in options)
        {
            if (string.IsNullOrEmpty(option)) continue;

            var radioButton = new RadioButton
            {
                Content = option,
                GroupName = "Options"
            };
            radioButton.CheckedChanged += OnAnswerSelected;
            OptionsStack.Add(radioButton);
            base.currentRadioButtons?.Add(radioButton);
        }
    }

    private void OnAnswerSelected(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value && sender is RadioButton radioButton && radioButton.Content != null)
        {
            base.selectedAnswer = radioButton.Content.ToString();
        }
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(base.selectedAnswer))
            {
                await DisplayAlert("Warning", "Please select an answer", "OK");
                return;
            }

            base.answers?.Add((base.QuestionText, base.selectedAnswer, base.correctAnswer));
            base.questionsAnswered++;

            if (base.questionsAnswered < base.totalQuestions)
            {
                base.LoadNextQuestion();
                base.selectedAnswer = null;
                base.ClearRadioButtons();
            }
            else
            {
                await base.NavigateToResults();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnSubmitClicked: {ex.Message}");
            await DisplayAlert("Error", "An error occurred. Please try again.", "OK");
        }
    }

    private async void OnReturnToMenuClicked(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("///home");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error in OnReturnToMenuClicked: {ex.Message}");
            await base.DisplayAlert("Error", "Unable to return to menu. Please try again.", "OK");
        }
    }
}