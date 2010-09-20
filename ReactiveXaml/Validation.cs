using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security;
using System.Runtime.Serialization;
using System.Diagnostics.Contracts;

namespace ReactiveXaml
{
    public class ReactiveValidatedObject : ReactiveObject, IDataErrorInfo
    {
        public ReactiveValidatedObject()
        {
            //_IsValidObservable = new BehaviorSubject<bool>(this.IsValid());

            this.PropertyChanged += (o, e) => {
                if (errorMap.ContainsKey(e.PropertyName))
                    errorMap.Remove(e.PropertyName);
            };
        }

        static MemoizingMRUCache<Tuple<Type, string>, IEnumerable<ValidationAttribute>> validationAttributeMap = new MemoizingMRUCache<Tuple<Type, string>, IEnumerable<ValidationAttribute>>((prop, _) => (
                prop.Item1.GetProperty(prop.Item2)
                    .GetCustomAttributes(typeof(ValidationAttribute), true)
                    .Cast<ValidationAttribute>()
            ), 25);        

        readonly Dictionary<string, string> errorMap = new Dictionary<string, string>();
        
        public string Error {
            get { return null; }
        }

        public string this[string propName] {
            get {
                Contract.Requires(propName != null);

                string ret = null;
                if(!errorMap.TryGetValue(propName, out ret)) {
                    ret = errorMap[propName] = calculatePropertyIsInvalid(propName);
                }

                // NB: This is null in the constructor :-/
                if (_IsValidObservable != null || false)
                    _IsValidObservable.OnNext(errorMap.All(x => x.Value == null));
                return ret;
            }
        }


        BehaviorSubject<bool> _IsValidObservable;
        [IgnoreDataMember]
        public IObservable<bool> IsValidObservable {
            get { return _IsValidObservable; }
        }

        public bool IsValid()
        {
            foreach (var prop in allPublicProperties.Value) { var dontcare = this[prop.Name]; }
            return errorMap.All(x => x.Value == null);
        }

        string calculatePropertyIsInvalid(string propName)
        {
            foreach(var v in validationAttributeMap.Get(new Tuple<Type,string>(GetType(), propName))) {
                // FIXME: This is slow and retarded
                try {
                    var ctx = new ValidationContext(this, null, null) { MemberName = propName };
                    v.Validate(GetType().GetProperty(propName).GetValue(this, null), ctx);
                } catch(Exception ve) {
                    return ve.Message;
                }
            }

            return null;
        }
    }

    public class ValidationBase : ValidationAttribute
    {
        public bool AllowNull = false;
        public bool AllowBlanks = true;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var ret = base.IsValid(value, validationContext);
            if (ret == null || ret.ErrorMessage == null)
                return null;
            return getStandardMessage(validationContext);
        }

        protected bool isValidViaNullOrBlank(object value)
        {
            if (value == null && !AllowNull)
                return false;

            string s = value as string;
            return !(s != null && !AllowBlanks && String.IsNullOrWhiteSpace(s));
        }

        protected ValidationResult isValidViaNullOrBlank(object value, ValidationContext ctx)
        {
            if (isValidViaNullOrBlank(value))
                return null;

            return new ValidationResult(String.Format("{0} is blank",
                ctx.DisplayName ?? "The value"));
        }

        protected virtual ValidationResult getStandardMessage(ValidationContext ctx)
        {
            return new ValidationResult(ErrorMessage ??
                String.Format("{0} is incorrect", ctx.DisplayName ?? "The value"));
        }
    }

    public class ValidatesViaMethod : ValidationBase
    {
        public string Name;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var is_blank = isValidViaNullOrBlank(value, validationContext);
            if (is_blank != null)
                return is_blank;

            string func = Name ?? String.Format("Is{0}Valid", validationContext.MemberName);
            var mi = validationContext.ObjectType.GetMethod(func, BindingFlags.Public | BindingFlags.Instance);
            bool result = (bool)mi.Invoke(validationContext.ObjectInstance, new[] { value });

            if (!result)
                throw new ValidationException(getStandardMessage(validationContext).ErrorMessage);

            return result ? null : getStandardMessage(validationContext);
        }
    }
}
