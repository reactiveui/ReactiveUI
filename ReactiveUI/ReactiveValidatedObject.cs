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
        readonly Dictionary<string, object> _validatedProperties = new Dictionary<string, object>();

        [IgnoreDataMember]
        readonly Dictionary<string, string> _validationErrors = new Dictionary<string, string>();

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
                Value = _validationErrors[e.PropertyName] != null
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

                string prevResult;
                _validationErrors.TryGetValue(x.PropertyName, out prevResult);

                this.Log().Debug("Checking {0:X}.{1}...", this.GetHashCode(), x.PropertyName);
                string result = getPropertyValidationError(x);
                this.Log().Debug("Validation result: {0}", result);

                _validationErrors[x.PropertyName] = result;

                if (result != prevResult)
                {
                    this.OnErrorsChanged(new DataErrorsChangedEventArgs(x.PropertyName));
                }                
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
        /// Gets the property validation error.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="change">The property change.</param>
        /// <returns>An error message or null.</returns>
        string getPropertyValidationError<TSender, TValue>(IObservedChange<TSender, TValue> change)
        {
            object obj;

            lock (_validatedProperties) {
                if (!_validatedProperties.TryGetValue(change.PropertyName, out obj)) {
                    return null;
                }
            }

            IList<Func<IObservedChange<TSender, TValue>, string>> validationFunctions = obj as IList<Func<IObservedChange<TSender, TValue>, string>>;

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
        public void Validate<TSender, TProperty>(Expression<Func<TSender, TProperty>> property, Func<IObservedChange<TSender, TProperty>,string> result)
        {
            Contract.Requires(property != null);
            Contract.Requires(result != null);

            string propertyName = Reflection.SimpleExpressionToPropertyName(property);
            
            object obj;
            if (_validatedProperties.TryGetValue(propertyName, out obj))
            {
                IList<Func<IObservedChange<TSender, TProperty>, string>> validationFunctions = obj as IList<Func<IObservedChange<TSender, TProperty>, string>>;
                if (validationFunctions != null)
                {
                    validationFunctions.Add(result);
                }
            }
            else
            {
                IList<Func<IObservedChange<TSender, TProperty>, string>> validationFunctions = new List<Func<IObservedChange<TSender, TProperty>, string>>();
                validationFunctions.Add(result);
                _validatedProperties[propertyName] = validationFunctions;
            }
        }

        /// <summary>
        /// Adds a validation rule for the specified property.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The expression containing the property name.</param>
        /// <param name="result">The function to be called when a change occurs returning a bool.</param>
        /// <param name="errorMessage">The error message returned in case <paramref name="result"/> returns false.</param>
        public void Validate<TSender, TProperty>(Expression<Func<TSender, TProperty>> property, Func<IObservedChange<TSender, TProperty>, bool> result, string errorMessage)
        {
            Contract.Requires(result != null);

            this.Validate(property, ioc => result(ioc) ? null : errorMessage);
        }
    }   
}

// vim: tw=120 ts=4 sw=4 et :
