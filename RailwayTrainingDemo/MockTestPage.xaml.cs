using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;

namespace RailwayTrainingDemo;

public partial class MockTestPage : BaseQuizPage
{
    private TimeSpan timeRemaining = TimeSpan.FromHours(1);
    private IDispatcherTimer? timer;
    private Color timerColor = Colors.Green;

    public string TimeRemaining => $"{timeRemaining:hh\\:mm\\:ss}";

    public Color TimerColor
    {
        get => timerColor;
        set
        {
            if (timerColor != value)
            {
                timerColor = value;
                OnPropertyChanged();
            }
        }
    }

    public MockTestPage() : base(20)
    {
        try
        {
            InitializeComponent();
            BindingContext = this;
            InitializeTimer();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in MockTestPage constructor: {ex.Message}");
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
                timer?.Stop();
                await base.NavigateToResults();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnSubmitClicked: {ex.Message}");
            await DisplayAlert("Error", "An error occurred. Please try again.", "OK");
        }
    }

    private void InitializeTimer()
    {
        try
        {
            if (Application.Current?.Dispatcher == null) return;

            timer = Application.Current.Dispatcher.CreateTimer();
            if (timer != null)
            {
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;
                timer.Start();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing timer: {ex.Message}");
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));
        OnPropertyChanged(nameof(TimeRemaining));

        if (timeRemaining.TotalMinutes <= 5)
        {
            TimerColor = Colors.Red;
        }
        else if (timeRemaining.TotalMinutes <= 15)
        {
            TimerColor = Colors.Orange;
        }

        if (timeRemaining <= TimeSpan.Zero)
        {
            timer?.Stop();
            _ = HandleTimeUp();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        timer?.Stop();
    }

    private async Task HandleTimeUp()
    {
        try
        {
            await DisplayAlert("Time's Up!", "Your time has expired. Let's review your results.", "OK");
            await base.NavigateToResults();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling time up: {ex.Message}");
            await DisplayAlert("Error", "There was a problem submitting your test. Please try again.", "OK");
        }
    }
} 