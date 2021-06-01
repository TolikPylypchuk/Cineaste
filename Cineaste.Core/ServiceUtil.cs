using System;

using Splat;

namespace Cineaste.Core
{
    public static class ServiceUtil
    {
        public static T GetDefaultService<T>(string? contract = null) =>
            Locator.Current.GetService<T>(contract) ??
                throw new InvalidOperationException($"{typeof(T).FullName} not found");
    }
}
