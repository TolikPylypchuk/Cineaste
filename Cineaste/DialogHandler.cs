using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;

using Cineaste.Core;
using Cineaste.Core.DialogModels;
using Cineaste.Core.ViewModels.Forms.Preferences;
using Cineaste.Properties;

using ReactiveUI;

using static Cineaste.Constants;
using static Cineaste.Core.ServiceUtil;
using static Cineaste.Util;

namespace Cineaste
{
    public sealed class DialogHandler
    {
        private readonly Window window;

        public DialogHandler(Window window) =>
            this.window = window;

        public async Task ShowMessageDialogAsync(InteractionContext<MessageModel, Unit> ctx)
        {
            var result = await this.ShowDialogWindowForViewModel<MessageModel, Unit>(
                ctx.Input,
                vm => new MessageModel(
                    vm.Message.Localized(),
                    vm.Title.Localized(),
                    vm.CloseText?.Localized() ?? Messages.OK));

            ctx.SetOutput(result);
        }

        public async Task ShowConfirmDialogAsync(InteractionContext<ConfirmationModel, bool> ctx)
        {
            bool? result = await this.ShowDialogWindowForViewModel<ConfirmationModel, bool?>(
                ctx.Input,
                vm => new ConfirmationModel(
                vm.Message.Localized(),
                vm.Title.Localized(),
                vm.ConfirmText?.Localized() ?? Messages.Confirm,
                vm.CancelText?.Localized() ?? Messages.Cancel));

            ctx.SetOutput(result == true);
        }

        public async Task ShowInputDialogAsync(InteractionContext<InputModel, string?> ctx)
        {
            string? result = await this.ShowDialogWindowForViewModel<InputModel, string?>(ctx.Input);
            ctx.SetOutput(result);
        }

        public async Task ShowTagFormDialogAsync(InteractionContext<TagFormViewModel, Unit> ctx)
        {
            var result = await this.ShowDialogWindowForViewModel(
                ctx.Input, title: null, vm => vm.Close, vmTitle: vm => vm.FormTitle);
            ctx.SetOutput(result);
        }

        public async Task ShowSaveFileDialogAsync(InteractionContext<string, string?> ctx)
        {
            var dialog = new SaveFileDialog
            {
                Title = Messages.SaveFile,
                Directory = this.GetFileDialogInitialDirectory(),
                InitialFileName = ctx.Input,
                Filters = new()
                {
                    new() { Name = Messages.FileExtensionDescription, Extensions = new() { ListFileExtension } },
                    new() { Name = Messages.AllExtensionsDescription, Extensions = new() { "*" } },
                }
            };

            string fileName = await dialog.ShowAsync(this.window);
            ctx.SetOutput(fileName);
        }

        public async Task ShowOpenFileDialogAsync(InteractionContext<Unit, string?> ctx)
        {
            var dialog = new OpenFileDialog
            {
                Title = Messages.OpenFile,
                Directory = this.GetFileDialogInitialDirectory(),
                Filters = new()
                {
                    new() { Name = Messages.FileExtensionDescription, Extensions = new() { ListFileExtension } },
                    new() { Name = Messages.AllExtensionsDescription, Extensions = new() { "*" } },
                }
            };

            string[]? fileNames = await dialog.ShowAsync(this.window);

            if (fileNames.Length > 0)
            {
                ctx.SetOutput(fileNames[0]);
            } else
            {
                ctx.SetOutput(null);
            }
        }

        public async Task ShowAboutDialogAsync(InteractionContext<AboutModel, Unit> ctx)
        {
            var result = await this.ShowDialogWindowForViewModel(ctx.Input, nameof(Messages.About), vm => vm.Close);
            ctx.SetOutput(result);
        }

        private Task<TResult?> ShowDialogWindowForViewModel<TViewModel, TResult>(
            TViewModel viewModel,
            Func<TViewModel, TViewModel>? transform = null)
            where TViewModel : DialogModelBase<TResult> =>
            this.ShowDialogWindowForViewModel(viewModel, viewModel.Title, vm => vm.Close, transform: transform);

        private async Task<TResult?> ShowDialogWindowForViewModel<TViewModel, TResult>(
            TViewModel viewModel,
            string? title,
            Func<TViewModel, IObservable<TResult>> close,
            Expression<Func<TViewModel, string?>>? vmTitle = null,
            Func<TViewModel, TViewModel>? transform = null)
            where TViewModel : ReactiveObject
        {
            var view = GetDefaultService<IViewFor<TViewModel>>();
            var newViewModel = transform != null ? transform(viewModel) : viewModel;
            view.ViewModel = newViewModel;

            var window = new Window
            {
                Content = view,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                SizeToContent = SizeToContent.WidthAndHeight,
                CanResize = false,
                Icon = this.window.Icon
            };

            var subscriptions = new CompositeDisposable();

            if (title != null)
            {
                window.Title = Messages.ResourceManager.GetString(title, CultureInfo.CurrentUICulture) ?? title;
            } else if (vmTitle != null)
            {
                newViewModel.WhenAnyValue(vmTitle)
                    .Select(title => !String.IsNullOrWhiteSpace(title)
                        ? String.Format(CultureInfo.InvariantCulture, Messages.TagFormHeaderFormat, title)
                        : Messages.TagFormHeader)
                    .BindTo(window, w => w.Title)
                    .DisposeWith(subscriptions);
            }

            var result = default(TResult);

            close(view.ViewModel)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(closeResult =>
                {
                    result = closeResult;
                    window.Close();
                })
                .DisposeWith(subscriptions);

            await window.ShowDialog(this.window);

            subscriptions.Dispose();

            return result;
        }

        private string GetFileDialogInitialDirectory() =>
            PlatformDependent(
                windows: () => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                macos: GetUnixHomeFolder,
                linux: GetUnixHomeFolder);
    }
}
