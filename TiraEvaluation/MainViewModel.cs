using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TiraEvaluation
{
    public class MainViewModel : INotifyPropertyChanged
    {
        string _groundTruthPath;

        public MainViewModel()
        {
            TiraEntries = new ObservableCollection<TiraEntry>();
        }

        public string GroundTruthPath
        {
            get
            {
                return _groundTruthPath;
            }
            set
            {
                _groundTruthPath = value;
                On_PropertyChanged("GroundTruthPath");
            }
        }

        public ObservableCollection<TiraEntry> TiraEntries { get; set; }

        ICommand _addEntryCommand;
        public ICommand AddEntryCommand
        {
            get
            {
                return _addEntryCommand ?? (_addEntryCommand = new RelayCommand(obj =>
                {
                    TiraEntries.Add(new TiraEntry());
                }, obj => true));
            }
        }

        ICommand _selectGroundTruthPathCommand;
        public ICommand SelectGroundTruthPathCommand
        {
            get
            {
                return _selectGroundTruthPathCommand ?? (_selectGroundTruthPathCommand = new RelayCommand(obj =>
                {
                    var diag = new OpenFileDialog();
                    diag.Multiselect = false;
                    diag.Filter = "Json Files (*.json)|*.json";

                    if (diag.ShowDialog().GetValueOrDefault())
                    {
                        GroundTruthPath = diag.FileName;
                    }
                }, obj => true));
            }
        }

        ICommand _evaluateCommand;
        public ICommand EvaluateCommand
        {
            get
            {
                return _evaluateCommand ?? (_evaluateCommand = new RelayCommand(obj =>
                {
                    try
                    {
                        new EvaluationRun().Evaluate(TiraEntries, GroundTruthPath);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
                }, obj => true));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void On_PropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}