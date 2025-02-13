using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RailwayTrainingDemo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public abstract partial class BaseQuizPage : ContentPage, INotifyPropertyChanged
    {
        protected List<(string Question, string CorrectAnswer, List<string> Options)> allQuestions;
        protected List<(string Question, string UserAnswer, string CorrectAnswer)> answers;
        protected string selectedAnswer;
        protected int questionsAnswered = 0;
        protected int totalQuestions;
        protected string questionText;
        protected string correctAnswer;
        protected double progress;
        protected string progressText;
        protected List<RadioButton> currentRadioButtons;

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

        protected BaseQuizPage(int numberOfQuestions)
        {
            try
            {
                totalQuestions = numberOfQuestions;
                answers = new List<(string, string, string)>();
                currentRadioButtons = new List<RadioButton>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BaseQuizPage constructor: {ex.Message}");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                await InitializeQuiz();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnAppearing: {ex.Message}");
            }
        }

        protected async Task InitializeQuiz()
        {
            try
            {
                allQuestions = LoadQuestionsFromCsv().ToList();
                await ResetQuiz();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing quiz: {ex.Message}");
                await DisplayAlert("Error", "Failed to initialize quiz. Please try again.", "OK");
            }
        }

        protected async Task ResetQuiz()
        {
            try
            {
                answers.Clear();
                questionsAnswered = 0;
                selectedAnswer = null;

                if (allQuestions == null || !allQuestions.Any())
                {
                    allQuestions = LoadQuestionsFromCsv().ToList();
                }

                Random rng = new Random();
                allQuestions = allQuestions.OrderBy(x => rng.Next()).ToList();

                // Reset UI
                ClearOptions();
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

        protected abstract void ClearOptions();

        protected void LoadNextQuestion()
        {
            try
            {
                if (questionsAnswered >= totalQuestions || questionsAnswered >= allQuestions.Count)
                {
                    return;
                }

                var currentQuestion = allQuestions[questionsAnswered];
                QuestionText = currentQuestion.Question;
                correctAnswer = currentQuestion.CorrectAnswer;

                Random rng = new Random();
                var shuffledOptions = currentQuestion.Options.OrderBy(x => rng.Next()).ToList();

                DisplayOptions(shuffledOptions);
                UpdateProgress();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadNextQuestion: {ex.Message}");
                throw;
            }
        }

        protected abstract void DisplayOptions(List<string> options);

        protected void UpdateProgress()
        {
            Progress = (double)questionsAnswered / totalQuestions;
            ProgressText = $"Question {questionsAnswered + 1} of {totalQuestions}";
        }

        protected async Task NavigateToResults()
        {
            try
            {
                var quizAnswers = answers.Select(a => new Models.QuizAnswer
                {
                    Question = a.Question,
                    UserAnswer = a.UserAnswer,
                    CorrectAnswer = a.CorrectAnswer,
                    IsCorrect = a.UserAnswer.Equals(a.CorrectAnswer, StringComparison.OrdinalIgnoreCase)
                }).ToList();

                var navigationParameter = new Dictionary<string, object>
                {
                    { "answers", quizAnswers }
                };

                await Shell.Current.GoToAsync($"{nameof(QuizResultsPage)}", navigationParameter);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await DisplayAlert("Error", "Unable to show results. Please try again.", "OK");
            }
        }

        protected IEnumerable<(string Question, string CorrectAnswer, List<string> Options)> LoadQuestionsFromCsv()
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

        protected void ClearRadioButtons()
        {
            if (currentRadioButtons != null)
            {
                foreach (var rb in currentRadioButtons)
                {
                    rb.IsChecked = false;
                }
            }
        }
    }
}