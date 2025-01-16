namespace RailwayTrainingDemo;

using RailwayTrainingDemo.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class MultipleChoice : ContentPage, INotifyPropertyChanged
{
    // Fields
    private List<(string Question, string CorrectAnswer, List<string> Options)> allQuestions;
    private List<(string Question, string UserAnswer, string CorrectAnswer)> answers;
    private string selectedAnswer;
    private int questionsAnswered;
    private int totalQuestions = 3;
    private string questionText;
    private string correctAnswer;
    private double progress;
    private string progressText;

    // Properties with INotifyPropertyChanged
    public string QuestionText
    {
        get => questionText;
        set
        {
            if (questionText != value)
            {
                questionText = value;
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

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Constructor
    public MultipleChoice()
    {
        InitializeComponent();
        BindingContext = this;
        answers = new List<(string, string, string)>();
        _ = InitializeQuiz();
    }

    // Initialize quiz
    private async Task InitializeQuiz()
    {
        try
        {
            allQuestions = LoadQuestionsFromCsv().ToList();
            await ResetQuiz();
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "Failed to load question: " + ex.Message, "OK");
        }
    }

    // Reset quiz state
    private async Task ResetQuiz()
    {
        try
        {
            // Reset state
            answers.Clear();
            questionsAnswered = 0;
            selectedAnswer = null;

            // Ensure we have questions
            if (allQuestions == null || !allQuestions.Any())
            {
                allQuestions = LoadQuestionsFromCsv().ToList();
            }

            if (allQuestions.Count < totalQuestions)
            {
                await DisplayAlert("Warning", "Not enough questions available. Quiz will be shorter.", "OK");
                totalQuestions = Math.Max(1, allQuestions.Count);
            }

            // Shuffle questions
            Random rng = new Random();
            allQuestions = allQuestions.OrderBy(x => rng.Next()).ToList();

            // Reset UI
            OptionsStack.Clear();
            QuestionText = string.Empty;
            Progress = 0;
            ProgressText = $"Question 1 of {totalQuestions}";

            LoadNextQuestion();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in ResetQuiz: {ex.Message}");
            await DisplayAlert("Error", "Failed to reset quiz. Please try again.", "OK");
        }
    }

    // Load next question
    private void LoadNextQuestion()
    {
        if (questionsAnswered >= totalQuestions || questionsAnswered >= allQuestions.Count)
        {
            return;
        }

        var currentQuestion = allQuestions[questionsAnswered];
        QuestionText = currentQuestion.Question;
        correctAnswer = currentQuestion.CorrectAnswer;

        // Shuffle options
        Random rng = new Random();
        var shuffledOptions = currentQuestion.Options.OrderBy(x => rng.Next()).ToList();

        // Update UI
        OptionsStack.Clear();
        foreach (var option in shuffledOptions)
        {
            var radioButton = new RadioButton
            {
                Content = option,
                GroupName = "Options"
            };
            radioButton.CheckedChanged += OnAnswerSelected;
            OptionsStack.Add(radioButton);
        }

        UpdateProgress();
    }

    // Event handlers
    private void OnAnswerSelected(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value && sender is RadioButton radioButton)
        {
            selectedAnswer = radioButton.Content?.ToString();
        }
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(selectedAnswer))
            {
                await DisplayAlert("Warning", "Please select an answer", "OK");
                return;
            }

            // Store answer
            answers.Add((QuestionText, selectedAnswer, correctAnswer));
            questionsAnswered++;

            if (questionsAnswered < totalQuestions)
            {
                LoadNextQuestion();
                selectedAnswer = null;
                
                // Clear radio button selection
                var radioButtons = this.GetVisualTreeDescendants().OfType<RadioButton>();
                foreach (var rb in radioButtons)
                {
                    rb.IsChecked = false;
                }
            }
            else
            {
                try
                {
                    // Convert answers for results page
                    var quizAnswers = answers.Select(a => new Models.QuizAnswer
                    {
                        Question = a.Question,
                        UserAnswer = a.UserAnswer,
                        CorrectAnswer = a.CorrectAnswer,
                        IsCorrect = a.UserAnswer == a.CorrectAnswer
                    }).ToList();

                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "answers", quizAnswers }
                    };

                    await Shell.Current.GoToAsync(nameof(QuizResultsPage), navigationParameter);
                }
                catch (Exception navEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Navigation error: {navEx.Message}");
                    await DisplayAlert("Error", "Unable to show results. Please try again.", "OK");
                }
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
            await DisplayAlert("Error", "Unable to return to menu. Please try again.", "OK");
        }
    }

    private void UpdateProgress()
    {
        Progress = (double)questionsAnswered / totalQuestions;
        ProgressText = $"Question {questionsAnswered + 1} of {totalQuestions}";
    }

    private IEnumerable<(string Question, string CorrectAnswer, List<string> Options)> LoadQuestionsFromCsv()
    {
        var questions = new List<(string Question, string CorrectAnswer, List<string> Options)>();
        
        try
        {
            string targetPath = Path.Combine(FileSystem.AppDataDirectory, "questions.csv");

            // If file doesn't exist in app data, copy from embedded resource
            if (!File.Exists(targetPath))
            {
                using Stream sourceStream = GetType().Assembly
                    .GetManifestResourceStream("RailwayTrainingDemo.questions.csv");
                if (sourceStream == null)
                {
                    throw new FileNotFoundException("Embedded questions file not found");
                }
                using FileStream targetStream = File.Create(targetPath);
                sourceStream.CopyTo(targetStream);
            }

            // Read questions from the file
            foreach (string line in File.ReadAllLines(targetPath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = line.Split(',');
                if (values.Length < 3) continue; // Need at least question, correct answer, and one option

                var question = values[0].Trim();
                var correctAnswer = values[1].Trim();
                var options = values.Skip(2) // Exclude correct answer in options
                                  .Select(o => o.Trim())
                                  .Where(o => !string.IsNullOrWhiteSpace(o))
                                  .ToList();

                if (options.Count >= 2) // Ensure at least 2 options
                {
                    questions.Add((question, correctAnswer, options));
                }
            }

            if (!questions.Any())
            {
                throw new InvalidOperationException("No valid questions found in CSV file");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading questions: {ex.Message}");
            
            // Return default questions if file loading fails
            return new List<(string, string, List<string>)>
            {
                ("What is AWS?", 
                 "Automatic Warning System", 
                 new List<string> { "Automatic Warning System", "Advanced Warning Signal", "Automated Work Schedule", "Alert Warning System" }),
                
                ("What does TPWS stand for?",
                 "Train Protection and Warning System",
                 new List<string> { "Train Protection and Warning System", "Track Position Warning System", "Train Position Warning Signal", "Track Protection Warning System" }),
                
                ("What is PPE?",
                 "Personal Protective Equipment",
                 new List<string> { "Personal Protective Equipment", "Personal Protection Enforcement", "Protective Personal Equipment", "Personnel Protection Equipment" }),
                 
                ("What is COSS?",
                 "Controller of Site Safety",
                 new List<string> { "Controller of Site Safety", "Coordinator of Safety Systems", "Control of Safety Standards", "Chief of Safety Services" }),
                 
                ("What color is a stop signal?",
                 "Red",
                 new List<string> { "Red", "Green", "Yellow", "White" })
            };
        }

        return questions;
    }
}