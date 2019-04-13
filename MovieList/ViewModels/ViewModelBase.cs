using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MovieList.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public bool HasErrors => this.ValidationErros.Count > 0;

        protected Dictionary<string, ICollection<string>> ValidationErros { get; set; } =
            new Dictionary<string, ICollection<string>>();

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
            =>  this.ValidationErros.GetValueOrDefault(propertyName ?? String.Empty);

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.Validate(propertyName);
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnErrorsChanged(string propertyName)
            => this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

        protected virtual void Validate([CallerMemberName] string propertyName = "")
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentOutOfRangeException(nameof(propertyName), "Property name cannot be empty.");
            }

            string error = String.Empty;
            var value = this.GetValue(propertyName);
            var results = new List<ValidationResult>();

            var result = Validator.TryValidateProperty(
                value, new ValidationContext(this, null, null) { MemberName = propertyName }, results);

            this.ValidationErros.Remove(propertyName);

            if (!result)
            {
                this.ValidationErros.Add(propertyName, results.Select(r => r.ErrorMessage).ToList());
                this.OnErrorsChanged(propertyName);
            }
        }

        private object GetValue(string propertyName)
        {
            var propInfo = this.GetType().GetProperty(propertyName);
            return propInfo.GetValue(this);
        }
    }
}
