namespace RailwayTrainingDemo;
using System.ComponentModel;

public partial class FlashcardTopicPage : ContentPage
{
    public class Topic : INotifyPropertyChanged
    {
        private bool isSelected;

        public string Name { get; set; }
        public string Description { get; set; }
        public List<(string Term, string Definition)> Flashcards { get; set; }
        public int CardCount => Flashcards?.Count ?? 0;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    private readonly List<Topic> topics;

    public FlashcardTopicPage()
    {
        InitializeComponent();
        topics = CreateTopics();

        // Reset selection state when page is created
        foreach (var topic in topics)
        {
            topic.IsSelected = false;
        }

        TopicsCollection.ItemsSource = topics;
        TopicsCollection.SelectedItems.Clear();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Clear selections when returning to this page
        foreach (var topic in topics)
        {
            topic.IsSelected = false;
        }
        TopicsCollection.SelectedItems.Clear();
    }

    private void OnTopicTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Topic topic)
        {
            topic.IsSelected = !topic.IsSelected;

            // Ensure CollectionView selection matches the topic state
            if (topic.IsSelected && !TopicsCollection.SelectedItems.Contains(topic))
            {
                TopicsCollection.SelectedItems.Add(topic);
            }
            else if (!topic.IsSelected && TopicsCollection.SelectedItems.Contains(topic))
            {
                TopicsCollection.SelectedItems.Remove(topic);
            }
        }
    }

    private async void OnStartStudyClicked(object sender, EventArgs e)
    {
        var selectedTopics = TopicsCollection.SelectedItems.Cast<Topic>().ToList();

        if (!selectedTopics.Any())
        {
            await DisplayAlert("Warning", "Please select at least one topic", "OK");
            return;
        }

        // Combine flashcards from all selected topics and remove duplicates
        var combinedFlashcards = selectedTopics
            .SelectMany(t => t.Flashcards)
            .Distinct()
            .ToList();

        // Shuffle the combined flashcards
        Random rng = new Random();
        combinedFlashcards = combinedFlashcards
            .OrderBy(x => rng.Next())
            .ToList();

        var navigationParameter = new Dictionary<string, object>
        {
            { "flashcards", combinedFlashcards }
        };

        await Shell.Current.GoToAsync(nameof(FlashcardPage), navigationParameter);
    }

    private List<Topic> CreateTopics()
    {
        return new List<Topic>
        {
            new Topic
            {
                Name = "Signaling Systems",
                Description = "Learn about railway signaling and control systems",
                Flashcards = new List<(string, string)>
                {
                    ("AWS", "Automatic Warning System - Provides audible and visual warnings to train drivers"),
                    ("TPWS", "Train Protection and Warning System - Automatically stops trains that pass signals at danger"),
                    ("Signal Aspects", "The different colored lights and positions used in railway signals"),
                    ("ETCS", "European Train Control System - Standardized train protection system across Europe"),
                    ("Block Signaling", "System that prevents more than one train from occupying a section of track"),
                    ("Route Setting", "Process of aligning points and signals for a train's journey"),
                }
            },
            new Topic
            {
                Name = "Safety Procedures",
                Description = "Essential safety protocols and procedures",
                Flashcards = new List<(string, string)>
                {
                    ("COSS", "Controller of Site Safety - Person responsible for work site safety"),
                    ("PPE", "Personal Protective Equipment - Required safety gear for railway workers"),
                    ("Emergency Procedures", "Steps to take in case of railway emergencies"),
                    ("Risk Assessment", "Systematic evaluation of potential hazards and their mitigation"),
                    ("Safe System of Work", "Planned method of working to eliminate or reduce risks"),
                    ("Track Access", "Procedures for safely accessing the railway infrastructure"),
                }
            },
            new Topic
            {
                Name = "Track Infrastructure",
                Description = "Understanding railway track components and maintenance",
                Flashcards = new List<(string, string)>
                {
                    ("Ballast", "Crushed stone that supports the track and provides drainage"),
                    ("Sleepers", "Transverse supports for rails, traditionally made of wood or concrete"),
                    ("Rail Gauge", "Standard distance between the inner faces of the rails"),
                    ("Points", "Mechanical installation enabling trains to be guided from one track to another"),
                    ("Track Circuit", "Electrical circuit using rails to detect train presence"),
                    ("Cant", "Difference in height between the two rails on a curved track"),
                }
            },
            new Topic
            {
                Name = "Train Operations",
                Description = "Learn about train movement and operational procedures",
                Flashcards = new List<(string, string)>
                {
                    ("Permissible Speed", "Maximum allowed speed for a section of track"),
                    ("Train Planning", "Process of scheduling and coordinating train movements"),
                    ("Coupling", "Process of connecting railway vehicles together"),
                    ("Shunting", "Moving rail vehicles other than normal passage of trains"),
                    ("Route Knowledge", "Driver's understanding of the routes they operate on"),
                    ("Train Protection", "Systems ensuring safe movement of trains"),
                }
            },
            new Topic
            {
                Name = "Communication",
                Description = "Railway communication systems and protocols",
                Flashcards = new List<(string, string)>
                {
                    ("GSM-R", "Global System for Mobile Communications - Railway, used for train radio"),
                    ("Signal Post Telephone", "Trackside phone for communication with signallers"),
                    ("Emergency Call", "Priority communication in case of incidents"),
                    ("Hand Signals", "Visual communication method using flags or hand lamps"),
                    ("Radio Protocol", "Standard procedures for railway radio communication"),
                    ("Signal Box Communication", "Methods of communication between signal boxes"),
                }
            },
            new Topic
            {
                Name = "Emergency Response",
                Description = "Handling railway emergencies and incidents",
                Flashcards = new List<(string, string)>
                {
                    ("Emergency Stop", "Procedures for stopping trains in emergency situations"),
                    ("Protection Arrangements", "Methods of protecting staff and passengers during incidents"),
                    ("Evacuation Procedures", "Steps for safely evacuating trains and stations"),
                    ("First Aid", "Basic medical assistance procedures for railway environments"),
                    ("Fire Safety", "Fire prevention and response procedures"),
                    ("Incident Reporting", "Process of documenting and reporting railway incidents"),
                }
            },
            new Topic
            {
                Name = "Railway Regulations",
                Description = "Key rules and regulatory requirements",
                Flashcards = new List<(string, string)>
                {
                    ("Rule Book", "Official document containing operational rules and procedures"),
                    ("Sectional Appendix", "Local instructions and information for specific routes"),
                    ("Network License", "Authorization to operate on the railway network"),
                    ("Safety Certificate", "Document confirming compliance with safety requirements"),
                    ("Operating Restrictions", "Limitations on railway operations"),
                    ("Standards Compliance", "Adherence to railway industry standards"),
                }
            },
            new Topic
            {
                Name = "Environmental Safety",
                Description = "Environmental considerations in railway operations",
                Flashcards = new List<(string, string)>
                {
                    ("Noise Management", "Procedures for controlling railway noise pollution"),
                    ("Waste Disposal", "Proper handling of railway-related waste materials"),
                    ("Vegetation Control", "Management of trackside vegetation"),
                    ("Wildlife Protection", "Measures to protect wildlife near railway lines"),
                    ("Pollution Prevention", "Methods to prevent environmental contamination"),
                    ("Energy Efficiency", "Practices for reducing energy consumption in railway operations"),
                }
            }
        };
    }
}