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
//listView1.EnsureVisible(listView1.Items.Count - 1)
namespace Mock_Investing
{
    public partial class Form1 : Form
    {
        List<Coin> coin;
        string market;
        List<CoinDetail> coins;
        public Form1()
        {
            InitializeComponent();
            market = fetchCoinList();
            Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            coins = fetchCoinInfo(market);
            for (int i = 0; i < coins.Count; i++)
            {
                datagridview.Rows.Add($"{coins[i].market}", $"{coin[i].korean_name}", $"{coin[i].english_name}", $"{coins[i].trade_price.ToString("C")}", $"{Math.Round(coins[i].signed_change_rate * 100, 2) + "%"}");
            }
        }
        private string fetchCoinList()
        {
            //코인 마켓 정보
            // 통신
            string urlMarket = "https://api.upbit.com/v1/market/all?isDetails=false";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlMarket);
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

            var coinObject = JsonSerializer.Deserialize<List<Coin>>(text);
            List<Coin> coinObjectKRW = new List<Coin>();
            coinObjectKRW.Add(coinObject[0]);
            string coinName = "KRW-BTC";
            for (int i = 1; i < coinObject.Count; i++)
            {
                if (coinObject[i].market.Contains("KRW-"))
                {
                    coinName = coinName + ", " + coinObject[i].market;
                    coinObjectKRW.Add(coinObject[i]);
                }
            }

            coin = coinObjectKRW;
            response.Close();
            stream.Close();
            reader.Close();
            return coinName;
        }
        private List<CoinDetail> fetchCoinInfo(string coinName)
        {
            // 각 코인 Ticker

            string urlTicker = "https://api.upbit.com/v1/ticker";
            StringBuilder dataParams = new StringBuilder();
            dataParams.Append("markets=" + coinName);
            HttpWebRequest requestDetail = (HttpWebRequest)WebRequest.Create(urlTicker + "?" + dataParams);
            requestDetail.Method = "GET";
            WebResponse responseDetail = requestDetail.GetResponse();
            Stream dataStream = responseDetail.GetResponseStream();
            StreamReader readerDetail = new StreamReader(dataStream);
            string text = readerDetail.ReadToEnd();
            JsonDocumentOptions jsonDocumentOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };
            JsonDocument jsonDocument = JsonDocument.Parse(text, jsonDocumentOptions);

            //역직렬화
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true, // 데이터 후행의 쉼표 허용 여부
            };

            var coinObject = JsonSerializer.Deserialize<List<CoinDetail>>(text);

            readerDetail.Close();
            dataStream.Close();
            responseDetail.Close();
            return coinObject;
        }

        // 매 1초마다 Tick 이벤트 핸들러 실행
        void timer_Tick(object sender, EventArgs e)
        {
            coins = fetchCoinInfo(market);
            for (int i = 0; i < coins.Count; i++)
            {
                //listBox.Items[i] = coins[i].market + " | " +coin[i].korean_name + ": \t" + coins[i].trade_price.ToString("C");
                datagridview.Rows[i].Cells[3].Value = $"{coins[i].trade_price.ToString("C")}";
                datagridview.Rows[i].Cells[4].Value = $"{Math.Round(coins[i].signed_change_rate * 100, 2) + "%"}";
            }

        }
        public class Coin
        {
            [JsonInclude]
            public string market { get; set; }              // 업비트에서 제공중인 시장 정보
            [JsonInclude]
            public string korean_name { get; set; }      // 	거래 대상 암호화폐 한글명
            [JsonInclude]
            public string english_name { get; set; }      // 거래 대상 암호화폐 영문명
            [JsonInclude]
            public string market_warning { get; set; }   // 유의 종목 여부
        }

        public class CoinDetail
        {

            [JsonInclude]
            public string market { get; set; }                      //	종목 구분 코드
            [JsonInclude]
            public string trade_date { get; set; }                  // 최근 거래 일자(UTC)
            [JsonInclude]
            public string trade_time { get; set; }                // 	최근 거래 시각(UTC)
            [JsonInclude]
            public string trade_date_kst { get; set; }            // 최근 거래 일자(KST)
            [JsonInclude]
            public string trade_time_kst { get; set; }           // 최근 거래 시각(KST)
            [JsonInclude]
            public double opening_price { get; set; }        // 시가
            [JsonInclude]
            public double high_price { get; set; }               // 고가
            [JsonInclude]
            public double low_price { get; set; }                // 저가
            [JsonInclude]
            public double trade_price { get; set; }             // 종가
            [JsonInclude]
            public double prev_closing_price { get; set; }  // 전일 종가
            [JsonInclude]
            public string change { get; set; }                  // EVEN : 보합  RISE : 상승  FALL : 하락
            [JsonInclude]
            public double change_price { get; set; }        // 변화액의 절대값
            [JsonInclude]
            public double change_rate { get; set; }         // 변화율의 절대값
            [JsonInclude]
            public double signed_change_price { get; set; } // 부호가 있는 변화액
            [JsonInclude]
            public double signed_change_rate { get; set; } // 부호가 있는 변화율
            [JsonInclude]
            public double trade_volume { get; set; }        // 가장 최근 거래량
            [JsonInclude]
            public double acc_trade_price { get; set; }     // 누적 거래대금(UTC 0시 기준)
            [JsonInclude]
            public double acc_trade_price_24h { get; set; } // 24시간 누적 거래대금
            [JsonInclude]
            public double acc_trade_volume { get; set; } // 누적 거래량(UTC 0시 기준)
            [JsonInclude]
            public double acc_trade_volume_24h { get; set; } // 24시간 누적 거래량
            [JsonInclude]
            public double highest_52_week_price { get; set; } // 52주 신고가
            [JsonInclude]
            public string highest_52_week_date { get; set; } // 52주 신고가 달성일
            [JsonInclude]
            public double lowest_52_week_price { get; set; } // 52주 신저가
            [JsonInclude]
            public string lowest_52_week_date { get; set; } // 52주 신저가 달성일
            [JsonInclude]
            public long timestamp { get; set; }                 // 타임스탬프
        }

     
        private void datagridview_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = datagridview.SelectedRows[0];
            string data = row.Cells[0].Value.ToString();
            Chart chart = new Chart(data);
            chart.Show();
        }
    }
}

