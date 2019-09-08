using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

using HandyControl.Data;

namespace MovieList.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public bool HasErrors => this.ValidationErros.Count > 0;

        protected Dictionary<string, ICollection<string>> ValidationErros { get; } =
            new Dictionary<string, ICollection<string>>();

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable? GetErrors(string? propertyName)
            =>  this.ValidationErros.GetValueOrDefault(propertyName ?? String.Empty);

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (!String.IsNullOrEmpty(propertyName))
            {
                this.Validate(propertyName);
            }

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnErrorsChanged(string propertyName)
            => this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

        protected void Validate([CallerMemberName] string propertyName = "", object? value = null)
        {
            value ??= this.GetValue(propertyName);

            if (value == null || value.Equals(String.Empty))
            {
                value = null;
            }

            var results = new List<ValidationResult>();

            bool result = Validator.TryValidateProperty(
                value, new ValidationContext(this, null, null) { MemberName = propertyName }, results);

            this.ValidationErros.Remove(propertyName);

            if (!result)
            {
                this.ValidationErros.Add(propertyName, results.Select(r => r.ErrorMessage).ToList());
                this.OnErrorsChanged(propertyName);
            }
        }

        protected Func<string, OperationResult<bool>> Verify(string property)
            => value =>
            {
                this.Validate(property, value);
                return this.ValidationErros.ContainsKey(property) && this.ValidationErros[property].Count != 0
                    ? new OperationResult<bool>
                    {
                        Data = false,
                        Message = this.ValidationErros[property].First(),
                        ResultType = ResultType.Failed
                    }
                    : new OperationResult<bool> { Data = true, ResultType = ResultType.Success };
            };

        private object? GetValue(string propertyName)
            => this.GetType().GetProperty(propertyName)?.GetValue(this);
    }
}
