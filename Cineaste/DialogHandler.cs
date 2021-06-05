
using System.Reactive;
using System.Threading.Tasks;

using Avalonia.Controls;

using Cineaste.Core.DialogModels;
using Cineaste.Core.ViewModels.Forms.Preferences;

using ReactiveUI;

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

        public Task ShowSaveFileDialogAsync(InteractionContext<string, string?> ctx) =>
            Task.CompletedTask;

        public Task ShowOpenFileDialogAsync(InteractionContext<Unit, string?> ctx) =>
            Task.CompletedTask;

        public Task ShowAboutDialogAsync(InteractionContext<AboutModel, Unit> ctx) =>
            Task.CompletedTask;
    }
}
