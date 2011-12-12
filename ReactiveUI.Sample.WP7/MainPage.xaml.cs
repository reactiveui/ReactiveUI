using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using ReactiveUI.Sample.WP7.MicrosoftTranslatorService;
using ReactiveUI.Xaml;

namespace ReactiveUI.Sample.WP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        public TranslatorViewModel ViewModel { get; protected set; }

        // Constructor
        public MainPage()
        {
            ViewModel = new TranslatorViewModel();
            InitializeComponent();

            FromText.KeyUp += (o, e) => {
                ViewModel.EnglishText = FromText.Text;
            };
        }
    }

    public class TranslatorViewModel : ReactiveObject
    {
        public string _EnglishText;
        public string EnglishText {
            get { return _EnglishText; }
            set { this.RaiseAndSetIfChanged(x => x.EnglishText, value); }
        }

        public ObservableAsPropertyHelper<string> _TranslatedText;
        public string TranslatedText {
            get { return _TranslatedText.Value; }
        }

        public ReactiveAsyncCommand TranslateText { get; protected set; }

        public TranslatorViewModel()
        {
            TranslateText = new ReactiveAsyncCommand();

            this.ObservableForProperty(x => x.EnglishText)
                .Throttle(TimeSpan.FromMilliseconds(1200))
                .Subscribe(x => TranslateText.Execute(x.Value));

            var client = new LanguageServiceClient() as LanguageService;

            var translation_func = Observable.FromAsyncPattern<string, string>(
                client.BeginTranslateToGerman, client.EndTranslateToGerman);
            
            var results = TranslateText.RegisterAsyncObservable(x => translation_func((string)x));

            _TranslatedText = this.ObservableToProperty(results, x => x.TranslatedText);
        }
    }

    public static class LanguageServiceHelper
    {
        const string AppId = "8E94679989E7F3717D0B412E53BEAA73794369B2";

        public static IAsyncResult BeginTranslateToGerman(this LanguageService Client, string Text, AsyncCallback Callback, object Context)
        {
            return Client.BeginTranslate(new TranslateRequest(AppId, Text, "en", "de"), Callback, Context);
        }

        public static string EndTranslateToGerman(this LanguageService Client, IAsyncResult Result)
        {
            return Client.EndTranslate(Result).TranslateResult;
        }
    }

}
