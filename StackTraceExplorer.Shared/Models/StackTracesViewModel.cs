using Microsoft.VisualStudio.PlatformUI;
using StackTraceExplorer.Shared.Models;
using System.Collections.ObjectModel;

namespace StackTraceExplorer.Models
{
    public class StackTracesViewModel : ObservableObject
    {
        private ObservableCollection<StackTrace> _stackTraces = new ObservableCollection<StackTrace>();

        public ObservableCollection<StackTrace> StackTraces
        {
            get => _stackTraces;
            set => SetProperty(ref _stackTraces, value);
        }

        private int _selectedStackTraceIndex;

        public int SelectedStackTraceIndex
        {
            get => _selectedStackTraceIndex;
            set => SetProperty(ref _selectedStackTraceIndex, value);
        }

        public void AddStackTrace(string trace)
        {
            _stackTraces.Add(new StackTrace(trace));
            NotifyPropertyChanged("StackTraces");
        }

        public void SetStackTrace(string trace)
            => _stackTraces[_selectedStackTraceIndex].SetStackTrace(trace);

        public void AddClickedLine(CustomLinkVisualLineText line)
            => _stackTraces[_selectedStackTraceIndex].AddClickedLine(line);

        public bool IsClickedLine(CustomLinkVisualLineText line)
            => _stackTraces[_selectedStackTraceIndex].IsClickedLine(line);
    }
}
