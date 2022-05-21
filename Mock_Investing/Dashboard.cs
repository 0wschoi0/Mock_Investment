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
using Google.Cloud.Firestore;

namespace Mock_Investing
{
    public partial class Dashboard : Form
    {
        string userName;
        List<Candle> coin_candle;
        string coinName = "";
        int maxViewY = 0;
        int minViewY = 0;
        int flag = 0;
        public Dashboard(string uid)
        {
            InitializeComponent();

            guna2DataGridView1.Rows.Add(1);
            guna2DataGridView1.Rows[0].Cells[1].Value = "레이";
            guna2DataGridView1.Rows[0].Cells[2].Value = "68.01";
            guna2DataGridView1.Rows[0].Cells[3].Value = "-5.76%";
            guna2DataGridView1.Rows[0].Cells[4].Value = "371.676";
        }

        private void butMy_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 0;
        }

        private void butView_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 1;
        }

        private void butFav_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 2;
        }

        private void butTrans_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 3;
            if (flag == 0)
            {
                flag = 1;
                Chart(coinName);
                
            }
        }

        private void butLogout_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        // Transaction Methods Start
        public void Chart(string coinName)
        {
            if (coinName == "")
            {
                coinName = "KRW-BTC";
            }
            this.coinName = coinName;
            
            transactionChart.Series["Series1"]["PriceDownColor"] = "Blue";
            coin_candle = fetchcandle("100");
            Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            transactionChart.MouseWheel += mouseWheel;
            maxViewY = (int)coin_candle.ElementAt(0).high_price;
            minViewY = (int)coin_candle.ElementAt(0).low_price;
            for (int i = 0; i < coin_candle.Count; i++)
            {
                transactionChart.Series["Series1"].Points.AddXY(coin_candle.ElementAt(i).candle_date_time_kst, coin_candle.ElementAt(i).high_price);
                transactionChart.Series["Series1"].Points[i].YValues[1] = coin_candle.ElementAt(i).low_price;
                transactionChart.Series["Series1"].Points[i].YValues[2] = coin_candle.ElementAt(i).opening_price;
                transactionChart.Series["Series1"].Points[i].YValues[3] = coin_candle.ElementAt(i).trade_price;
                if (coin_candle.ElementAt(0).opening_price >= coin_candle.ElementAt(0).trade_price)
                {
                    transactionChart.Series["Series1"].Points[0].Color = Color.Blue;
                    transactionChart.Series["Series1"].Points[0].BorderColor = Color.Blue;
                }
                if (coin_candle.ElementAt(i).opening_price < coin_candle.ElementAt(i).trade_price)
                {
                    transactionChart.Series["Series1"].Points[i].Color = Color.Red;
                    transactionChart.Series["Series1"].Points[i].BorderColor = Color.Red;
                }

                if (maxViewY < (int)coin_candle.ElementAt(i).high_price)
                    maxViewY = (int)coin_candle.ElementAt(i).high_price;
                if (minViewY > (int)coin_candle.ElementAt(i).low_price)
                    minViewY = (int)coin_candle.ElementAt(i).low_price;
            }
            transactionChart.ChartAreas[0].AxisY.Maximum = maxViewY;
            transactionChart.ChartAreas[0].AxisY.Minimum = minViewY;
        }

        public List<Candle> fetchcandle(string count)
        {
            string urlMarket = "https://api.upbit.com/v1/candles/minutes/1";
            StringBuilder dataParams = new StringBuilder();
            dataParams.Append("market=" + coinName + "&" + "count=" + count);
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

            response.Close();
            stream.Close();
            reader.Close();
            return JsonSerializer.Deserialize<List<Candle>>(text);
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

            if (sender.Equals(transactionChart))
            {
                var xAxis = transactionChart.ChartAreas[0].AxisX;
                int minx = (int)xAxis.ScaleView.ViewMinimum;
                int maxx = (int)xAxis.ScaleView.ViewMaximum;

                transactionChart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                transactionChart.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
                transactionChart.ChartAreas[0].AxisY.ScrollBar.Enabled = false;

                if (e.Delta > 0) // 휠 위로 Zoom in
                {
                    int start = maxx / 2; //오래된 캔들 데이터일수록 인덱스 증가
                    int end = minx; //제일 최근 캔들 데이터 인덱스 0

                    for (int i = start - 1; i < end; i++)
                    {
                        if (i >= coin_candle.Count)
                            break;
                        if (i < 0)
                            i = 0;
                    }
                    transactionChart.ChartAreas[0].AxisX.ScaleView.Zoom(end, start);
                }
                else //휠 아래 Zoom out
                {
                    int start = maxx * 2;//오래된 캔들 데이터일수록 인덱스 증가
                    int end = minx;//제일 최근 캔들 데이터 인덱스 0
                    for (int i = start - 1; i < end; i++)
                    {
                        if (i >= coin_candle.Count)
                            break;
                        if (i < 0)
                            i = 0;
                    }
                    transactionChart.ChartAreas[0].AxisX.ScaleView.Zoom(end, start);
                }
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {

            List<Candle> new_candle = fetchcandle("100");
            coin_candle = new_candle;
            transactionChart.Series["Series1"].Points.Clear();
            for (int i = 0; i < coin_candle.Count; i++)
            {
                transactionChart.Series["Series1"].Points.AddXY(coin_candle.ElementAt(i).candle_date_time_kst, coin_candle.ElementAt(i).high_price);
                transactionChart.Series["Series1"].Points[i].YValues[1] = coin_candle.ElementAt(i).low_price;
                transactionChart.Series["Series1"].Points[i].YValues[2] = coin_candle.ElementAt(i).opening_price;
                transactionChart.Series["Series1"].Points[i].YValues[3] = coin_candle.ElementAt(i).trade_price;
                if (coin_candle.ElementAt(i).opening_price < coin_candle.ElementAt(i).trade_price)
                {
                    transactionChart.Series["Series1"].Points[i].Color = Color.Red;
                    transactionChart.Series["Series1"].Points[i].BorderColor = Color.Red;
                }
                if (coin_candle.ElementAt(i).opening_price >= coin_candle.ElementAt(i).trade_price)
                {
                    transactionChart.Series["Series1"].Points[0].Color = Color.Blue;
                    transactionChart.Series["Series1"].Points[0].BorderColor = Color.Blue;
                }

                if (maxViewY < (int)coin_candle.ElementAt(i).high_price)
                    maxViewY = (int)coin_candle.ElementAt(i).high_price;
                if (minViewY > (int)coin_candle.ElementAt(i).low_price)
                    minViewY = (int)coin_candle.ElementAt(i).low_price;
            }
            transactionChart.ChartAreas[0].AxisY.Maximum = maxViewY;
            transactionChart.ChartAreas[0].AxisY.Minimum = minViewY;
        }
        // Transaction Methods End
    }
}
