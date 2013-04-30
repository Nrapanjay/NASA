using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.Mvc;

using Sofa.Models;
using Sofa.Shared;
using System.IO;

namespace Sofa.Controllers
{
    public class HomeController : Controller
    {
        #region index page methods

        public ActionResult Index()
        {
            //Array ACF's loading
            GV.s2 = Server.MapPath("~/App_Data/CF.txt");
            GV.sw = System.IO.File.OpenText(GV.s2);
            GV.s2 = GV.sw.ReadLine();
            GV.cf = Convert.ToInt16(GV.s2);//number of cities in a list
            for (GV.i1 = 0; GV.i1 <= (GV.cf - 1); GV.i1++)
            {
                GV.acf[GV.i1, 0] = GV.sw.ReadLine();//city's name
                GV.acf[GV.i1, 1] = GV.sw.ReadLine();//1st argument
                GV.acf[GV.i1, 2] = GV.sw.ReadLine();//1st argument's delay
                GV.acf[GV.i1, 3] = GV.sw.ReadLine();//2nd argument
                GV.acf[GV.i1, 4] = GV.sw.ReadLine();//2nd argument's delay
                GV.acf[GV.i1, 5] = GV.sw.ReadLine();//3rd argument
                GV.acf[GV.i1, 6] = GV.sw.ReadLine();//3rd argument's delay
                GV.acf[GV.i1, 7] = GV.sw.ReadLine();//maximum date
                GV.acf[GV.i1, 8] = GV.sw.ReadLine();//k0
                GV.acf[GV.i1, 9] = GV.sw.ReadLine();//k1
                GV.acf[GV.i1, 10] = GV.sw.ReadLine();//k2
                GV.acf[GV.i1, 11] = GV.sw.ReadLine();//k3
                GV.acf[GV.i1, 12] = GV.sw.ReadLine();//calculated average quality of the forecast
            }
            GV.sw.Close();
            GV.sw.Dispose();

            this.CreateInitialXmlDataFile();

            return View();
        }

