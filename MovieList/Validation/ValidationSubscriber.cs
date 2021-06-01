using System;
using System.Windows;

using Cineaste.Properties;

using ReactiveUI.Validation.Helpers;

namespace Cineaste.Validation
{
    public sealed class ValidationSubscriber : IDisposable
    {
        private IDisposable? mainSubscription;
        private IDisposable? eventSubscription;

        private ValidationSubscriber()
        { }

        public static ValidationSubscriber Create(FrameworkElement element, ValidationHelper rule)
        {
            var subscriber = new ValidationSubscriber();

            subscriber.mainSubscription = rule.ValidationChanged
                .Subscribe(state =>
                {
                    subscriber.eventSubscription?.Dispose();

                    if (!state.IsValid)
                    {
                        subscriber.eventSubscription = element.Events().LostFocus
                            .Subscribe(_ =>
                            {
                                ManualValidation.MarkInvalid(
                                    element,
                                    Messages.ResourceManager.GetString($"Validation{state.Text[0]}") ?? state.Text[0]);
                                subscriber.eventSubscription?.Dispose();
                            });
                    } else
                    {
                        ManualValidation.ClearValidation(element);
                    }
                });

            return subscriber;
        }

        public void Dispose()
        {
            this.mainSubscription?.Dispose();
            this.eventSubscription?.Dispose();
        }
    }
}
