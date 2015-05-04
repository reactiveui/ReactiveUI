# IBindingTypeConverter

## chatlogs

    ghuntley [11:48 AM] 
    @paulcbetts: can I get an explanation as to what ``GetAffinityForObjects`` is and how it works for the documentation? https://github.com/reactiveui/ReactiveUI/blob/master/ReactiveUI/BindingTypeConverters.cs#L91
    
    ghuntley [11:49 AM]
    https://github.com/ghuntley/ReactiveUI/blob/readthedocs/docs/sources/binding/type-converters.md
    
        
    paulcbetts [11:50 AM] 
    Return 0 if you can't convert an object and return like 50 if you can
    
    ghuntley [11:53 AM] 
    What happens if you return other than 0 (false)? Like 25 instead of 50?
    
    paulcbetts [11:54 AM] 
    Don't worry about that :)
    
    ghuntley [11:54 AM] 
    100 seems to be your favourite number https://github.com/reactiveui/ReactiveUI/blob/master/ReactiveUI/BindingTypeConverters.cs#L18
    
    ghuntley [12:10 PM] 
    thus my interpretation is
    
    ghuntley [12:10 PM]
    public int GetAffinityForObjects(Type fromType, Type toType)
           {
               if (fromType is string)
               {
                   return 100;
               }
               return 0;
           }
    
    paulcbetts [12:11 PM] 
    `fromType` is always `Type`
    
    ghuntley [12:16 PM] 
    thus (if fromtype = typeof(System.String))

    paulcbetts [12:21 PM] 
    :+1:


## Example - WIP

    using System;
    using ReactiveUI;
    using Conditions;
    using Splat;
    
    namespace MyCoolApp.Core.Converters
    {
        public class InverseStringIsNullEmptyOrWhitespaceToBoolTypeConverter : IBindingTypeConverter, IEnableLogger
        {
            public int GetAffinityForObjects(Type fromType, Type toType)
            {
                if (fromType == typeof(string))
                {
                    return 100; // any number other than 0 signifies conversion is possible.
                }
                return 0;
            }
    
            public bool TryConvert(object from, Type toType, object conversionHint, out object result)
            {
                Condition.Requires(from).IsNotNull();
                Condition.Requires(toType).IsNotNull();
                
                try
                {
                    result = !String.IsNullOrWhiteSpace(from);
                }
                catch (Exception ex)
                {
                    this.Log().WarnException("Couldn't convert object to type: " + toType, ex);
                    result = null;
                    return false;
                }
                
                return true;
            }
    
        }
    }

## Reference Material
* https://github.com/reactiveui/ReactiveUI/blob/master/ReactiveUI/PropertyBinding.cs#L50
* https://github.com/reactiveui/ReactiveUI/blob/master/ReactiveUI/Xaml/BindingTypeConverters.cs#L24

* http://stackoverflow.com/questions/23592231/how-do-i-register-an-ibindingtypeconverter-in-reactiveui
* https://github.com/reactiveui/ReactiveUI/commit/7fba662c7308db60cfda9e6fb3331b7cb514f14c

## Registration

    Locator.CurrentMutable.RegisterConstant(
        new MyCoolTypeConverter(), typeof(IBindingTypeConverter));

