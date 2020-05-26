using System;
using System.Reactive;
using System.Threading.Tasks;

using MaterialDesignExtensions.Controls;

using MaterialDesignThemes.Wpf;

using MovieList.DialogModels;
using MovieList.Properties;

using ReactiveUI;

using static MovieList.Constants;

namespace MovieList
{
    public class DialogHandler
    {
        public DialogHandler(DialogHost host)
            => this.Host = host;

        public DialogHost Host { get; }

        public async Task ShowMessageDialogAsync(InteractionContext<MessageModel, Unit> ctx)
        {
            if (this.Host.IsOpen)
            {
                ctx.SetOutput(Unit.Default);
                return;
            }

            var viewModel = new MessageModel(
                ctx.Input.Message.Localized(),
                ctx.Input.Title.Localized(),
                ctx.Input.CloseText?.Localized() ?? Messages.OK);

            var view = ViewLocator.Current.ResolveView(viewModel);
            view.ViewModel = viewModel;

            await DialogHost.Show(view);

            ctx.SetOutput(Unit.Default);
        }

        public async Task ShowConfirmDialogAsync(InteractionContext<ConfirmationModel, bool> ctx)
        {
            if (this.Host.IsOpen)
            {
                ctx.SetOutput(false);
                return;
            }

            var viewModel = new ConfirmationModel(
                ctx.Input.Message.Localized(),
                ctx.Input.Title.Localized(),
                ctx.Input.ConfirmText?.Localized() ?? Messages.Confirm,
                ctx.Input.CancelText?.Localized() ?? Messages.Cancel);

            var view = ViewLocator.Current.ResolveView(viewModel);
            view.ViewModel = viewModel;

            var result = await DialogHost.Show(view);

            ctx.SetOutput(result is bool confirm && confirm);
        }

        public async Task ShowInputDialogAsync(InteractionContext<InputModel, string?> ctx)
        {
            if (this.Host.IsOpen)
            {
                ctx.SetOutput(null);
                return;
            }

            var viewModel = new InputModel(
                ctx.Input.Message.Localized(),
                ctx.Input.Title.Localized(),
                ctx.Input.ConfirmText?.Localized() ?? Messages.Confirm,
                ctx.Input.CancelText?.Localized() ?? Messages.Cancel);

            var view = ViewLocator.Current.ResolveView(viewModel);
            view.ViewModel = viewModel;

            var result = await DialogHost.Show(view);

            ctx.SetOutput(result as string);
        }

        public async Task ShowColorDialogAsync(InteractionContext<ColorModel, string?> ctx)
        {
            if (this.Host.IsOpen)
            {
                ctx.SetOutput(null);
                return;
            }

            var viewModel = new ColorModel(
                ctx.Input.Message.Localized(),
                ctx.Input.Title.Localized(),
                ctx.Input.ConfirmText?.Localized() ?? Messages.Confirm,
                ctx.Input.CancelText?.Localized() ?? Messages.Cancel);

            var view = ViewLocator.Current.ResolveView(viewModel);
            view.ViewModel = viewModel;

            var result = await DialogHost.Show(view);

            ctx.SetOutput(result as string);
        }

        public async Task ShowSaveFileDialogAsync(InteractionContext<string, string?> ctx)
        {
            var dialogArgs = new SaveFileDialogArguments
            {
                Width = 1000,
                Height = 600,
                Filters = $"{Messages.FileExtensionDescription}|*.{ListFileExtension}|" +
                          $"{Messages.AllExtensionsDescription}|*",
                CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filename = ctx.Input,
                ForceFileExtensionOfFileFilter = true,
                CreateNewDirectoryEnabled = true
            };

            var result = await SaveFileDialog.ShowDialogAsync(this.Host, dialogArgs);

            ctx.SetOutput(result == null || result.Canceled ? null : result.File);
        }

        public async Task ShowOpenFileDialogAsync(InteractionContext<Unit, string?> ctx)
        {
            var dialogArgs = new OpenFileDialogArguments
            {
                Width = 1000,
                Height = 600,
                Filters = $"{Messages.FileExtensionDescription}|*.{ListFileExtension}|" +
                          $"{Messages.AllExtensionsDescription}|*",
                CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            var result = await OpenFileDialog.ShowDialogAsync(this.Host, dialogArgs);

            ctx.SetOutput(result == null || result.Canceled ? null : result.File);
        }
    }
}
