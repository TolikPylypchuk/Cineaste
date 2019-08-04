using System;
using System.IO;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MovieList.Options
{
    public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
    {
        private readonly IOptionsMonitor<T> options;
        private readonly string section;
        private readonly string file;

        public WritableOptions(IOptionsMonitor<T> options, string section, string file)
        {
            this.options = options;
            this.section = section;
            this.file = file;
        }

        public T Value => options.CurrentValue;

        public T Get(string name) => options.Get(name);

        public void Update(Action<T> applyChanges)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(this.file));
            var sectionObject = jObject.TryGetValue(this.section, out var sectionToken)
                ? JsonConvert.DeserializeObject<T>(sectionToken.ToString())
                : (this.Value ?? new T());

            applyChanges(sectionObject);

            jObject[this.section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
            File.WriteAllText(this.file, JsonConvert.SerializeObject(jObject, Formatting.Indented));
        }
    }
}
