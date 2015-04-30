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
