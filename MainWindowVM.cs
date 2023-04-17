using LSM6200_FL100.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace ParetoHadron
{
    public class MainWindowVM : ViewModelBase
    {
        
       
        private DateTime selectedStartDate;
        public DateTime SelectedStartDate
        {
            get => selectedStartDate;
            set
            {
                selectedStartDate = value;
                OnPropertyChanged("SelectedStartDate");
            }
        }

        private DateTime selectedEndDate;
        public DateTime SelectedEndDate
        {

            get => selectedEndDate;
            set
            {
                selectedEndDate = value;
                OnPropertyChanged("SelectedEndDate");
            }
        }
        public MainWindowVM()
        {
            SelectedStartDate = DateTime.Now;
            SelectedEndDate = DateTime.Now;
            //getMTF = new RelayCommand(ExecuteGetMTF, CanExecuteGetMTF);
        }

        #region Relay command example 

        RelayCommand<MainWindowVM> MTFResultsRelayCommand;
       

        public ICommand MTFResultsICommand => MTFResultsRelayCommand ?? (MTFResultsRelayCommand = new RelayCommand<MainWindowVM>(
            param => MTFResultsAction(),
            param => true));

        private async Task MTFResultsAction()
        {
            //var start = SelectedStartDate;
            //var end = SelectedEndDate;  
            MessageBox.Show("This should show.");
            //start = StartDateDisplay.Text;
            //var end = EndDateDisplay.Text;
            //    start = start.Split(" ")[0];
            //    end = end.Split(" ")[0];

            //    var start_p = DateTime.Parse(start);
            //    var end_p = DateTime.Parse(end);
            //    var days = (end_p - start_p);




            //    var dirs = Directory.GetFiles(@"C:\Hadron640\debugOnly");
            //    //var dirs = Directory.GetDirectories("C:\\Hadron640\\debugOnly").ToList();
            //    List<string> EOMTFdata = new List<string>();
            //    string resultsPath = @"C:\Hadron640\defect_results\MTFOnly.csv";
            //    StreamWriter sw = new StreamWriter(resultsPath);

            //    foreach (var file in dirs)
            //    {
            //        var cf = File.GetLastWriteTime(file);

            //        if (start_p < cf && cf < end_p)
            //        {
            //            List<string> rawData = new List<string>();
            //            rawData = File.ReadAllLines(file).ToList();
            //            foreach (string line in rawData)
            //            {
            //                string sn = file.Split("\\")[3].Split("_")[0];
            //                if (line.Contains("EO MTF:"))
            //                {
            //                    string date = line.Split(" ")[0];
            //                    string data = line.Split(":")[3];
            //                    sw.WriteLine(sn + "," + "EO MTF" + "," + data + "," + date, true);
            //                }

            //                if (line.Contains("IR MTF:"))
            //                {
            //                    string date = line.Split(" ")[0];
            //                    string data = line.Split(":")[3];
            //                    sw.WriteLine(sn + "," + "IR MTF" + "," + data + "," + date, true);
            //                }
            //            }
            //        }
            //    }
            //}

            #endregion
        }
    }
}
    //private bool mTFRunning = true;
    //public bool MTFRunning
    //{
    //    get { return mTFRunning; }
    //    set { }
    //}


    //private RelayCommand getMTF;
    //public RelayCommand GetMTF { get { return getMTF; } set { getMTF = value; } }

    //private bool CanExecuteGetMTF(object obj)
    //{
    //    return true; 
    //}

    //private void ExecuteGetMTF(object obj)
    //{

    //}



 
    




//    }

//    }

