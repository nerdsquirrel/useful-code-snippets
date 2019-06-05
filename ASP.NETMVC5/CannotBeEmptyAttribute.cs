using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class CannotBeEmptyAttribute : ValidationAttribute
    {
        private const string DEFAULT_ERROR = "'{0}' must have at least one element and elements cannot be empty.";
        public CannotBeEmptyAttribute() : base(DEFAULT_ERROR) //
        {
        }

        public override bool IsValid(object value)
        {
            var list = value as IEnumerable;
            if (list == null)
            {
                return false;
            }
            int count = 0;
            var sequence = list.GetEnumerator();

            while (sequence.MoveNext())
            {
                count++;
                var item = sequence.Current;
                if (item == null)
                {
                    return false;
                }

                if (item is string itemCast)
                {
                    if (string.IsNullOrEmpty(itemCast))
                    {
                        return false;
                    }
                }
            }   
            return count > 0;                                                       
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }
    }
}
