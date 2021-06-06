using System;
using System.Reactive;
using System.Threading.Tasks;

using Avalonia.Controls;

using Cineaste.Core.DialogModels;
using Cineaste.Core.ViewModels.Forms.Preferences;
using Cineaste.Properties;

using ReactiveUI;

using static Cineaste.Constants;
using static Cineaste.Util;
using static Cineaste.Core.Util;

namespace Cineaste
{
    public sealed class DialogHandler
    {
        private readonly Window window;

        public DialogHandler(Window window) =>
            this.window = window;

        public Task ShowMessageDialogAsync(InteractionContext<MessageModel, Unit> ctx) =>
            Task.CompletedTask;

        public Task ShowConfirmDialogAsync(InteractionContext<ConfirmationModel, bool> ctx) =>
            Task.CompletedTask;

        public Task ShowInputDialogAsync(InteractionContext<InputModel, string?> ctx) =>
            Task.CompletedTask;

        public Task ShowColorDialogAsync(InteractionContext<ColorModel, string?> ctx) =>
            Task.CompletedTask;

        public Task ShowTagFormDialogAsync(InteractionContext<TagFormViewModel, Unit> ctx) =>
            Task.CompletedTask;

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

            var fileNames = await dialog.ShowAsync(this.window);

            if (fileNames.Length > 0)
            {
                ctx.SetOutput(fileNames[0]);
            }
        }

        public Task ShowAboutDialogAsync(InteractionContext<AboutModel, Unit> ctx) =>
            Task.CompletedTask;

        private string GetFileDialogInitialDirectory() =>
            PlatformDependent(
                windows: () => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                macos: GetUnixHomeFolder,
                linux: GetUnixHomeFolder);
    }
}
