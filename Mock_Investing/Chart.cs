using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
namespace Mock_Investing
{
    
    public partial class Chart : Form
    {
        List<Candle> coin_candle;
        string coinName = "";
        Series chartSeries;
        public Chart(string coinName)
        {
            InitializeComponent();
            this.coinName = coinName;
            fetchcandle();
            chartSeries = chart1.Series["Series1"];
            chart1.Series["Series1"]["PriceUpColor"] = "Red";
            chart1.Series["Series1"]["PriceDownColor"] = "Blue";
            int maxViewY = (int)coin_candle.ElementAt(0).high_price;
            int minViewY = (int)coin_candle.ElementAt(0).low_price;
            for (int i = 0; i < coin_candle.Count; i++)
            {
                chart1.Series["Series1"].Points.AddXY(coin_candle.ElementAt(i).candle_date_time_kst, coin_candle.ElementAt(i).high_price);
                chart1.Series["Series1"].Points[i].YValues[1] = coin_candle.ElementAt(i).low_price;
                chart1.Series["Series1"].Points[i].YValues[2] = coin_candle.ElementAt(i).opening_price;
                chart1.Series["Series1"].Points[i].YValues[3] = coin_candle.ElementAt(i).trade_price;
                if (maxViewY < (int)coin_candle.ElementAt(i).high_price) { maxViewY = (int)coin_candle.ElementAt(i).high_price; }
                if (minViewY > (int)coin_candle.ElementAt(i).low_price) { minViewY = (int)coin_candle.ElementAt(i).low_price; }
            }
            chart1.ChartAreas[0].AxisY.Maximum = maxViewY * 1.1;
            chart1.ChartAreas[0].AxisY.Minimum = minViewY * 0.9;
           
            //chart1.AxisViewChanged += chart1_AxisViewChanged;
            chart1.MouseWheel += mouseWheel;
        }

        public void fetchcandle()
        {
            string urlMarket = "https://api.upbit.com/v1/candles/minutes/1";
            StringBuilder dataParams = new StringBuilder();
            dataParams.Append("market=" + coinName + "&" + "count=30");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlMarket + "?" + dataParams);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //인코딩
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            var text = reader.ReadToEnd();

            //Native Object 생성
            JsonDocumentOptions jsonDocumentOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };
            JsonDocument jsonDocument = JsonDocument.Parse(text);

            //역직렬화
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true // 데이터 후행의 쉼표 허용 여부
            };

            coin_candle = JsonSerializer.Deserialize<List<Candle>>(text);

            response.Close();
            stream.Close();
            reader.Close();
        }

        public class Candle
        {
            [JsonInclude]
            public string market { get; set; }

            [JsonInclude]
            public string candle_date_time_kst { get; set; }

            [JsonInclude]
            public double opening_price { get; set; }

            [JsonInclude]
            public double high_price { get; set; }

            [JsonInclude]
            public double low_price { get; set; }

            [JsonInclude]
            public double trade_price { get; set; }

            [JsonInclude]
            public double candle_acc_trade_volume { get; set; }


        }
        private void mouseWheel(object sender, MouseEventArgs e)
        {

            if (sender.Equals(chart1))
            {
                var xAxis = chart1.ChartAreas[0].AxisX;
                var yAxis = chart1.ChartAreas[0].AxisY;
                int miny = (int)yAxis.ScaleView.ViewMinimum;
                int maxy = (int)yAxis.ScaleView.ViewMaximum;
                int minx = (int)xAxis.ScaleView.ViewMinimum;
                int maxx = (int)xAxis.ScaleView.ViewMaximum;

                chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
                chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
                chart1.ChartAreas[0].AxisY.ScrollBar.Enabled = false;
                int x = (int)xAxis.PixelPositionToValue(e.Location.X);

                if (e.Delta < 0) // 휠 아래
                {
                    int start = x - (x - minx) / 2;
                    int end = x + (maxx - x) / 2;
                    for (int i = start - 1; i < end; i++)
                    {
                        if (i >= coin_candle.Count)
                            break;
                        if (i < 0)
                            i = 0;
                        if (coin_candle.ElementAt(i).high_price > maxy)
                            maxy = (int)coin_candle.ElementAt(i).high_price;
                        if (coin_candle.ElementAt(i).low_price < miny) 
                            miny = (int)coin_candle.ElementAt(i).low_price;
                    }
                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(start, end);
                    chart1.ChartAreas[0].AxisY.ScaleView.Zoom(miny,maxy);

                    chart1.ChartAreas[0].AxisY.Maximum = maxy;
                    chart1.ChartAreas[0].AxisY.Minimum = miny;
                }
                else //휠 위로
                {
                    int start = x - (x - minx) * 2;
                    int end = x + (maxx - x) * 2;
                    for (int i = start - 1; i < end; i++)
                    {
                        if (i >= coin_candle.Count)
                            break;
                        if (i < 0)
                            i = 0;
                        if (coin_candle.ElementAt(i).high_price > maxy)
                            maxy = (int)coin_candle.ElementAt(i).high_price;
                        if (coin_candle.ElementAt(i).low_price < miny)
                            miny = (int)coin_candle.ElementAt(i).low_price;
                    }
                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(start, end);
                    chart1.ChartAreas[0].AxisY.ScaleView.Zoom(miny, maxy);
                    this.chart1.ChartAreas[0].AxisY.Maximum = maxy;
                    this.chart1.ChartAreas[0].AxisY.Minimum = miny;
                }
                
            }
            chart1.AxisViewChanged += chart1_AxisViewChanged;
        }
        
        private void chart1_AxisViewChanged(object sender, ViewEventArgs e)
        {
            chart1.MouseWheel += mouseWheel ;
            if (sender.Equals(chart1))
            {
                int start = (int)e.Axis.ScaleView.ViewMinimum;
                int end = (int)e.Axis.ScaleView.ViewMaximum;

                int max = (int)e.ChartArea.AxisY.ScaleView.ViewMinimum;
                int min = (int)e.ChartArea.AxisY.ScaleView.ViewMaximum;

                for (int i = start - 1; i < end; i++)
                {
                    if (i >= coin_candle.Count)
                        break;
                    if (i < 0)
                        i = 0;

                    if (coin_candle.ElementAt(i).high_price > max)
                        max = (int)coin_candle.ElementAt(i).high_price;
                    if (coin_candle.ElementAt(i).low_price < min)
                        min = (int)coin_candle.ElementAt(i).low_price;
                }
                this.chart1.ChartAreas[0].AxisY.Maximum = max;
                this.chart1.ChartAreas[0].AxisY.Minimum = min;
            }
        }
        
    }


}
