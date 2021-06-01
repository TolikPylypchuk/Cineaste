using System;
using System.Windows.Markup;

using Cineaste.Properties;

namespace Cineaste.Localization
{
    [MarkupExtensionReturnType(typeof(string))]
    public sealed class TranslateExtension : MarkupExtension
    {
        public TranslateExtension(string key) =>
            this.Key = key;

        [ConstructorArgument("key")]
        public string Key { get; set; }

        public bool ToUpper { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            string result = Messages.ResourceManager.GetString(this.Key) ?? this.Key;
            return this.ToUpper ? result.ToUpper() : result;
        }
    }
}
