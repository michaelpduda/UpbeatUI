/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UpbeatUI.ViewModel;

namespace BasicUpbeatUISample.ViewModel
{
    // This extends BaseViewModel, which provides pre-written SetProperty and RaisePropertyChanged methods.
    public class BottomViewModel : BaseViewModel, IDisposable
    {
        private readonly IUpbeatService _upbeatService;
        private readonly SharedTimer _sharedTimer;

        public BottomViewModel(
            // This will be a unique IUpbeatService created and injected by the IUpbeatStack specifically for this ViewModel.
            IUpbeatService upbeatService,
            // This is a shared singleton service.
            SharedTimer sharedTimer)
        {
            _upbeatService = upbeatService ?? throw new NullReferenceException(nameof(upbeatService));
            _sharedTimer = sharedTimer ?? throw new NullReferenceException(nameof(sharedTimer));

            // Registering a CloseCallback allows the ViewModel to prevent itself from closing. For example: if there is unsaved work. This can also completely prevent the application from shutting down. CloseCallbacks can be either async or non-async methods/lambdas.
            _upbeatService.RegisterCloseCallback(AskBeforeClosingAsync);

            _sharedTimer.Ticked += SharedTimerTicked;

            // DelegateCommand is a common convenience ICommand implementation to call methods or lambda expressions when the command is executed. It supports both async and non-async methods/lambdas.
            OpenMenuCommand = new DelegateCommand(
                // Create a Parameters object for a ViewModel and pass it to the IUpbeatStack using OpenViewModel. The IUpbeatStack will use the configured mappings to create the appropriate ViewModel from the Parameters type.
                () => _upbeatService.OpenViewModel(
                    new MenuViewModel.Parameters()));
            OpenSharedListCommand = new DelegateCommand(
                () => _upbeatService.OpenViewModel(
                    new SharedListViewModel.Parameters()));
            OpenRandomDataCommand = new DelegateCommand(
                () => _upbeatService.OpenViewModel(
                    new RandomDataViewModel.Parameters()));
        }

        public ICommand OpenMenuCommand { get; }
        public ICommand OpenSharedListCommand { get; }
        public ICommand OpenRandomDataCommand { get; }
        public string SecondsElapsed => $"{_sharedTimer.ElapsedSeconds} Seconds";

        public void Dispose() =>
            _sharedTimer.Ticked -= SharedTimerTicked;

        // This CloseCallback method opens a new ViewModel and View to confirm that the user wants to close this ViewModel.
        private async Task<bool> AskBeforeClosingAsync()
        {
            bool okToClose = false;
            // OpenViewModelAsync can be awaited, and will return once the child ViewModel is closed. This is useful to show a popup requesting input from the user.
            await _upbeatService.OpenViewModelAsync(
                new ConfirmPopupViewModel.Parameters
                {
                    Message = "The application is trying to exit.\nClick Confirm to exit or off this popup to cancel.",
                    // The ConfirmPopupViewModel will execute this callback (set the okToClose bool to true) if the user confirms that closing. If the popup closes without the user confirming, okToClose remains false, and the application will remain running.
                    ConfirmCallback = () => okToClose = true,
                });
            return okToClose;
        }

        private void SharedTimerTicked(object sender, EventArgs e) =>
            // Ensure that the PropertyChanged event is raised on the UI thread
            Application.Current.Dispatcher.Invoke(() => RaisePropertyChanged(nameof(SecondsElapsed)));

        // This nested Parameters class (full class name: "BottomViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
        public class Parameters
        { }
    }
}
