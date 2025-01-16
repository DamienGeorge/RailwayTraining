using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RailwayTrainingDemo.Models;

public class QuizAnswer : INotifyPropertyChanged
{
    private string question;
    private string userAnswer;
    private string correctAnswer;
    private bool isCorrect;

    public string Question
    {
        get => question;
        set
        {
            if (question != value)
            {
                question = value;
                OnPropertyChanged();
            }
        }
    }

    public string UserAnswer
    {
        get => userAnswer;
        set
        {
            if (userAnswer != value)
            {
                userAnswer = value;
                OnPropertyChanged();
            }
        }
    }

    public string CorrectAnswer
    {
        get => correctAnswer;
        set
        {
            if (correctAnswer != value)
            {
                correctAnswer = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsCorrect
    {
        get => isCorrect;
        set
        {
            if (isCorrect != value)
            {
                isCorrect = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AnswerColor));
                OnPropertyChanged(nameof(ResultIcon));
            }
        }
    }

    public Color AnswerColor => IsCorrect ? Colors.Green : Colors.Red;
    public string ResultIcon => IsCorrect ? "\uF012C" : "\uF015X"; // Checkmark or X icon

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 