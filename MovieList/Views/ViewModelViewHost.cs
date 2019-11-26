// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;

using ReactiveUI;

using Splat;

namespace MovieList.Views
{
    [SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "Deliberate usage")]
    public class ViewModelViewHost : TransitioningContentControl, IViewFor, IEnableLogger, IDisposable
    {
        /// <summary>
        /// The default content dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultContentProperty = DependencyProperty.Register(
            nameof(DefaultContent),
            typeof(object),
            typeof(ViewModelViewHost),
            new PropertyMetadata(null));

        /// <summary>
        /// The view model dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel),
            typeof(object),
            typeof(ViewModelViewHost),
            new PropertyMetadata(null, SomethingChanged));

        /// <summary>
        /// The view contract observable dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewContractObservableProperty = DependencyProperty.Register(
            nameof(ViewContractObservable),
            typeof(IObservable<string?>),
            typeof(ViewModelViewHost),
            new PropertyMetadata(Observable.Return(default(string)), SomethingChanged));

        private readonly Subject<Unit> updateViewModel = new Subject<Unit>();
        private string? viewContract;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelViewHost"/> class.
        /// </summary>
        public ViewModelViewHost()
        {
            if (ModeDetector.InUnitTestRunner())
            {
                this.ViewContractObservable = Observable.Never<string>();
                return;
            }

            this.ViewContractObservable = Observable.FromEvent<SizeChangedEventHandler, string?>(
                eventHandler =>
                {
                    void Handler(object sender, SizeChangedEventArgs e) => eventHandler(null);
                    return Handler;
                },
                x => SizeChanged += x,
                x => SizeChanged -= x)
                .StartWith((string?)null)
                .DistinctUntilChanged();

            var contractChanged = updateViewModel.Select(_ => ViewContractObservable).Switch();

            var contract = new BehaviorSubject<string?>(null);
            contractChanged.Subscribe(contract);

            updateViewModel.Subscribe(_ => ResolveViewForViewModel(ViewModel, contract.Value));

            contractChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => viewContract = x);
        }

        /// <summary>
        /// Gets or sets the view contract observable.
        /// </summary>
        public IObservable<string?> ViewContractObservable
        {
            get => (IObservable<string?>)this.GetValue(ViewContractObservableProperty);
            set => this.SetValue(ViewContractObservableProperty, value);
        }

        /// <summary>
        /// Gets or sets the content displayed by default when no content is set.
        /// </summary>
        public object DefaultContent
        {
            get => this.GetValue(DefaultContentProperty);
            set => this.SetValue(DefaultContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the ViewModel to display.
        /// </summary>
        public object? ViewModel
        {
            get => this.GetValue(ViewModelProperty);
            set => this.SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// Gets or sets the view contract.
        /// </summary>
        public string? ViewContract
        {
            get => this.viewContract;
            set => this.ViewContractObservable = Observable.Return(value);
        }

        /// <summary>
        /// Gets or sets the view locator.
        /// </summary>
        public IViewLocator? ViewLocator { get; set; }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of resources inside the class.
        /// </summary>
        /// <param name="isDisposing">If we are disposing managed resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                this.updateViewModel?.Dispose();
            }

            isDisposed = true;
        }

        private static void SomethingChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
            => ((ViewModelViewHost)dependencyObject).updateViewModel.OnNext(Unit.Default);

        private void ResolveViewForViewModel(object? viewModel, string? contract)
        {
            if (viewModel == null)
            {
                this.Content = DefaultContent;
                return;
            }

            var viewLocator = this.ViewLocator ?? ReactiveUI.ViewLocator.Current;
            var viewInstance = viewLocator.ResolveView(viewModel, contract) ?? viewLocator.ResolveView(viewModel);

            if (viewInstance == null)
            {
                this.Content = this.DefaultContent;
                this.Log().Warn($"The {nameof(ViewModelViewHost)} could not find a valid view for the view model of type {viewModel.GetType()} and value {viewModel}.");
                return;
            }

            viewInstance.ViewModel = viewModel;

            this.Content = viewInstance;
        }
    }
}
