using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections;
using System.Linq.Expressions;
using System.Diagnostics.Contracts;

namespace ReactiveUI
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class ReactiveValidatedObject : ReactiveObject, INotifyDataErrorInfo
    {
        [IgnoreDataMember]
        private readonly Dictionary<string, IList<Func<IObservedChange<object, object>, string>>> _validatedProperties = 
            new Dictionary<string, IList<Func<IObservedChange<object, object>, string>>>();

        [IgnoreDataMember]
        private readonly Dictionary<string, string> _validationErrors = new Dictionary<string, string>();

        #region Events

        /// <summary>
        /// Occurs when a validation error occurred.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Raises the <see cref="E:ErrorsChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="DataErrorsChangedEventArgs" /> instance containing the event data.</param>
        public virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
        {
            EventHandler<DataErrorsChangedEventArgs> handler = this.ErrorsChanged;

            if (handler != null)
            {
                handler(this, e);
            }

            notifyObservable(new ObservedChange<object, bool>() 
            {
                PropertyName = e.PropertyName,
                Sender = this,
                Value = _validationErrors.ContainsKey(e.PropertyName)
            }, this._validationObservable);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveValidatedObject" /> class.
        /// This class hook up to the Changing observable to receive property changing notifications.
        /// </summary>
        public ReactiveValidatedObject()
        {
            _validationObservable = new Subject<IObservedChange<object, bool>>();

            this.Changed.Subscribe(x =>
            {
                if (x.Sender != this)
                {
                    return;
                }
                this.CheckPropertyForValidationErrors(x);
            });
        }

        /// <summary>
        /// Gets a value that indicates whether the entity has validation errors.
        /// </summary>
        /// <returns>true if the entity currently has validation errors; otherwise, false.</returns>
        public bool HasErrors
        {
            get
            {
                return this._validationErrors.Any();
            }
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for; 
        /// or null or <see cref="F:System.String.Empty" />, to retrieve entity-level errors.</param>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return _validationErrors.Values;
            }
            else
            {
                string error;
                if (_validationErrors.TryGetValue(propertyName, out error))
                {
                    return new string[] { error };
                }
                return Enumerable.Empty<string>();
            }
        }

        [IgnoreDataMember]
        readonly Subject<IObservedChange<object, bool>> _validationObservable;

        [IgnoreDataMember]
        public IObservable<IObservedChange<object, bool>> ValidationObservable {
            get { return _validationObservable;  }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="change"></param>
        private void CheckPropertyForValidationErrors(IObservedChange<object, object> change)
        {
            string prevResult;
            _validationErrors.TryGetValue(change.PropertyName, out prevResult);

            this.Log().Debug("Checking {0:X}.{1}...", this.GetHashCode(), change.PropertyName);
            string result = getPropertyValidationError(change);
            this.Log().Debug("Validation result: {0}", result);

            if (result == null)
            {
                _validationErrors.Remove(change.PropertyName);
            }
            else
            {
                _validationErrors[change.PropertyName] = result;
            }

            if (result != prevResult)
            {
                this.OnErrorsChanged(new DataErrorsChangedEventArgs(change.PropertyName));
            }
        }

        /// <summary>
        /// Gets the property validation error.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="change">The property change.</param>
        /// <returns>An error message or null.</returns>
        private string getPropertyValidationError<TSender, TValue>(IObservedChange<TSender, TValue> change)
        {
            IList<Func<IObservedChange<object, object>, string>> list;

            lock (_validatedProperties) {
                if (!_validatedProperties.TryGetValue(change.PropertyName, out list)) {
                    return null;
                }
            }

            IList<Func<IObservedChange<TSender, TValue>, string>> validationFunctions = list as IList<Func<IObservedChange<TSender, TValue>, string>>;

            //make sure we have a value
            change.fillInValue();

            foreach (var v in validationFunctions)
            {
                string result = v(change);
                if (result != null)
                {
                    this.Log().Info("{0:X}.{1} failed validation: {2}", this.GetHashCode(), change.PropertyName, result);
                    return result;
                }
            }         
            
            return null;
        }

        /// <summary>
        /// Adds a validation rule for the specified property.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The expression containing the property name.</param>
        /// <param name="result">The function to be called when a change occurs.</param>
        public void Validate<TSender, TProperty>(string propertyName, Func<IObservedChange<TSender, TProperty>,string> result)
        {
            Contract.Requires(propertyName != null);
            Contract.Requires(result != null);

            IList<Func<IObservedChange<object, object>, string>> validationFunctions;
            if (_validatedProperties.TryGetValue(propertyName, out validationFunctions))
            {
                validationFunctions.Add((Func<IObservedChange<object, object>,string>)result);
            }
            else
            {
                validationFunctions = new List<Func<IObservedChange<object, object>, string>>();
                validationFunctions.Add((Func<IObservedChange<object, object>, string>)result);
                _validatedProperties[propertyName] = validationFunctions;
            }

            //initial check
            this.CheckPropertyForValidationErrors(new ObservedChange<object, object>()
            {
                PropertyName = propertyName,
                Sender = this,
                // value will be filled in later
                Value = null,
            });
        }        
    }
  
    public static class ReactiveValidatedObjectMixins
    {
        /// <summary>
        /// Adds a validation rule for the specified property.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The expression containing the property name.</param>
        /// <param name="result">The function to be called when a change occurs.</param>
        public static void Validate<TSender, TProperty>(this ReactiveValidatedObject This, 
            Expression<Func<TSender, TProperty>> property, 
            Func<IObservedChange<TSender, TProperty>, string> result)
        {
            This.Validate<TSender, TProperty>(Reflection.SimpleExpressionToPropertyName(property), result);
        }

        /// <summary>
        /// Adds a validation rule for the specified property.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The expression containing the property name.</param>
        /// <param name="result">The function to be called when a change occurs returning a bool.</param>
        /// <param name="errorMessage">The error message returned in case <paramref name="result"/> returns false.</param>
        public static void Validate<TSender, TProperty>(this ReactiveValidatedObject This, 
            Expression<Func<TSender, TProperty>> property, 
            Func<IObservedChange<TSender, TProperty>, bool> result, 
            string errorMessage)
        {
            This.Validate<TSender,TProperty>(Reflection.SimpleExpressionToPropertyName(property), ioc => result(ioc) ? null : errorMessage);
        }

        /// <summary>
        /// Adds a validation rule for the specified property.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="result">The function to be called when a change occurs returning a bool.</param>
        /// <param name="errorMessage">The error message returned in case <paramref name="result"/> returns false.</param>
        public static void Validate<TSender, TProperty>(this ReactiveValidatedObject This,
            string propertyName, 
            Func<IObservedChange<TSender, TProperty>, bool> result, 
            string errorMessage)
        {
            This.Validate<TSender, TProperty>(propertyName, ioc => result(ioc) ? null : errorMessage);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
