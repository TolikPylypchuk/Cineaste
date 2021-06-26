using System;
using System.Reactive;
using System.Threading.Tasks;

using Avalonia.Controls;

using Cineaste.Core;
using Cineaste.Core.DialogModels;
using Cineaste.Core.ViewModels.Forms.Preferences;
using Cineaste.Properties;

using ReactiveUI;

using static Cineaste.Constants;
using static Cineaste.Util;
using static Cineaste.Core.Util;

using static Cineaste.Core.ServiceUtil;

namespace Cineaste
{
    public sealed class DialogHandler
    {
        private readonly Window window;

        public DialogHandler(Window window) =>
            this.window = window;

        public Task ShowMessageDialogAsync(InteractionContext<MessageModel, Unit> ctx) =>
            this.ShowDialogWindowForViewModel(
                ctx.Input,
                vm => new MessageModel(
                    vm.Message.Localized(),
                    vm.Title.Localized(),
                    vm.CloseText?.Localized() ?? Messages.OK));

        public Task ShowConfirmDialogAsync(InteractionContext<ConfirmationModel, bool> ctx) =>
            this.ShowDialogWindowForViewModel(
                ctx.Input,
                vm => new ConfirmationModel(
                    vm.Message.Localized(),
                    vm.Title.Localized(),
                    vm.ConfirmText?.Localized() ?? Messages.Confirm,
                    vm.CancelText?.Localized() ?? Messages.Cancel));

        public async Task ShowInputDialogAsync(InteractionContext<InputModel, string?> ctx)
        {
            await this.ShowDialogWindowForViewModel(ctx.Input);
            ctx.SetOutput(null);
        }

        public async Task ShowColorDialogAsync(InteractionContext<ColorModel, string?> ctx)
        {
            await this.ShowDialogWindowForViewModel(ctx.Input);
            ctx.SetOutput(null);
        }

        public async Task ShowTagFormDialogAsync(InteractionContext<TagFormViewModel, Unit> ctx)
        {
            await this.ShowDialogWindowForViewModel(ctx.Input, ctx.Input.FormTitle, ctx.Input.Close);
            ctx.SetOutput(Unit.Default);
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
            await this.ShowDialogWindowForViewModel(ctx.Input, Messages.About, ctx.Input.Close);
            ctx.SetOutput(Unit.Default);
        }

        private string GetFileDialogInitialDirectory() =>
            PlatformDependent(
                windows: () => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                macos: GetUnixHomeFolder,
                linux: GetUnixHomeFolder);

        private Task ShowDialogWindowForViewModel<TViewModel>(
            TViewModel viewModel,
            Func<TViewModel, TViewModel>? transform = null)
            where TViewModel : DialogModelBase =>
            this.ShowDialogWindowForViewModel(viewModel, viewModel.Title, viewModel.Close, transform);

        private async Task ShowDialogWindowForViewModel<TViewModel>(
            TViewModel viewModel,
            string title,
            IObservable<Unit> close,
            Func<TViewModel, TViewModel>? transform = null)
            where TViewModel : ReactiveObject
        {
            var view = GetDefaultService<IViewFor<TViewModel>>();
            view.ViewModel = transform != null ? transform(viewModel) : viewModel;

            var window = new Window
            {
                Content = view,
                Title = title,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                SizeToContent = SizeToContent.WidthAndHeight,
                CanResize = false
            };

            var subscription = close.Subscribe(() => window.Close());

            await window.ShowDialog(this.window);

            subscription.Dispose();
        }
    }
}
