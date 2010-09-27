using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using ReactiveXaml.Sample.WP7.MicrosoftTranslatorService;

namespace ReactiveXaml.Sample.WP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        public TranslatorViewModel ViewModel { get; protected set; }

        // Constructor
        public MainPage()
        {
            ViewModel = new TranslatorViewModel();
            InitializeComponent();
        }
    }

    public class TranslatorViewModel : ReactiveObject
    {
        string _EnglishText;
        public string EnglishText {
            get { return _EnglishText; }
            set { this.RaiseAndSetIfChanged(x => x.EnglishText, value); }
        }

        ObservableAsPropertyHelper<string> _TranslatedText;
        public string TranslatedText {
            get { return _TranslatedText.Value; }
        }

        public ReactiveAsyncCommand TranslateText { get; protected set; }

        const string AppId = "8E94679989E7F3717D0B412E53BEAA73794369B2";
        public TranslatorViewModel()
        {
            TranslateText = new ReactiveAsyncCommand(null, 1);

            this.ObservableForProperty(x => x.EnglishText)
                .CombineLatest(TranslateText.CanExecuteObservable, 
                    (text, enabled) => new { Text = text.Value, IsNotBusy = enabled})
                .Where(x => x.IsNotBusy)
                .Throttle(TimeSpan.FromMilliseconds(1200))
                .Subscribe(x => TranslateText.Execute(x.Text));

            var client = new LanguageServiceClient() as LanguageService;

            var translation_func = Observable.FromAsyncPattern<TranslateRequest, TranslateResponse>(
                client.BeginTranslate, client.EndTranslate);
            
            var results = TranslateText
                .RegisterObservableAsyncFunction(x => translation_func(new TranslateRequest(AppId, (string)x, "en", "de")))
                .Select(x => x.TranslateResult);

            _TranslatedText = this.ObservableToProperty(results, x => x.TranslatedText);
        }
    }
}