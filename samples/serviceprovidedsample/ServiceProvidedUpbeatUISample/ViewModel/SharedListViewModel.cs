/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows;
using System.Windows.Input;
using UpbeatUI.ViewModel;

namespace ServiceProvidedUpbeatUISample.ViewModel
{
    // This extends BaseViewModel, which provides pre-written SetProperty and RaisePropertyChanged methods.
    public class SharedListViewModel : BaseViewModel, IDisposable
    {
        private readonly IUpbeatService _upbeatService;
        private readonly SharedTimer _sharedTimer;
        private readonly SharedList _sharedList;

        public SharedListViewModel(
            // This will be a unique IUpbeatService created and injected by the IUpbeatStack for this ViewModel and child ViewModels.
            IUpbeatService upbeatService,
            // This is a scoped service shared between this ViewModel and other ViewModel or scoped/transient service dependencies.
            SharedList sharedList,
            // This is a shared singleton service.
            SharedTimer sharedTimer,
            // This is a child ViewModel, which can help with separating concerns and keep ViewModels from being too complicated. Child ViewModels share an IUpbeatService and any scoped services with their parents.
            SharedListDataViewModel sharedListDataViewModel)
        {
            _upbeatService = upbeatService ?? throw new NullReferenceException(nameof(upbeatService));
            _sharedTimer = sharedTimer ?? throw new NullReferenceException(nameof(sharedTimer));
            _sharedList = sharedList ?? throw new ArgumentNullException(nameof(sharedList));
            SharedListDataViewModel = sharedListDataViewModel ?? throw new ArgumentNullException(nameof(sharedListDataViewModel));

            _sharedTimer.Ticked += SharedTimerTicked;
            _sharedList.StringAdded += SharedListStringAdded;

            CloseCommand = new DelegateCommand(_upbeatService.Close);
        }

        public ICommand CloseCommand { get; }
        public string StringsCount => $"{_sharedList.Strings.Count} Strings";
        public string SecondsElapsed => $"{_sharedTimer.ElapsedSeconds} Seconds";
        public SharedListDataViewModel SharedListDataViewModel { get; }

        public void Dispose() =>
            _sharedTimer.Ticked -= SharedTimerTicked;

        private void SharedListStringAdded(object sender, EventArgs e) =>
            // Ensure that the PropertyChanged event is raised on the UI thread
            Application.Current.Dispatcher.Invoke(() => RaisePropertyChanged(nameof(StringsCount)));

        private void SharedTimerTicked(object sender, EventArgs e) =>
            // Ensure that the PropertyChanged event is raised on the UI thread
            Application.Current.Dispatcher.Invoke(() => RaisePropertyChanged(nameof(SecondsElapsed)));

        // This nested Parameters class (full class name: "BottomViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
        public class Parameters
        { }
    }
}
