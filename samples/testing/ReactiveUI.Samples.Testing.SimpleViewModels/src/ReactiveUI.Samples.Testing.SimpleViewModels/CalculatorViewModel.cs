using System.Reactive.Linq;

namespace ReactiveUI.Samples.Testing.SimpleViewModels
{
    /// <summary>
    /// A view model with some straight forward calculations. There are no
    /// async operations involve, nor are there any delays or other time related
    /// calls.
    /// Operation: The user enters text into the input text field.
    /// If the text doesn't contain numbers, then an error message is shown in the ErrorText field
    /// Otherwise the number x2 is shown in the ResultText field.
    /// The ErrorText and ResultText fields should be empty when nothing is in InputText.
    /// </summary>
    public class CalculatorViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<string> _ErrorText;
        private readonly ObservableAsPropertyHelper<string> _ResultText;

        public string InputText
        {
            get => _InputText;
            set => this.RaiseAndSetIfChanged(ref _InputText, value);
        }
        string _InputText;

        public string ErrorText => _ErrorText.Value;

        public string ResultText => _ResultText.Value;

        public CalculatorViewModel()
        {
            var haveInput = this.WhenAny(x => x.InputText, x => x.Value)
                .Where(x => !string.IsNullOrEmpty(x));

            // Convert into a stream of parsed integers, or null if we fail.
            var parsedIntegers = haveInput
                .Select(x => int.TryParse(x, out var val) ? (int?)val : null);

            // Now, the error text
            parsedIntegers
                .Select(x => x.HasValue ? "" : "Error")
                .ToProperty(this, x => x.ErrorText, out _ErrorText);

            // And the result, which is *2 of the input.
            parsedIntegers
                .Select(x => x.HasValue ? (x.Value * 2).ToString() : "")
                .ToProperty(this, x => x.ResultText, out _ResultText);
        }
    }
}