        public JsonResult GetPlaces()
        {
            List<string> places = new List<string>();
            for (int i = 0; i < GV.cf; i++)
                places.Add(GV.acf[i, 0]);

            return Json(places, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInitialDates(int selectedPlace)
        {
            List<string> dates = new List<string>();
            string tempFinalDateString = GV.acf[selectedPlace, 7];
            string tempYear = tempFinalDateString.Substring(0, 4);
            string tempMonth = tempFinalDateString.Substring(4, 2);
            string tempDay = tempFinalDateString.Substring(6, 2);

            tempFinalDateString = tempYear + "/" + tempMonth + "/" + tempDay;
            dates.Add(tempFinalDateString);
            DateTime finalDate = new DateTime(int.Parse(tempYear), int.Parse(tempMonth), int.Parse(tempDay));
            while (finalDate.Date != DateTime.Now.Date)
            {
                finalDate = finalDate.Date.Subtract(TimeSpan.FromDays(1));

                tempYear = finalDate.Year.ToString();
                tempMonth = finalDate.Month < 10 ? "0" + finalDate.Month : finalDate.Month.ToString();
                tempDay = finalDate.Day < 10 ? "0" + finalDate.Day : finalDate.Day.ToString();

                string currentDayString = tempYear + "/" + tempMonth + "/" + tempDay;
                dates.Add(currentDayString);
            }

            dates.Reverse();
            return Json(dates, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFinalDate(int selectedPlace)
        {
            string tempFinalDateString = GV.acf[selectedPlace, 7];
            string tempYear = tempFinalDateString.Substring(0, 4);
            string tempMonth = tempFinalDateString.Substring(4, 2);
            string tempDay = tempFinalDateString.Substring(6, 2);
            tempFinalDateString = tempYear + "/" + tempMonth + "/" + tempDay;
            return Json(tempFinalDateString, JsonRequestBehavior.AllowGet);
        }

        public void CreateInitialXmlDataFile()
        {
            StreamWriter swf;
            GV.d1 = Convert.ToInt16(GV.acf[GV.si, 2]);//1st argument's delay
            GV.d2 = Convert.ToInt16(GV.acf[GV.si, 4]);//2nd argument's delay
            GV.d3 = Convert.ToInt16(GV.acf[GV.si, 6]);//3rd argument's delay
            GV.k0 = Convert.ToDouble(GV.acf[GV.si, 8]);//k0
            GV.k1 = Convert.ToDouble(GV.acf[GV.si, 9]);//k1
            GV.k2 = Convert.ToDouble(GV.acf[GV.si, 10]);//k2
            GV.k3 = Convert.ToDouble(GV.acf[GV.si, 11]);//k3

            //now, we create .xml-file for the chart
            GV.i1f = 1;
            do
            {
                try
                {
                    GV.chrt = GV.chrt + 1;
                    if (GV.chrt > 200) { GV.chrt = 2; }
                    GV.s2c = Server.MapPath("~/App_Data/Charts/ChartEn.xml");
                    swf = System.IO.File.CreateText(GV.s2c);
                    swf.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                    swf.WriteLine("<WeatherForecast>");
                    GV.s2 = Server.MapPath("~/App_Data/TemperatureFiles/f" + GV.acf[GV.si, 1] + ".txt");
                    GV.f1f = System.IO.File.OpenText(GV.s2);
                    GV.s2 = Server.MapPath("~/App_Data/TemperatureFiles/f" + GV.acf[GV.si, 3] + ".txt");
                    GV.f2f = System.IO.File.OpenText(GV.s2);
                    GV.s2 = Server.MapPath("~/App_Data/TemperatureFiles/f" + GV.acf[GV.si, 5] + ".txt");
                    GV.f3f = System.IO.File.OpenText(GV.s2);
                    //LOAD DATES
                    //Инициалазация начальной даты долгосрочного прогноза
                    GV.s1 = Convert.ToString(DateTime.Now.Year);
                    if (DateTime.Now.Month < 10) GV.s1 = GV.s1 + '0';
                    GV.s1 = GV.s1 + DateTime.Now.Month;
                    if (DateTime.Now.Day < 10) GV.s1 = GV.s1 + '0';
                    GV.s1 = GV.s1 + DateTime.Now.Day;
                    GV.s2 = Server.MapPath("~/App_Data/date.txt");
                    GV.sw = System.IO.File.OpenText(GV.s2);
                    GV.i3 = 0; // we havn't date's match yet
                    GV.i1 = 0; //index of the array
                    GV.i2 = -1;//quantity of elements in DropDownLIst6
                    do
                    {
                        GV.i1 = GV.i1 + 1;
                        GV.s2 = GV.sw.ReadLine();
                        GV.s3 = GV.f1f.ReadLine();
                        GV.x1[GV.i1] = Convert.ToDouble(GV.s3);
                        GV.s3 = GV.f2f.ReadLine();
                        GV.x2[GV.i1] = Convert.ToDouble(GV.s3);
                        GV.s3 = GV.f3f.ReadLine();
                        GV.x3[GV.i1] = Convert.ToDouble(GV.s3);
                        if (GV.s1 == GV.s2) { GV.i3 = 1; } // we have date's match now
                        if (GV.i3 == 1)
                        {
//                            DropDownList5.Items.Add(GV.s2); DropDownList6.Items.Add(GV.s2);
                            GV.p1 = GV.k0 + GV.k1 * GV.x1[GV.i1 - GV.d1] + GV.k2 * GV.x2[GV.i1 - GV.d2] + GV.k3 * GV.x3[GV.i1 - GV.d3];
//                            if (DropDownList7.SelectedItem.Text == "Celsius") { GV.p1 = (GV.p1 - 32) * 5 / 9; }
                            swf.WriteLine("<Chart ax=\"" + GV.s2 + "\" ay=\"" + Convert.ToString(GV.p1) + "\"/>");
                            GV.i2 = GV.i2 + 1;
                        }
                    } while ((!GV.sw.EndOfStream) && (GV.acf[GV.si, 7] != GV.s2)); // we have the end of date now
                    GV.sw.Close();
                    GV.sw.Dispose();
                    swf.WriteLine("</WeatherForecast>");
                    swf.Flush();
                    swf.Close();
                    swf.Dispose();
                    GV.f1f.Close();
                    GV.f1f.Dispose();
                    GV.f2f.Close();
                    GV.f2f.Dispose();
                    GV.f3f.Close();
                    GV.f3f.Dispose();
                    GV.i1f = 0;
                }
                catch (Exception err) { }
            } while (GV.i1f == 1);
        }

        public void CreateXmlDataFileAfterSelectingPlace(int selectedPlace)
        {
            StreamWriter swf;
            GV.si = selectedPlace;
            GV.d1 = Convert.ToInt16(GV.acf[GV.si, 2]);//1st argument's delay
            GV.d2 = Convert.ToInt16(GV.acf[GV.si, 4]);//2nd argument's delay
            GV.d3 = Convert.ToInt16(GV.acf[GV.si, 6]);//3rd argument's delay
            GV.k0 = Convert.ToDouble(GV.acf[GV.si, 8]);//k0
            GV.k1 = Convert.ToDouble(GV.acf[GV.si, 9]);//k1
            GV.k2 = Convert.ToDouble(GV.acf[GV.si, 10]);//k2
            GV.k3 = Convert.ToDouble(GV.acf[GV.si, 11]);//k3

            //now, we create .xml-file for the chart
            GV.i1f = 1;
            do
            {
                try
                {
                    GV.chrt = GV.chrt + 1;
                    if (GV.chrt > 200) { GV.chrt = 2; }
                    GV.s2c = Server.MapPath("~/App_Data/Charts/ChartEn.xml");
                    swf = System.IO.File.CreateText(GV.s2c);
                    swf.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                    swf.WriteLine("<WeatherForecast>");
                    GV.s2 = Server.MapPath("~/App_Data/TemperatureFiles/f" + GV.acf[GV.si, 1] + ".txt");
                    GV.f1f = System.IO.File.OpenText(GV.s2);
                    GV.s2 = Server.MapPath("~/App_Data/TemperatureFiles/f" + GV.acf[GV.si, 3] + ".txt");
                    GV.f2f = System.IO.File.OpenText(GV.s2);
                    GV.s2 = Server.MapPath("~/App_Data/TemperatureFiles/f" + GV.acf[GV.si, 5] + ".txt");
                    GV.f3f = System.IO.File.OpenText(GV.s2);
                    //LOAD DATES
                    //Инициалазация начальной даты долгосрочного прогноза
                    GV.s1 = Convert.ToString(DateTime.Now.Year);
                    if (DateTime.Now.Month < 10) GV.s1 = GV.s1 + '0';
                    GV.s1 = GV.s1 + DateTime.Now.Month;
                    if (DateTime.Now.Day < 10) GV.s1 = GV.s1 + '0';
                    GV.s1 = GV.s1 + DateTime.Now.Day;
                    GV.s2 = Server.MapPath("~/App_Data/date.txt");
                    GV.sw = System.IO.File.OpenText(GV.s2);
                    GV.i3 = 0; // we havn't date's match yet
                    GV.i1 = 0; //index of the array
                    GV.i2 = -1;//quantity of elements in DropDownLIst6
                    do
                    {
                        GV.i1 = GV.i1 + 1;
                        GV.s2 = GV.sw.ReadLine();
                        GV.s3 = GV.f1f.ReadLine();
                        GV.x1[GV.i1] = Convert.ToDouble(GV.s3);
                        GV.s3 = GV.f2f.ReadLine();
                        GV.x2[GV.i1] = Convert.ToDouble(GV.s3);
                        GV.s3 = GV.f3f.ReadLine();
                        GV.x3[GV.i1] = Convert.ToDouble(GV.s3);
                        if (GV.s1 == GV.s2) { GV.i3 = 1; } // we have date's match now
                        if (GV.i3 == 1)
                        {
                            //                            DropDownList5.Items.Add(GV.s2); DropDownList6.Items.Add(GV.s2);
                            GV.p1 = GV.k0 + GV.k1 * GV.x1[GV.i1 - GV.d1] + GV.k2 * GV.x2[GV.i1 - GV.d2] + GV.k3 * GV.x3[GV.i1 - GV.d3];
                            //                            if (DropDownList7.SelectedItem.Text == "Celsius") { GV.p1 = (GV.p1 - 32) * 5 / 9; }
                            swf.WriteLine("<Chart ax=\"" + GV.s2 + "\" ay=\"" + Convert.ToString(GV.p1) + "\"/>");
                            GV.i2 = GV.i2 + 1;
                        }
                    } while ((!GV.sw.EndOfStream) && (GV.acf[GV.si, 7] != GV.s2)); // we have the end of date now
                    GV.sw.Close();
                    GV.sw.Dispose();
                    swf.WriteLine("</WeatherForecast>");
                    swf.Flush();
                    swf.Close();
                    swf.Dispose();
                    GV.f1f.Close();
                    GV.f1f.Dispose();
                    GV.f2f.Close();
                    GV.f2f.Dispose();
                    GV.f3f.Close();
                    GV.f3f.Dispose();
                    GV.i1f = 0;
                }
                catch (Exception err) { }
            } while (GV.i1f == 1);
        }

        public void CreateXmlDataFileAfterSelectingDates(int selectedPlace, string initialDate, string finalDate)
        {
            if(initialDate != null)
                initialDate = initialDate.Replace("/", "");
            if (finalDate != null)
                finalDate = finalDate.Replace("/", "");

            StreamWriter swf;
            GV.si = selectedPlace;
            GV.d1 = Convert.ToInt16(GV.acf[GV.si, 2]);//1st argument's delay
            GV.d2 = Convert.ToInt16(GV.acf[GV.si, 4]);//2nd argument's delay
            GV.d3 = Convert.ToInt16(GV.acf[GV.si, 6]);//3rd argument's delay
            GV.k0 = Convert.ToDouble(GV.acf[GV.si, 8]);//k0
            GV.k1 = Convert.ToDouble(GV.acf[GV.si, 9]);//k1
            GV.k2 = Convert.ToDouble(GV.acf[GV.si, 10]);//k2
            GV.k3 = Convert.ToDouble(GV.acf[GV.si, 11]);//k3

            //now, we create .xml-file for the chart
            GV.i1f = 1;
            do
            {
                try
                {
                    GV.chrt = GV.chrt + 1;
                    if (GV.chrt > 200) { GV.chrt = 2; }
                    GV.s2c = Server.MapPath("~/App_Data/Charts/ChartEn.xml");
                    swf = System.IO.File.CreateText(GV.s2c);
                    swf.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                    swf.WriteLine("<WeatherForecast>");
                    GV.s2 = Server.MapPath("~/App_Data/TemperatureFiles/f" + GV.acf[GV.si, 1] + ".txt");
                    GV.f1f = System.IO.File.OpenText(GV.s2);
                    GV.s2 = Server.MapPath("~/App_Data/TemperatureFiles/f" + GV.acf[GV.si, 3] + ".txt");
                    GV.f2f = System.IO.File.OpenText(GV.s2);
                    GV.s2 = Server.MapPath("~/App_Data/TemperatureFiles/f" + GV.acf[GV.si, 5] + ".txt");
                    GV.f3f = System.IO.File.OpenText(GV.s2);
                    //LOAD DATES
                    //Считывание начальной и конечной дат долгосрочного прогноза
                    GV.s1 = initialDate;//date of the begin
                    GV.s4 = finalDate;//date of the end
                    GV.sw = System.IO.File.OpenText(Server.MapPath("~/App_Data/date.txt"));
                    GV.i3 = 0; // we havn't date's match yet
                    GV.i1 = 0; //index of the array
                    do
                    {
                        GV.i1 = GV.i1 + 1;
                        GV.s2 = GV.sw.ReadLine();
                        GV.s3 = GV.f1f.ReadLine();
                        GV.x1[GV.i1] = Convert.ToDouble(GV.s3);
                        GV.s3 = GV.f2f.ReadLine();
                        GV.x2[GV.i1] = Convert.ToDouble(GV.s3);
                        GV.s3 = GV.f3f.ReadLine();
                        GV.x3[GV.i1] = Convert.ToDouble(GV.s3);
                        if (GV.s1 == GV.s2) { GV.i3 = 1; } // we have date's match now
                        if (GV.i3 == 1)
                        {
                            //                            DropDownList5.Items.Add(GV.s2); DropDownList6.Items.Add(GV.s2);
                            GV.p1 = GV.k0 + GV.k1 * GV.x1[GV.i1 - GV.d1] + GV.k2 * GV.x2[GV.i1 - GV.d2] + GV.k3 * GV.x3[GV.i1 - GV.d3];
                            //                            if (DropDownList7.SelectedItem.Text == "Celsius") { GV.p1 = (GV.p1 - 32) * 5 / 9; }
                            swf.WriteLine("<Chart ax=\"" + GV.s2 + "\" ay=\"" + Convert.ToString(GV.p1) + "\"/>");
                            GV.i2 = GV.i2 + 1;
                        }
                    } while ((!GV.sw.EndOfStream) && (GV.acf[GV.si, 7] != GV.s2)); // we have the end of date now
                    GV.sw.Close();
                    GV.sw.Dispose();
                    swf.WriteLine("</WeatherForecast>");
                    swf.Flush();
                    swf.Close();
                    swf.Dispose();
                    GV.f1f.Close();
                    GV.f1f.Dispose();
                    GV.f2f.Close();
                    GV.f2f.Dispose();
                    GV.f3f.Close();
                    GV.f3f.Dispose();
                    GV.i1f = 0;
                }
                catch (Exception err) { }
            } while (GV.i1f == 1);
        }

        public JsonResult GetChartData()
        {
            XDocument chartDataDocument = XDocument.Load(Server.MapPath("~/App_Data/Charts/ChartEn.xml"));
            List<XElement> chartPoints = chartDataDocument.Element("WeatherForecast").Elements().ToList();

            List<ChartDataModel> chartData = new List<ChartDataModel>();
            foreach (XElement element in chartPoints)
            {
                ChartDataModel model = new ChartDataModel
                {
                    Ax = element.Attribute("ax").Value,
                    Ay = double.Parse(element.Attribute("ay").Value)
                };

                chartData.Add(model);
            }

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region city correlations page methods

        public JsonResult GetCityData(string city)
        {
            XDocument dataDocument = XDocument.Load(Server.MapPath("~/App_Data/CityData.xml"));
            List<XElement> dataElements = dataDocument.Element("SummaryData").Elements().ToList();
            XElement desiredDataElement = dataElements.Where(element => element.Element("Place").Value == city).FirstOrDefault();

            CityData desiredCityData = null;

            if (desiredDataElement != null)
            {
                    desiredCityData = new CityData
                {
                    Place = desiredDataElement.Element("Place").Value,
                    OneCellOne = desiredDataElement.Element("one").Element("cellOne").Value,
                    OneCellTwo = desiredDataElement.Element("one").Element("cellTwo").Value,
                    TwoCellOne = desiredDataElement.Element("two").Element("cellOne").Value,
                    TwoCellTwo = desiredDataElement.Element("two").Element("cellTwo").Value,
                    ThreeCellOne = desiredDataElement.Element("three").Element("cellOne").Value,
                    ThreeCellTwo = desiredDataElement.Element("three").Element("cellTwo").Value
                };
            }

            return Json(desiredCityData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataAfterSelectingTypeOfDegrees(string degrees)
        {
            if (degrees == "Fahrenheit")
                return this.GetChartData();
            else
            {
                XDocument chartDataDocument = XDocument.Load(Server.MapPath("~/App_Data/Charts/ChartEn.xml"));
                List<XElement> chartPoints = chartDataDocument.Element("WeatherForecast").Elements().ToList();

                List<ChartDataModel> chartData = new List<ChartDataModel>();
                foreach (XElement element in chartPoints)
                {
                    ChartDataModel model = new ChartDataModel
                    {
                        Ax = element.Attribute("ax").Value,
                        Ay = (double.Parse(element.Attribute("ay").Value) - 32)*(5.0/9.0)
                    };

                    chartData.Add(model);
                }

                return Json(chartData, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}
