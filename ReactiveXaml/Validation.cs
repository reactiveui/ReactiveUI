using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;

namespace ReactiveXaml
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class ReactiveValidatedObject : ReactiveObject, IDataErrorInfo
    {
        /// <summary>
        ///
        /// </summary>
        public ReactiveValidatedObject()
        {
            //_IsValidObservable = new BehaviorSubject<bool>(this.IsValid());

            this.PropertyChanged += (o, e) => {
                if (errorMap.ContainsKey(e.PropertyName))
                    errorMap.Remove(e.PropertyName);
            };
        }

        static MemoizingMRUCache<Tuple<Type, string>, IEnumerable<ValidationAttribute>> validationAttributeMap 
            = new MemoizingMRUCache<Tuple<Type, string>, IEnumerable<ValidationAttribute>>((prop, _) => (
                RxApp.getPropertyInfoForProperty(prop.Item1, prop.Item2)
                    .GetCustomAttributes(typeof(ValidationAttribute), true)
                    .Cast<ValidationAttribute>()
            ), 25);        

        readonly Dictionary<string, string> errorMap = new Dictionary<string, string>();
        
        /// <summary>
        /// 
        /// </summary>
        [IgnoreDataMember]
        public string Error {
            get { return null; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        [IgnoreDataMember]
        public string this[string propName] {
            get {
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

        /// <summary>
        /// 
        /// </summary>
        [IgnoreDataMember]
        public IObservable<bool> IsValidObservable {
            get { return _IsValidObservable; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
                    this.Log().InfoFormat("{0:X}.{1} failed validation: {2}", 
                        this.GetHashCode(), propName, ve.Message);
                    return ve.Message;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class ValidationBase : ValidationAttribute
    {
        public bool AllowNull = false;
        public bool AllowBlanks = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var ret = base.IsValid(value, validationContext);
            if (ret == null || ret.ErrorMessage == null)
                return null;
            return getStandardMessage(validationContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool isValidViaNullOrBlank(object value)
        {
            if (value == null && !AllowNull)
                return false;

            string s = value as string;
            return !(s != null && !AllowBlanks && String.IsNullOrWhiteSpace(s));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        protected ValidationResult isValidViaNullOrBlank(object value, ValidationContext ctx)
        {
            if (isValidViaNullOrBlank(value))
                return null;

            return new ValidationResult(String.Format("{0} is blank",
                ctx.DisplayName ?? "The value"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        protected virtual ValidationResult getStandardMessage(ValidationContext ctx)
        {
            return new ValidationResult(ErrorMessage ??
                String.Format("{0} is incorrect", ctx.DisplayName ?? "The value"));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ValidatesViaMethodAttribute : ValidationBase
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

// vim: tw=120 ts=4 sw=4 et :