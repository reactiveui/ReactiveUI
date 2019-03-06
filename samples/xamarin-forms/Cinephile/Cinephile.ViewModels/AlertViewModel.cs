namespace Cinephile.ViewModels
{
    public class AlertViewModel
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ButtonText { get; private set; }

        public AlertViewModel(string title, string description, string buttonText)
        {
            Title = title;
            Description = description;
            ButtonText = buttonText;
        }
    }
}
