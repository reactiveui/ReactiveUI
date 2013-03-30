using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Xaml
{
    public static class ReactiveValidatedObjectExtensions
    {
        /// <summary>
        /// Enables validation via <see cref="ValidationAttribute"/> attributes.
        /// </summary>
        /// <param name="This"></param>
        public static void ValidateViaAttributes(this ReactiveValidatedObject This)
        {
            //get all properties which have validation attributes
            var properties = This.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => Tuple.Create(p, p.GetCustomAttributes(typeof(ValidationAttribute), true).Cast<ValidationAttribute>() ?? new ValidationAttribute[0]))
                .Where(x => x.Item2.Any());

            //for each property add validation for every attribute
            foreach (var property in properties) {
                var propertyName = property.Item1.Name;
                var validationAttributes = property.Item2;

                This.Validate<object,object>(propertyName, ioc => {
                    foreach (var v in validationAttributes) {
                        try {
                            var ctx = new ValidationContext(This, null, null) { MemberName = propertyName };
                            v.Validate(ioc.Value, ctx);
                        } catch (Exception ex) {
                            This.Log().Info("{0:X}.{1} failed validation: {2}", This.GetHashCode(), propertyName, ex.Message);
                            return ex.Message;
                        }
                    }
                    return null;
                });
            }
        }
    }
}
