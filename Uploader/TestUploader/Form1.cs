using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Uploader;
using Newtonsoft.Json;

namespace TestUploader
{
    /// <summary>
    /// A test form used to upload a file from a windows application using
    /// the Uploader Web Service
    /// </summary>
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // do nothing
        }


        /// <summary>
        /// Upload any file to the web service; this function may be
        /// used in any application where it is necessary to upload
        /// a file through a web service
        /// </summary>
        /// <param name="filename">Pass the file path to upload</param>
        public void UploadFile(string filename)
        {
            try
            {
                // get the exact file name from the path
                String strFile = System.IO.Path.GetFileName(filename);

                // create an instance fo the web service
                TestUploader.Uploader.FileUploader srv = new TestUploader.Uploader.FileUploader();

                // get the file information form the selected file
                FileInfo fInfo = new FileInfo(filename);

                // get the length of the file to see if it is possible
                // to upload it (with the standard 4 MB limit)
                long numBytes = fInfo.Length;
                double dLen = Convert.ToDouble(fInfo.Length / 1000000);

                // Default limit of 4 MB on web server
                // have to change the web.config to if
                // you want to allow larger uploads
                if (dLen < 4)
                {
                    // set up a file stream and binary reader for the 
                    // selected file
                    FileStream fStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fStream);

                    // convert the file to a byte array
                    byte[] data = br.ReadBytes((int)numBytes);
                    br.Close();

                    // pass the byte array (file) and file name to the web service
                    string sTmp = srv.UploadFile(data, strFile);
                    fStream.Close();
                    fStream.Dispose();
                   
                    // this will always say OK unless an error occurs,
                    // if an error occurs, the service returns the error message
                    MessageBox.Show("File Upload Status: " + sTmp, "File Upload");
                    Boolean success= ReportingWeather(fInfo.FullName, fInfo.DirectoryName);
                    if (success)
                    {
                        MessageBox.Show("Output Files are generated Successfully");

                    }
                    else
                        MessageBox.Show("Output files are not genrated. Please contact your Administrator");
                }
                else
                {
                    // Display message if the file was too large to upload
                    MessageBox.Show("The file selected exceeds the size limit for uploads.", "File Size");
                }
            }
            catch (Exception ex)
            {
                // display an error message to the user
                MessageBox.Show(ex.Message.ToString(), "Upload Error");
            }
        }

        public Boolean ReportingWeather(string path, string outputdirectory)
        {
            try {

                string url = string.Format("http://api.openweathermap.org/data/2.5/weather?id=");

                if (File.Exists(path))
                {
                    // Read a text file line by line.
                    string[] lines = File.ReadAllLines(path);
                    foreach (string line in lines)
                        CallRestMethod(url, line, outputdirectory);
                }
               
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "ReportingWeather");

            }
            return true;
        }



        public  string GetCityId(string data)
        {

            string[] tokens = data.Split('=');
            return tokens[0];
        }
        public  Boolean CallRestMethod(string url, string data, string Outputpath)
        {
            string result = string.Empty;
            Boolean isSuccess = false;
            try
            {
                string parameter = GetCityId(data);
                url += parameter;
                HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
                webrequest.Method = "GET";
                webrequest.ContentType = "application/Json";
                webrequest.Headers.Add("x-api-key", "aa69195559bd4f88d79f9aadeb77a8f6");

                HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
                StreamReader responseStream = new StreamReader(webresponse.GetResponseStream());
                
                result = responseStream.ReadToEnd();


                RootObject Weatherresponse = JsonConvert.DeserializeObject<RootObject>(result);

                webresponse.Close();

                foreach (Weather Cityweather in Weatherresponse.weather)
                {
                    //Use any text you want to save in the file.
                    string temp = "The weather in " + Weatherresponse.name + "  is  " + Cityweather.description;
                     isSuccess= GenerateWeatherFile(Weatherresponse.name,temp, Outputpath);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "CallRestMethod");
                return false;
            }
            return isSuccess;
        } 

        public Boolean GenerateWeatherFile(string CityName, string CityWeather, string OutputPath)
        {
            try
            {
                string format = "Mddyyyyhhmmsstt";
                string _datetime = String.Format(DateTime.Now.ToString(format));
                string _filename = CityName + _datetime + ".txt";
                string fullPath = OutputPath + "\\" + _filename;

                using (StreamWriter writer = new StreamWriter(fullPath))
                {
                    writer.WriteLine(CityWeather);

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "GenerateWeatherFile");
                return false;
            }
            return true;

        }

        /// <summary>
        /// Allow the user to browse for a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open File";
            openFileDialog1.Filter = "All Files|*.*";
            openFileDialog1.FileName = "";

            try
            {
                openFileDialog1.InitialDirectory = "C:\\Temp";
            }
            catch
            {
                // skip it 
            }

            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName == "")
                return;
            else
                txtFileName.Text = openFileDialog1.FileName;

        }



        /// <summary>
        /// If the user has selected a file, send it to the upload method, 
        /// the upload method will convert the file to a byte array and
        /// send it through the web service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (txtFileName.Text != string.Empty)
                UploadFile(txtFileName.Text);
            else
                MessageBox.Show("You must select a file first.", "No File Selected");
        }
    }
}