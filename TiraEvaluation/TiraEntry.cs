using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TiraEvaluation
{
    public class TiraEntry : INotifyPropertyChanged
    {
        double _overallAccuracy;
        string _path;

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                On_PropertyChanged("Path");
            }
        }

        public double OverallAccuracy
        {
            get
            {
                return _overallAccuracy;
            }
            set
            {
                _overallAccuracy = value;
                On_PropertyChanged("OverallAccuracy");
            }
        }

        ICommand _selectPathCommand;
        public ICommand SelectPathCommand
        {
            get
            {
                return _selectPathCommand ?? (_selectPathCommand = new RelayCommand(obj =>
                {
                    var diag = new OpenFileDialog();
                    diag.Multiselect = false;
                    diag.Filter = "Json Files (*.json)|*.json";

                    if (diag.ShowDialog().GetValueOrDefault())
                    {
                        Path = diag.FileName;
                    }
                }, obj => true));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void On_PropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}