## Usage/Binding
    [redacted]: you never need to worry about specifying the converter in the binding. the GetAffinityForObjects is there for bindings to determine their priority when receiving certain types.
    [redacted]: let me see if i can understand what paul's magic rules for the result is - i'm pretty sure it's 0 - don't care, positive number - i can resolve this, and larger number wins
    [redacted]: eureka https://github.com/reactiveui/ReactiveUI/blob/238524a922aed50f8141a1d26ff24b8f2b101b60/ReactiveUI/RegisterableInterfaces.cs#L155-L165
    [@ghuntley]: ah that explains some things as to why the binding converter was firing on each view model load even when the binding was not wired onto a view.
    [@ghuntley]: let’s say however theres a InverseStringIsWhitespaceEmpyOrNullToBoolConverter and a StringIsWhitespaceEmptyToNullToBoolConverter. ie. String.IsNullEmptyOrWhitespace(x) and !String.IsNullEmptyOrWhitespace(x) both registered into splat. How can I be specific as to which one should be used.
    [@ghuntley]: like both will return a compatible affinity but it will be the wrong one depending on the UI case use.
    [@ghuntley]: "Hide this StackLayout w/IsVisible" if ViewModel.Property is null.
    "Show this StackLayout w/IsVisible" if ViewModel.Property is not null.
    [@ghuntley]: Yo
    [redacted]: morning!
    [redacted]: totally missed your recent question
    [@ghuntley]: morning indeed.
    [redacted]: i think, if you desire specific behaviour for a converter that logic should live in the binding itself
    [redacted]: the converters should be as dumb as possible
    [redacted]: does that make sense?
    [@ghuntley]: where I'm lost is concrete example as to provide hints to this.onewaybind(vm=>, v=>, ??, ??) which type to specifically use instead of doing autoresolution via affinity.
    [redacted]: the boolean to visibility converter is probably your best example - if you're on a platform where you can do Visibility.Collapsed, that'll be the default unless you pass in a conversion hint
    [@ghuntley]: Mvx has set.Bind(label).For(l => l.Text).To(vm => vm.FullName).WithConversion("AbbreviateIfLongerThan", 12L);
    [redacted]: but if you want to explicitly do Hidden, you need to opt in
    [redacted]: okay, so text trimming is an interesting example. in the apps that i've build, i always leave it up to the control to take care of that, rather than implementing it in the binding
    [@ghuntley]: good to know and interesting insight. Let me tailor above to exact outcome I'm after
    [redacted]: there's also the overloads on OneWayBind which let you  specify an Func<T,TOut> selector if you want to apply some application logic to your binding
    [@ghuntley]: set.Bind(view => view.MobilePhoneStackLayout)
       .For("IsVisibile")
       .To(vm => vm.MobilePhoneNumber (string))
       .WithConversion("InverseStringIsNullEmptyOrWhitespaceToBool");
    [redacted]: i'd recommend that over complicated converters
    [@ghuntley]: hide the entire sub-stacklayout if the viewmodel property is empty.
    [redacted]: hmmm
    [redacted]: okay, so you could totally do a String->Visibility converter - that won't clash with the defaults
    [@ghuntley]: RxUI has visibility converters?
    [redacted]: and you could use the conversion object to override the defaults, like this: https://github.com/reactiveui/ReactiveUI/blob/238524a922aed50f8141a1d26ff24b8f2b101b60/ReactiveUI/Xaml/BindingTypeConverters.cs#L36
    redacted]: yeah, they're just bool->Visibility
    [@ghuntley]: hmm but in this case I want to come from string and specify show|hide behavior on the binding so that I don't have to do a "IsMobilePhonePopulatedVisibile" (bool) on the viewmodel.
    [redacted]: lemme whip up a little sample
    [redacted]: i think it's totally doable, but perhaps i missed something
    [@ghuntley]: I'm the one probably missing something, from the last convo my thoughts have progressed - your saying my current implementation is on track but I should consider creating a behavior conversion enum and using that as conversion hint.
    [@ghuntley]: code would be wise. You have r/w access to https://github.com/ghuntley/ReactiveUI/blob/readthedocs/docs/sources/binding/type-converters.md fyi
    [redacted]: (Y)
    [@ghuntley]: alright penny has dropped moment. That's actually a pretty good API design.
    [@ghuntley]: :highfive: pretty sure I know the path forward now that the semantics of what/why ```public enum BooleanToVisibilityHint``` existed (even though it was right in front of me - didn't understand the why)
    [redacted]: no problem


    /// <summary>
    /// Returns a positive integer when this class supports
    /// TryConvert for this particular Type. If the method isn't supported at
    /// all, return a non-positive integer. When multiple implementations
    /// return a positive value, the host will use the one which returns
    /// the highest value. When in doubt, return '2' or '0'.
    /// </summary>
    /// <param name="fromType">The source type to convert from</param>
    /// <param name="toType">The target type to convert to</param>
    /// <returns>A positive integer if TryConvert is supported,
    /// zero or a negative value otherwise</returns>
    int GetAffinityForObjects(Type fromType, Type toType);


    I see you don't need to use a contract after all https://github.com/reactiveui/ReactiveUI/blob/9532a8df95bd4ed76f6fa9d9fd156edc0d973e98/ReactiveUI/PropertyBinding.cs#L1001 . `conversionHint` only needs to be set it if you use it in your `TryConvert` implementation
