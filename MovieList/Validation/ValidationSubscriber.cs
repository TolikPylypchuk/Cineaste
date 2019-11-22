using System;
using System.Windows;

using ReactiveUI.Validation.Helpers;

namespace MovieList.Validation
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
                    if (!state.IsValid)
                    {
                        subscriber.eventSubscription = element.Events().LostFocus
                            .Subscribe(_ =>
                            {
                                ManualValidation.MarkInvalid(element, state.Text[0]);
                                subscriber.eventSubscription?.Dispose();
                            });
                    } else
                    {
                        ManualValidation.ClearValidation(element);
                        subscriber.eventSubscription?.Dispose();
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
