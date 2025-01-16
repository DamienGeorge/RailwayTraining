namespace RailwayTrainingDemo;

public partial class Menu : ContentPage
{
    public Menu()
    {
        InitializeComponent();
    }

    private async void FlashBtn_Clicked(object sender, EventArgs e)
    {
       await Navigation.PushAsync(new FlashcardTopicPage());
    }

    private async void MultipleChoiceBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MultipleChoice());
    }
}