using System;
using System.Reflection;

using MethodDecorator.Fody.Interfaces;

using MovieList.Data;

using Splat;

[module: LogException]

namespace MovieList.Data
{
    [AttributeUsage(
        AttributeTargets.Method |
        AttributeTargets.Constructor |
        AttributeTargets.Assembly |
        AttributeTargets.Module)]
    public sealed class LogExceptionAttribute : Attribute, IMethodDecorator
    {
        private Type? targetType;

        public void Init(object instance, MethodBase method, object[] args)
            => this.targetType = method.DeclaringType;

        public void OnEntry() { }

        public void OnExit() { }

        public void OnException(Exception exception)
        {
            var logManager = Locator.Current.GetService<ILogManager>();
            logManager.GetLogger(this.targetType).Error(exception);
        }
    }
}
