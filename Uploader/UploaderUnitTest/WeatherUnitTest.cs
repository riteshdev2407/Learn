using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUploader;

namespace UploaderUnitTest
{
    [TestClass]
    public class WeatherUnitTest
    {
        

        [TestMethod]
        public void TestGetCityID()
        {
            string temp = "12345=Mumbai";
            Form1 testobj = new Form1();
            string result = testobj.GetCityId(temp);
            Assert.AreEqual(result, "12345");
        }

        [TestMethod]
        public void TestCallRestMethod()
        {
            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?id=");
            string data = "2172797";
            string outputpath = @"C:\\Install";
            Form1 testobj = new Form1();
            Boolean result = testobj.CallRestMethod(url,data,outputpath);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void TestNegativeCallRestMethod()
        {
            string wrongurl = string.Format("http://api.openweathermap.org/data/2.5/weather?q=");
            string data = "2172797";
            string outputpath = @"C:\\Install";
            Form1 testobj = new Form1();
            Boolean result = testobj.CallRestMethod(wrongurl, data, outputpath);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void TestGenerateWeatherFile()
        {
            string cityname = "London";
            string cityweather = "cloudy";
            string outputpath = @"C:\\Install";
            Form1 testobj = new Form1();
            Boolean result = testobj.GenerateWeatherFile(cityname, cityweather, outputpath);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void TestMegativeGenerateWeatherFile()
        {
            string cityname = "London";
            string cityweather = "cloudy";
            string wrongoutputpath = "";
            Form1 testobj = new Form1();
            Boolean result = testobj.GenerateWeatherFile(cityname, cityweather, wrongoutputpath);
            Assert.AreEqual(result, false);
        }
    }
}
