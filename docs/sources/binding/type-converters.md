# IBindingTypeConverter

## Example - WIP

    using System;
    using ReactiveUI;
    using Conditions;
    using Splat;
    
    namespace MyCoolApp.Core.Converters
    {
        public class InverseStringIsNullEmptyOrWhitespaceToBoolTypeConverter : IBindingTypeConverter, IEnableLogger
        {
            public int GetAffinityForObjects(System.Type fromType, System.Type toType)
            {
                throw new System.NotImplementedException();
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
