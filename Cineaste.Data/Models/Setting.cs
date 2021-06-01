using System;

using Dapper.Contrib.Extensions;

namespace MovieList.Data.Models
{
    [Table("Settings")]
    internal sealed class Setting : EntityBase
    {
        public string Key { get; set; } = String.Empty;
        public string Value { get; set; } = String.Empty;

        public override string ToString() =>
            $"Setting #{this.Id}: {this.Key} - {this.Value}";
    }
}
