using Caliburn.Micro;
using ICSharpCode.AvalonEdit.Document;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace StackTraceExplorer.Models
{
    public class StackTracesViewModel : PropertyChangedBase
    {
        public StackTracesViewModel()
        {
            _stackTraces = new ObservableCollection<Stacktrace>();
        }

        private ObservableCollection<Stacktrace> _stackTraces;
        public ObservableCollection<Stacktrace> StackTraces
        {
            get
            {
                return _stackTraces;
            }
            set
            {
                _stackTraces = value;
                NotifyOfPropertyChange();
            }
        }

        private int _selectedStackTraceIndex;
        public int SelectedStackTraceIndex
        {
            get
            {
                return _selectedStackTraceIndex;
            }
            set
            {
                _selectedStackTraceIndex = value;
                NotifyOfPropertyChange();
            }
        }

        public void AddStackTrace(string trace)
        {
            _stackTraces.Add(new Stacktrace(trace));
            NotifyOfPropertyChange("StackTraces");
        }

        public void SetStackTrace(string trace) => _stackTraces[_selectedStackTraceIndex].SetStackTrace(trace);

        public void AddClickedLine(CustomLinkVisualLineText line) => _stackTraces[_selectedStackTraceIndex].AddClickedLine(line);

        public bool IsClickedLine(CustomLinkVisualLineText line) => _stackTraces[_selectedStackTraceIndex].IsClickedLine(line);
    }

    public class Stacktrace : PropertyChangedBase
    {
        public TextDocument Document { get; set; }
        public List<CustomLinkVisualLineText> ClickedLines { get; set; }

        public Stacktrace(string trace = null)
        {
            SetStackTrace(trace);
            ClickedLines = new List<CustomLinkVisualLineText>();
        }

        private bool _wordWrap;
        public bool WordWrap
        {
            get
            {
                return _wordWrap;
            }
            set
            {
                _wordWrap = value;
                NotifyOfPropertyChange();
            }
        }

        public void SetStackTrace(string trace)
        {
            Document = new TextDocument { Text = trace };
            NotifyOfPropertyChange("Document");
        }

        public void AddClickedLine(CustomLinkVisualLineText line) => ClickedLines.Add(line);

        public bool IsClickedLine(CustomLinkVisualLineText line) => ClickedLines.Any(l => l.Link.SequenceEqual(line.Link));
    }
}
