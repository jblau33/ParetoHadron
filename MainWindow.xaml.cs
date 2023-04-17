using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ParetoHadron
{
     /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window    
    {
        private MainWindowVM Vm;
        public MainWindow(MainWindowVM vm)
        {
            DataContext = vm;
            Vm = vm;
            InitializeComponent();
        }

        private async void btn_LatestFiles_Click(object sender, RoutedEventArgs e)
        {
            //int index = fileList.Count;
            List<string> files = new List<string>();
            string[] fileList = Directory.GetFiles(@"C:\\Hadron640\\debugOnly");
            StreamWriter sw = new StreamWriter(@"C:\Hadron640\defect_results\latestFiles.csv");
            int i = 0;
            //foreach (string file in fileList)
            while (i < fileList.Length - 1)
            {
                string sn = fileList[i].Split("\\")[5].Split("_")[0];
                string comp = fileList[i + 1].Split("\\")[5].Split("_")[0];

                for (i = 0; i < fileList.Length; i++)
                {
                    if (comp == sn)
                    { }
                }
            }
            sw.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string folderName = @"\\gold\MicroBoloTools\Production Automation\Hadron_640R\LogFiles\ATP";
            var dirs = Directory.GetDirectories(folderName).ToList();
            dirs.Remove(folderName + "\\" + "TestC");
            string dbFinalFolder = @"\\gold\MicroBoloTools\Production Automation\Hadron_640R\LogFiles\debugOnly\";
            string localDbOnlyFolder = @"C:\Hadron640\debugOnly\";

            List<string> subFolders = new List<string>();
            foreach (string folder in dirs)
            {
                var sub2Folders = Directory.GetDirectories(folder).ToList();
                foreach (string sub3Folder in sub2Folders)
                {
                    //get the number of folders in serial number folder 
                    var temp = Directory.GetDirectories(sub3Folder).Length;
                    var folders = Directory.GetDirectories(sub3Folder);

                    //Iterate through each folder until the last filename = length 
                    foreach (var f in folders)
                    {
                        var number = System.IO.Path.GetFileName(f);
                        if (number == temp.ToString())
                        {
                            //Do something here 
                            string origPath = f + "\\" + "debug.txt";
                            string[] desc = f.Split('\\');
                            string finalPath = (dbFinalFolder + desc[9] + "_" + desc[10] + "_" + "debug.txt");
                            string finalPathLocal = (localDbOnlyFolder + desc[9] + "_" + desc[10] + "_" + "debug.txt");
                            //File.Copy(origPath, finalPath, true);
                            File.Copy(origPath, finalPathLocal, true);
                        }
                    }

                }
            }
            MessageBox.Show("Done");
        }

        //private void btn_EOMTFOnly_Click(object sender, RoutedEventArgs e)
        //{    var start = StartDateDisplay.Text; 
        //     var end = EndDateDisplay.Text;
        //     start = start.Split(" ")[0];
        //     end = end.Split(" ")[0];

        //    var start_p = DateTime.Parse(start);
        //    var end_p = DateTime.Parse(end); 
        //    var days =  (end_p - start_p); 



            
        //    var dirs = Directory.GetFiles(@"C:\Hadron640\debugOnly");
        //    //var dirs = Directory.GetDirectories("C:\\Hadron640\\debugOnly").ToList();
        //    List<string> EOMTFdata = new List<string>();
        //    string resultsPath = @"C:\Hadron640\defect_results\MTFOnly.csv";
        //    StreamWriter sw = new StreamWriter(resultsPath);

        //    foreach (var file in dirs)
        //    {
        //        var cf = File.GetLastWriteTime(file);

        //        if (start_p < cf  && cf<end_p) 
        //        { 
        //            List<string> rawData = new List<string>();
        //            rawData = File.ReadAllLines(file).ToList();
        //            foreach (string line in rawData)
        //            {
        //                string sn = file.Split("\\")[3].Split("_")[0];
        //                 if (line.Contains("EO MTF:"))
        //                 {
        //                    string date = line.Split(" ")[0];
        //                    string data = line.Split(":")[3];
        //                    sw.WriteLine(sn + "," + "EO MTF" + "," + data + "," + date, true);
        //                 }

        //                if (line.Contains("IR MTF:"))
        //                {
        //                string date = line.Split(" ")[0];
        //                string data = line.Split(":")[3];
        //                sw.WriteLine(sn + "," + "IR MTF" + "," + data + "," + date, true);
        //                }
        //            }
        //        }
        //    }
        //}


        private async void btn_Summary_Click(object sender, RoutedEventArgs e)
        {
            string resultsPath = @"C:\Hadron640\defect_results\defects.csv";
            List<string> fileList = File.ReadAllLines(@"C:\Hadron640\defect_results\latestFiles.txt").ToList();
            List<string> NonIRData = new List<string>();
            List<string> IRData = new List<string>();
            List<string> AllData = new List<string>();
            List<string> NonIRDataDefects = new List<string>();

            StreamWriter sw = new StreamWriter(resultsPath);

            foreach (string file in fileList)
            {
                string sn = file.Split("\\")[3].Split("_")[0];

                //for non-IR acuracy data
                NonIRData = GetNonIRAccuracyData(file);
                Task.Delay(100);
                NonIRDataDefects = GetNonIRDataDefects(NonIRData);
                Task.Delay(100);




                foreach (string line in NonIRDataDefects)
                {
                    AllData.Add(line);
                }

                //for IR accuracy defects....
                string stageLocs = getStageLocs(file);
                string[] BBErrors = findBBErrors(file, stageLocs).Split(",");

                if (BBErrors[0] == "True")
                {
                    AllData.Add(sn + "," + "BBHigh" + "," + "," + BBErrors[3]);
                }

                if (BBErrors[1] == "True")
                {
                    AllData.Add(sn + "," + "BBMid" + "," + "," + BBErrors[3]);
                }

                if (BBErrors[2] == "True")
                {
                    AllData.Add(sn + "," + "BBLow" + "," + "," + BBErrors[3]);
                }

            }
            foreach (string f_result in AllData)
            {
                sw.WriteLine(f_result);
            }
            sw.Close();
            MessageBox.Show("Done");
        }

        private void btn_Pareto_Click(object sender, RoutedEventArgs e)
        {
            string[] pc = File.ReadAllLines(@"C:\Hadron640\defect_results\defects.txt");
            string[] d_list = File.ReadAllLines(@"C:\Hadron640\defect_results\defectList.txt");
            StreamWriter wpd = new StreamWriter(@"C:\Hadron640\defect_results\pareto.csv");
            foreach (string defect in d_list)
            {
                int i = 0;
                foreach (string line in pc)
                {
                    if (line.Contains(defect))
                    {
                        i++;
                    }
                }

                {
                    wpd.WriteLine(defect + "," + i, true);
                }
            }
            wpd.Close();
        }

        private string findBBErrors(string file, string stageLoc)
        {
            string[] lines = File.ReadAllLines(file);
            string date = lines[0].Split(" ")[0];
            string BBHighStart = stageLoc.Split(",")[0];
            string BBMidStart = stageLoc.Split(",")[1];
            string BBLowStart = stageLoc.Split(",")[2];
            string BBLowEnd = stageLoc.Split(",")[3];

            int BBH;
            int BBM;
            int BBL;
            int BBLEnd;

            int.TryParse(BBHighStart, out BBH);
            int.TryParse(BBMidStart, out BBM);
            int.TryParse(BBLowStart, out BBL);
            int.TryParse(BBLowEnd, out BBLEnd);

            bool BBHigh = false;
            bool BBMid = false;
            bool BBLow = false;

            for (int i = BBH; i <= BBM; i++)
            {
                if (lines[i].Contains("[Error]"))
                {
                    BBHigh = true;

                }
            }

            for (int i = BBM; i <= BBL; i++)
            {
                if (lines[i].Contains("[Error]"))
                {
                    BBMid = true;
                }
            }

            for (int i = BBL; i <= BBLEnd; i++)
            {
                if (lines[i].Contains("[Error]"))
                {
                    BBLow = true;
                }
            }
            return BBHigh + "," + BBMid + "," + BBLow + "," + date;
        }

        private string getStageLocs(string file)
        {
            int index = 0;
            string stageLocs = "";
            int stage1_1 = 0;
            int stage1_2 = 0;
            int stage2 = 0;
            int stage3 = 0;

            string[] lines = File.ReadAllLines(file);

            string date = lines[0].Split(" ")[0];
            foreach (string line in lines)
            {
                index++;

                if (line.Contains("Stage1") && stage1_1 == 0)
                {
                    stage1_1 = index;
                }

                if (line.Contains("Stage1") && stage1_1 != 0)
                {
                    stage1_2 = index;
                }


                if (line.Contains("Stage2"))
                {
                    stage2 = index;
                }
                if (line.Contains("Stage3"))
                {
                    stage3 = index;
                }

                //string[] shortTime = line.Split("-");
                //string rptDate = shortTime[0] + "_" + shortTime[1] + "_" + shortTime[2].Split(" ")[0];

                stageLocs = stage1_1.ToString() + "," + stage2.ToString() +
                "," + stage3.ToString() + "," + stage1_2.ToString() + "," + date;
            }
            return stageLocs;
        }

        public List<string> GetNonIRDataDefects(List<string> NonIRData)
        {
            float EOMTF = 25;
            float IRMTF = 25;

            float EOSigmaV = 25;
            float EOSigmaH = 25;
            float EOSigmaVH = 20;

            float IRSigmaV = 50;
            float IRSigmaH = 50;
            float IRSigmaVH = 20;

            List<string> NIRAdefects = new List<string>();

            foreach (string line in NonIRData)
            {
                string sn = line.Split(" ")[0].Split(",")[0];
                string date = line.Split(",")[1].Split(" ")[0];

                if (line.Contains("IR sigma_v") && !line.Contains("h"))
                {
                    float IRSv = 0;
                    bool IRSvB = float.TryParse(line.Split("=")[1], out IRSv);
                    if ((IRSv.CompareTo(IRSigmaV) > 0))
                    {
                        NIRAdefects.Add(sn + "," + "IR sigma_v" + "," + IRSv + "," + date);
                    }
                }

                if (line.Contains("IR sigma_vh"))
                {
                    float IRSvh = 0;
                    bool IRSvBh = float.TryParse(line.Split("=")[1], out IRSvh);
                    if ((IRSvh.CompareTo(IRSigmaVH) > 0))
                    {
                        NIRAdefects.Add(sn + "," + "IR sigma_vh" + "," + IRSvh + "," + date);
                    }
                }

                if (line.Contains("IR sigma_h") && !line.Contains("v"))
                {
                    float IRSh = 0;
                    bool IRShB = float.TryParse(line.Split("=")[1], out IRSh);
                    if ((IRSh.CompareTo(IRSigmaH) > 0))
                    {
                        NIRAdefects.Add(sn + "," + "IR sigma_h" + "," + IRSh + "," + date);
                    }
                }

                if (line.Contains("EO sigma_v") && !line.Contains("h"))
                {
                    float EOSv = 0;
                    bool EOSvB = float.TryParse(line.Split("=")[1], out EOSv);
                    if (EOSv.CompareTo(EOSigmaV) > 0)
                    {
                        NIRAdefects.Add(sn + "," + "EO sigma_v" + "," + EOSv + "," + date);
                    }
                }

                if (line.Contains("EO sigma_h") && !line.Contains("v"))
                {
                    float EOSh = 0;
                    bool EOShB = float.TryParse(line.Split("=")[1], out EOSh);
                    if ((EOSh.CompareTo(EOSigmaH) > 0))
                    {
                        NIRAdefects.Add(sn + "," + "EO sigma_h" + "," + EOSh + "," + date);
                    }
                }

                if (line.Contains("EO sigma_vh"))
                {
                    float EOSvh = 0;
                    bool EOSvhB = float.TryParse(line.Split("=")[1], out EOSvh);
                    if ((EOSvh.CompareTo(EOSigmaVH) > 0))
                    {
                        NIRAdefects.Add(sn + "," + "EO sigma_vh" + "," + EOSvh + "," + date);
                    }
                }

                if (line.Contains("EO MTF") && !line.Contains("Module") && !line.Contains("[Error]"))
                {
                    float EOMTFv = 0;
                    bool EOMTFB = float.TryParse(line.Split(":")[3], out EOMTFv);
                    if ((EOMTFv.CompareTo(EOMTF) < 0))
                    {
                        NIRAdefects.Add(sn + "," + "EOMTF" + "," + EOMTFv + "," + date);
                    }
                }

                if (line.Contains("IR MTF:") && !line.Contains("Calculating") && !line.Contains("Module"))
                {
                    float IRMTFv = 0;
                    bool IRMTFB = float.TryParse(line.Split(":")[3], out IRMTFv);
                    if ((IRMTFv.CompareTo(IRMTF) < 0))
                    {
                        NIRAdefects.Add(sn + "," + "IRMTF" + "," + IRMTFv + "," + date);
                    }
                }
            }

            return NIRAdefects;
        }

        public List<string> GetNonIRAccuracyData(string file)
        {
            string sn = file.Split("\\")[3].Split("_")[0];
            List<string> res = new List<string>();
            List<string> data = File.ReadAllLines(file).ToList();

            foreach (string line in data)
            {

                if (line.Contains("EO MTF"))
                {
                    res.Add(sn + "," + line);
                }

                else if (line.Contains("IR MTF"))
                {
                    res.Add(sn + "," + line);
                }

                else if (line.Contains("EO sigma_h"))
                {
                    res.Add(sn + "," + line);
                }

                else if (line.Contains("EO sigma_v"))
                {
                    res.Add(sn + "," + line);
                }

                else if (line.Contains("EO sigma_v"))
                {
                    res.Add(sn + "," + line);
                }

                else if (line.Contains("IR sigma_v"))
                {
                    res.Add(sn + "," + line);
                }

                else if (line.Contains("IR sigma_h"))
                {
                    res.Add(sn + "," + line);
                }

                else if (line.Contains(sn + "," + "IR sigma_vh"))
                {
                    res.Add(sn + "," + line);
                }

            }
            return res;
        }

        private void PickerStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void EndDateDisplay_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void StartDateDisplay_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
//    private string findBBErrors(string file, string stageLoc)
//    {
//        string[] lines = File.ReadAllLines(file);

//        string BBHighStart = stageLoc.Split(",")[0];
//        string BBMidStart = stageLoc.Split(",")[1];
//        string BBLowStart = stageLoc.Split(",")[2];
//        string BBLowEnd = stageLoc.Split(",")[3];

//        int BBH;
//        int BBM;
//        int BBL;
//        int BBLEnd;

//        int.TryParse(BBHighStart, out BBH);
//        int.TryParse(BBMidStart, out BBM);
//        int.TryParse(BBLowStart, out BBL);
//        int.TryParse(BBLowEnd, out BBLEnd);

//        bool BBHigh = false;
//        bool BBMid = false;
//        bool BBLow = false;

//        for (int i = BBH; i <= BBM; i++)
//        {
//            if (lines[i].Contains("[Error]"))
//            {
//                BBHigh = true;
//            }
//        }

//        for (int i = BBM; i <= BBL; i++)
//        {
//            if (lines[i].Contains("[Error]"))
//            {
//                BBMid = true;
//            }
//        }

//        for (int i = BBL; i <= BBLEnd; i++)
//        {
//            if (lines[i].Contains("[Error]"))
//            {
//                BBLow = true;
//            }
//        }
//        return BBHigh + "," + BBMid + "," + BBLow;
//    }


//    private string getSN(string file)
//    {
//        string sn = file.Split("\\")[3].Split("_")[0];
//        return sn;
//    }

//    private string getStageLocs(string file)
//    {
//        int index = 0;
//        string stageLocs = "";
//        int stage1_1 = 0;
//        int stage1_2 = 0;
//        int stage2 = 0;
//        int stage3 = 0;

//        string[] lines = File.ReadAllLines(file);

//        foreach (string line in lines)
//        {
//            index++;

//            if (line.Contains("Stage1") && stage1_1 == 0)
//            {
//                stage1_1 = index;
//            }

//            if (line.Contains("Stage1") && stage1_1 != 0)
//            {
//                stage1_2 = index;
//            }


//            if (line.Contains("Stage2"))
//            {
//                stage2 = index;
//            }
//            if (line.Contains("Stage3"))
//            {
//                stage3 = index;
//            }

//            //string[] shortTime = line.Split("-");
//            //string rptDate = shortTime[0] + "_" + shortTime[1] + "_" + shortTime[2].Split(" ")[0];

//            stageLocs = stage1_1.ToString() + "," + stage2.ToString() +
//            "," + stage3.ToString() + "," + stage1_2.ToString();
//        }
//        return stageLocs;
//    }
//}




//   private List<string> getNIRFailures(string file)
//   {
//       List<string> results = new List<string>();
//       string[] data = File.ReadAllLines(file);
//       //string results =  "" ;
//       foreach (string line in data)
//       {
//           string sn = getSN(file);
//           if (line.Contains("Modules failed") && !line.Contains("RadiometricAccuracyIRModule"))
//           {
//               results.Add(sn + "," + line);
//               //results += sn +  "," + line;
//           }
//       }
//       return results;
//   }

//   private string findBBErrors(string file, string stageLoc)
//   {
//       string[] lines = File.ReadAllLines(file);

//       string BBHighStart = stageLoc.Split(",")[0];
//       string BBMidStart = stageLoc.Split(",")[1];
//       string BBLowStart = stageLoc.Split(",")[2];
//       string BBLowEnd = stageLoc.Split(",")[3];

//       int BBH;
//       int BBM;
//       int BBL;
//       int BBLEnd;

//       int.TryParse(BBHighStart, out BBH);
//       int.TryParse(BBMidStart, out BBM);
//       int.TryParse(BBLowStart, out BBL);
//       int.TryParse(BBLowEnd, out BBLEnd);

//       bool BBHigh = false;
//       bool BBMid = false;
//       bool BBLow = false;

//       for (int i = BBH; i <= BBM; i++)
//       {
//           if (lines[i].Contains("[Error]"))
//           {
//               BBHigh = true;
//           }
//       }

//       for (int i = BBM; i <= BBL; i++)
//       {
//           if (lines[i].Contains("[Error]"))
//           {
//               BBMid = true;
//           }
//       }

//       for (int i = BBL; i <= BBLEnd; i++)
//       {
//           if (lines[i].Contains("[Error]"))
//           {
//               BBLow = true;
//           }
//       }
//       return BBHigh + "," + BBMid + "," + BBLow;
//   }


//   private string getSN(string file)
//   {
//       string sn = file.Split("\\")[3].Split("_")[0];
//       return sn;
//   }

//   private string getStageLocs(string file)
//   {
//       int index = 0;
//       string stageLocs = "";
//       int stage1_1 = 0;
//       int stage1_2 = 0;
//       int stage2 = 0;
//       int stage3 = 0;

//       string[] lines = File.ReadAllLines(file);

//       foreach (string line in lines)
//       {
//           index++;

//           if (line.Contains("Stage1") && stage1_1 == 0)
//           {
//               stage1_1 = index;
//           }

//           if (line.Contains("Stage1") && stage1_1 != 0)
//           {
//               stage1_2 = index;
//           }


//           if (line.Contains("Stage2"))
//           {
//               stage2 = index;
//           }
//           if (line.Contains("Stage3"))
//           {
//               stage3 = index;
//           }

//           //string[] shortTime = line.Split("-");
//           //string rptDate = shortTime[0] + "_" + shortTime[1] + "_" + shortTime[2].Split(" ")[0];

//           stageLocs = stage1_1.ToString() + "," + stage2.ToString() +
//           "," + stage3.ToString() + "," + stage1_2.ToString();
//       }
//       return stageLocs;
//   }

//   private void btn_uniqueResultsIR_Click(object sender, RoutedEventArgs e)
//   {
//       string path = @"C:\Hadron640\defect_results\defects.csv";

//       string[] results = File.ReadAllLines(path);
//       string[] output = results.Distinct().ToArray();

//       string finalResultsPath = @"C:\Hadron640\defect_results\defects_u.csv";

//       using (StreamWriter UR = new StreamWriter(finalResultsPath))
//       {
//           foreach (string line in output)
//           {
//               UR.WriteLine(line);
//           }
//       }
//   }

//   private void btn_Pareto_Click(object sender, RoutedEventArgs e)
//   {
//       string[] pc = File.ReadAllLines(@"C:\Hadron640\defect_results\defects.csv");
//       string[] d_list = File.ReadAllLines(@"C:\Hadron640\defect_results\defectList.txt");
//       StreamWriter wpd = new StreamWriter(@"C:\Hadron640\defect_results\pareto.csv");
//       foreach (string defect in d_list)
//       {
//           int i = 0;
//           foreach (string line in pc)
//           {
//               if (line.Contains(defect))
//               {
//                   i++;
//               }
//           }

//           {
//               wpd.WriteLine(defect + "," + i, true);
//           }
//       }
//       wpd.Close();
//   }

//   private void btn_LatestFiles_Click_1(object sender, RoutedEventArgs e)
//   {
//       {
//           //int index = fileList.Count;
//           List<string> fileList = Directory.GetFiles(@"C:\Hadron640\debugOnly").ToList();
//           List<string> files = new List<string>();
//           StreamWriter sw = new StreamWriter(@"C:\Hadron640\defect_results\latestFiles.csv");
//           int i = 0;
//           foreach (string file in fileList)

//           {
//               string sn = file.Split("\\")[3].Split("_")[0];
//               string comp = fileList[i + 1].Split("\\")[3].Split("_")[0];
//               i++;

//               //while (comp == sn)
//               //{
//               //    comp = fileList[i + 1].Split("\\")[3].Split("_")[0];
//               //    i++;
//               //}
//               if (comp != sn)
//               {
//                   //files.Add(file);  
//                   sw.WriteLine(file);
//               }

//           }

//       }

//   }







//private void btn_AggFailures_Click(object sender, RoutedEventArgs e)
//{
//    string[] pc = File.ReadAllLines(@"C:\Hadron640\defect_results\defects_u.csv");
//    string[] d_list = File.ReadAllLines(@"C:\Hadron640\defect_results\defectList.txt");
//    StreamWriter wpd = new StreamWriter(@"C:\Hadron640\defect_results\agg_failures.csv");
//    int finalCount = d_list.Count();
//    {

//    List<string> sns = new List<string>();
//    List<string> defects = new List<string>();
//    List<string> cmbDefects = new List<string>();

//    foreach (string line in d_list)
//    {
//        sns.Add(line.Split(",")[0]);
//        defects.Add(line.Split(",")[1]); 
//    }

//        for (int i = 0; i < finalCount; i++)                
//        {

//            string dfc = "";
//            if (i == 0)
//            { 

//                string defect = defects[i]; }



//        }

//   wpd.WriteLine
//        //wpd.WriteLine(defect + "," + i, true);
//    }
//}
////wpd.Close();



////{
//    while(i <= fileList.Count) 
//    {
//        string sn = file.Split("\\")[3].Split("_")[0];
//        string comp = fileList[i + 1].Split("\\")[3].Split("_")[0];


//        sw.WriteLine(fileList[0], true);
//        if (sn != comp)
//        {
//            sw.WriteLine(fileList[i], true);
//            i++;
//        }

//    } 
//sw.Close ();
//}
//return files;

//NIRFailures = getNIRFailures(file);

//if (NIRFailures.Count > 0)
//{
//foreach (string NIRFailure in NIRFailures)
//{
//    string[] ln = NIRFailure.Split(",");
//    if (ln.Length == 2)
//    { 
//        sw.WriteLine(sn + "," + NIRFailure.Split("-")[4], true);
//        defects.Add(sn + "," + NIRFailure.Split("-")[4]);

//}

//get IRAccuracy defects
//string stageLoc = getStageLocs(file);

//BBErrors = findBBErrors(file, stageLoc);
//string[] BBResults = BBErrors.Split(",");

//if (BBResults[0] == "True")
//{ sw.WriteLine(sn + "," + "BBHigh", true);
//    defects.Add(sn + "," + "BBHigh");
//}

//if (BBResults[1] == "True")
//{
//    sw.WriteLine(sn + "," + "BBMid", true);
//    defects.Add(sn + "," + "BBMid");
//}

//if (BBResults[2] == "True")
//{
//    sw.WriteLine(sn + "," + "BBLow", true);
//    defects.Add(sn + "," + "BBLow");
//}
//StreamWriter sw_f = new StreamWriter(resultsPath);
//foreach (string result in defects)
//{
//    sw_f.WriteLine(result, true);
//}
//sw_f.Close();

// List<string> NIRFailures = new List<string>();
/*&& !line.Contains("RadiometricAccuracyIRModule"*/
//results += sn +  "," + line;
//List<string>  fileList = Directory.GetFiles(@"C:\Hadron640\debugOnly").ToList();
//extract the sn
//string BBErrors = "";

//NIRFailures = getNIRFailures(file);

//if (NIRFailures.Count > 0)
//{
//foreach (string NIRFailure in NIRFailures)
//{
//    //string[] ln = NIRFailure.Split(",");
//    //if (ln.Length == 2)
//    //{
//        sw.WriteLine(sn + "," + NIRFailure.Split("-")[4], true);
//        defects.Add(sn + "," + NIRFailure.Split("-")[4]);


//    //}
//}
// sw.Close();

//string line;
//StreamWriter sw_f = new StreamWriter(resultsPath, true);
//using ( StreamReader sr = new StreamReader(file))
//{
//    while ((line = sr.ReadLine()) != null) 
//    {
//        defects.Add(line);
//        if (line.Contains("Modules failed"))
//        {
//            defects.Add(line);
//            sw_f.WriteLine(line);
//        }
//    }


//while (!sr.EndOfStream)
//{
//    string data = sr.ReadLine();
//    if (data.Contains("Modules failed"))
//    {
//    }
//}
//sr.Close();

//private void btn_EOFailures_Click(object sender, RoutedEventArgs e)
//{
//    List<string> fileList = Directory.GetFiles(@"C:\Hadron640\debugOnly").ToList();
//    List<string> defects = new List<string>();

//    foreach (string file in fileList)
//    {
//        string[] data = File.ReadAllLines(file);
//        foreach (string line in data)
//        {
//            if (line.Contains("EO sigma_h"))
//            {
//                float sh = 0;
//                string fail_d = line.Split("=")[1];
//                float.TryParse(fail_d, out sh);
//                if (sh > 25)
//                {
//                    defects.Add(line);
//                }
//            }

//            if (line.Contains("EO sigma_v"))
//            {
//                float sh = 0;
//                string fail_d = line.Split("=")[1];
//                float.TryParse(fail_d, out sh);
//                if (sh > 25)
//                {
//                    defects.Add(line);
//                }
//            }

//            if (line.Contains("ER sigma_vh"))
//            {
//                float sh = 0;
//                string fail_d = line.Split("=")[1];
//                float.TryParse(fail_d, out sh);
//                if (sh > 20)
//                {
//                    defects.Add(line);
//                }
//            }

//            if (line.Contains("EO MTF"))
//            {
//                string MTF = line.Split(":")[3];
//                float MTF_float = 0;
//                float.TryParse(MTF, out MTF_float);
//                if (MTF_float < 100)
//                {
//                    defects.Add(line);
//                }
//            }


//            if (line.Contains("EO relative illumination"))
//            {
//                string relIllum = line.Split("=")[1];
//                relIllum = relIllum.Replace("%", "");
//                float illum = 0;
//                float.TryParse(relIllum, out illum);
//                if (illum < 50)
//                {
//                    defects.Add(line);
//                }
//            }
//        }
//    }
//}

//List<string> data = File.ReadAllLines(file).ToList();


//foreach (string line_d in data)
//{

//    if (line_d.Contains("EO MTF"))
//    {
//        sw.WriteLine(sn + "," + line_d, true);
//    }

//    if (line_d.Contains("IR MTF"))
//    {
//        sw.WriteLine(sn + "," + line_d, true);
//    }

//    if (line_d.Contains("EO sigma_h"))
//    {
//        sw.WriteLine(sn + "," + line_d, true);
//    }

//    if (line_d.Contains("EO sigma_v"))
//    {
//        sw.WriteLine(sn + "," + line_d, true);
//    }

//    if (line_d.Contains("EO sigma_v"))
//    {
//        sw.WriteLine(line_d, true);
//    }

//    if (line_d.Contains("IR sigma_v"))
//    {
//        sw.WriteLine(sn + "," + line_d, true);
//    }

//    if (line_d.Contains("EO sigma_h"))
//    {
//        sw.WriteLine(sn + "," + line_d, true);
//    }

//    if (line_d.Contains(sn + "," + "EO sigma_vh"))
//    {
//        sw.WriteLine(line_d, true);
//    }



//    string stageLoc = getStageLocs(file);
//    string BBErrors = findBBErrors(file, stageLoc);
//    string[] BBResults = BBErrors.Split(",");

//if ((BBResults[0] == "True"))
//{
//    sw.WriteLine(sn + "," + "BBHigh", true);

//}

//if (BBResults[1] == "True")
//{
//    sw.WriteLine(sn + "," + "BBMid", true);

//}

//if (BBResults[2] == "True")
//{
//    sw.WriteLine(sn + "," + "BBLow", true);

//}
//StreamWriter sw_f = new StreamWriter(resultsPath);
//foreach (string result in defects)
//{
//    sw.WriteLine(result, true);
//}


//float EOMTF = 25;
//float IRMTF = 25;

//float EOSigmaV = 25;
//float EOSigmaH = 25;
//float EOSigmaVH = 20;

//float IRSigmaV = 50;
//float IRSigmaH = 50;
//float IRSigmaVH = 20;

//float BBLowMax = 0.0f;

//    if (comp!=sn && (present.CompareTo(next)) > 0)
//        {
//        //files.Add(file);
//        string latestFile = fileList[i];
//        sw.WriteLine(latestFile);
//        i++;

//    }


//while (comp == sn)
//{
//    comp = fileList[i + 1].Split("\\")[5].Split("_")[0];
//    i++;

//}

//string sub4Folder = Directory.GetDirectories(sub3Folder).Last();
//string[] desc = sub4Folder.Split('\\');
//string origPath = (sub4Folder + "\\" + "debug.txt");                    
//string finalPath= (dbFinalFolder + desc[9] + "_" + desc[10] + "_" + "debug.txt");

//string debugFileOnlyPath = dbFinalFolder + desc[9] + "_" + desc[10] + "_" + "debug.txt";
//File.Copy(origPath, finalPath, true);


//List<string> finalPaths = new List<string>();
//foreach (string sub5Folder in sub4Folders)
//{
//    finalPaths.Add(sub5Folder + "\\" + "debug.txt");
//}

//foreach (string origPath in finalPaths)
//{
//    string[] desc = origPath.Split('\\');
//    string debugFileOnlyPath = dbFinalFolder + desc[9] + "_" + desc[10] + "_" + "debug.txt";
//    File.Copy(origPath, debugFileOnlyPath, true);
//}

//while (i < (fileList.Length - 1))
//{ 
//    if (comp == sn)
//    {
//        i++;
//        comp = fileList[i + 1].Split("\\")[3].Split("_")[0];                        
//    }

//    if (comp != sn)
//    {
//        sw.WriteLine(fileList[i]);
//        sn = fileList[i].Split("\\")[5].Split("_")[0];
//        i++;
//    }
//}

//comp = fileList[i].Split("\\")[5].Split("_")[0];
//i++;

//if (comp != sn)
//{
//    //files.Add(file);  
//    sw.WriteLine(fileList[i]);
//    i++;

//}

//private async void btn_LatestFiles_Click(object sender, RoutedEventArgs e)
//{
//    //int index = fileList.Count;


//    string[] fileList = Directory.GetFiles(@"C:\\Hadron640\\debugOnly");
//    List<string> files = new List<string>();
//    StreamWriter sw = new StreamWriter(@"C:\Hadron640\defect_results\latestFiles.csv");
//    int i = 0;
//    int present = 0;
//    int next = 0;
//    //foreach (string file in fileList)
//    while (i < fileList.Length - 1 )
//    {
//        //Task.Delay(100);

//        //string presentIndex = "";
//        //string nextIndex = "";


//        string presentIndex = fileList[i].Split('_')[1];
//        string nextIndex = fileList[i + 1].Split('_')[1];
//        bool presentB = int.TryParse(presentIndex, out present);
//        bool nextB = int.TryParse(nextIndex, out next);

//        if (present == next )
//        {
//            //string current = fileList[i];
//            sw.WriteLine(fileList[i]);

//        }

//        if (present > next)

//        {
//            string current = fileList[i];
//            sw.WriteLine(fileList[i]); }
//        i++;
//    }
//    sw.Close();

//}






//foreach (string file in fileList)
//{ 
//List<string> rawData = new List<string>();
//    rawData = File.ReadAllLines(file).ToList();
//    foreach (string line in rawData)
//    {
//        string sn = file.Split("\\")[3].Split("_")[0];
//        if (line.Contains("EO MTF:"))
//        {
//            string date = line.Split(" ")[0];
//            string data = line.Split(":")[3];
//            sw.WriteLine(sn + "," + data + "," + date);
//        }

//        if (line.Contains("IR MTF:"))
//        {
//            string date = line.Split(" ")[0];
//            string data = line.Split(":")[3];
//            sw.WriteLine(sn + "," + data + "," + date);
//        }
//    }

//List<string> fileList = File.ReadAllLines(@"C:\Hadron640\defect_results\latestFiles.txt").ToList();
//List<string> fileList = File.ReadAllLines(@"C:\Hadron640\defect_results\latestFiles.txt").ToList();
//using System.Net.Mail;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Reflection.Metadata;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using System.Threading.Tasks;

//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

//using System.Windows.Controls.Primitives;
//using System.Windows.Media.Effects;
//using System.Windows.Shapes;
//using System.Windows.Xps.Serialization;

//using Microsoft.VisualBasic;
//using System;
//using Microsoft.VisualBasic;

//using System.Collections.Specialized;
//using System.Globalization;

//using System.IO.Enumeration;
//using System.Net.Mail;
//using System.Security.Cryptography.X509Certificates;








