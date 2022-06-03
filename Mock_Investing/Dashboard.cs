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
        string userName;    //유저 이름
        string UID;         //uid
        int coinNum;        //유저 보유 코인 종류 수
        int asset = 0;      //현금
        int wallet = 0;     //자산
        int profit = 0;     //이익
        CoinOwn coinCurrent;
        Rank rankCurrent;
        BuyRecord recordCurrent;
        FirestoreDb db;
        CollectionReference collection;
        DocumentReference documentStatus;
        DocumentReference documentCoins;
        DocumentReference documentRecords;
        DocumentReference rdocument;
        DocumentSnapshot getUserData;
        DocumentSnapshot getUserCoins;
        DocumentSnapshot getBuyRecords;
        DocumentSnapshot getRanking;
        List<Candle> coin_candle;
        List<Coin> coin;
        List<CoinDetail> coins;
        Rankers first;
        Rankers second;
        Rankers third;
        string market;
        string coinName = "";
        int maxViewY = 0;
        int minViewY = 0;

        public Dashboard(string uid)
        {
            InitializeComponent();
            db = FirestoreDb.Create("mock-af23d");
            UID = uid;
            market = fetchCoinList();
            Chart(coinName);
            Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            coins = fetchCoinInfo(market);
            
            // 코인 리스트 초기화
            for (int i = 0; i < coins.Count; i++)
            {
                gridCoinList.Rows.Add(1);
                gridCoinListChart.Rows.Add(1);
                double changeRate = Math.Round(coins[i].signed_change_rate * 100, 2);
                int tradePrice = (int)(coins[i].acc_trade_price_24h / 1000000);
                gridCoinList.Rows[i].Cells[0].Value = coin[i].market.Trim().Remove(0, 4);
                gridCoinList.Rows[i].Cells[1].Value = coin[i].korean_name;
                gridCoinList.Rows[i].Cells[2].Value = coins[i].trade_price.ToString("C");
                gridCoinList.Rows[i].Cells[3].Value = changeRate.ToString() + "%";

                gridCoinListChart.Rows[i].Cells[0].Value = coin[i].market.Trim().Remove(0, 4);
                gridCoinListChart.Rows[i].Cells[1].Value = coin[i].korean_name;
                gridCoinListChart.Rows[i].Cells[2].Value = coins[i].trade_price.ToString("C");
               // gridCoinListChart.Rows[i].Cells[3].Value = changeRate.ToString() + "%";
                if (changeRate < 0)
                {
                    gridCoinList.Rows[i].Cells[2].Style.ForeColor = Color.Blue;
                    gridCoinList.Rows[i].Cells[3].Style.ForeColor = Color.Blue;

                    gridCoinListChart.Rows[i].Cells[2].Style.ForeColor = Color.Blue;
                  //  gridCoinListChart.Rows[i].Cells[3].Style.ForeColor = Color.Blue;
                }
                else if (changeRate == 0)
                {
                    gridCoinList.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                    gridCoinList.Rows[i].Cells[3].Style.ForeColor = Color.Black;

                    gridCoinListChart.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                    //gridCoinListChart.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                }
                else
                {
                    gridCoinList.Rows[i].Cells[2].Style.ForeColor = Color.Red;
                    gridCoinList.Rows[i].Cells[3].Style.ForeColor = Color.Red;

                    gridCoinListChart.Rows[i].Cells[2].Style.ForeColor = Color.Red;
                    //gridCoinListChart.Rows[i].Cells[3].Style.ForeColor = Color.Red;
                }
                gridCoinList.Rows[i].Cells[4].Value = tradePrice.ToString("N0") + "백만";
                //gridCoinListChart.Rows[i].Cells[4].Value = tradePrice.ToString("N0") + "백만";
            }
            // 테스트용 데이터 삽입
            gridMainList.Rows.Add(4);
            for(int i = 0; i < 4; i++)
            {
                int TmptradePrice = (int)(coins[i].acc_trade_price_24h / 1000000);
                double TmpchangeRate = Math.Round(coins[i].signed_change_rate * 100, 2);
                gridMainList.Rows[i].Cells[1].Value = coin[i].market.Trim().Remove(0, 4);
                gridMainList.Rows[i].Cells[2].Value = coin[i].korean_name;
                gridMainList.Rows[i].Cells[3].Value = coins[i].trade_price.ToString("C");
                gridMainList.Rows[i].Cells[4].Value = TmpchangeRate.ToString() + "%";
                gridMainList.Rows[i].Cells[5].Value = TmptradePrice.ToString("N0") + "백만";

            }

            gridRecordList.Rows.Add(4);
            for (int i = 0; i < 4; i++)
            {
                gridRecordList.Rows[i].Cells[1].Value = coin[i].market.Trim().Remove(0, 4);
                gridRecordList.Rows[i].Cells[2].Value = coin[i].korean_name;
                gridRecordList.Rows[i].Cells[3].Value = coins[i].trade_price.ToString("C");
                gridRecordList.Rows[i].Cells[4].Value = "2022-05-27 22:50";
                gridRecordList.Rows[i].Cells[5].Value = "판매";
            }

            //
        }

        private async void Dashboard_Load(object sender, EventArgs e)
        {
            liveWallet();
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
        }
        private void myPro_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 4;
        }

        private void butLogout_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        // 매수 구현 시작

        // 매수 구현 끝

        // 실시간 Ranking 및 자산 조회
        async void liveWallet()
        {
            // Firebase에서 사용자 데이터 및 Ranking 정보 가져오기
            int walletTempt = 0;
            int profitTempt = 0;
            collection = db.Collection(UID);
            Query allUserData = db.Collection(UID);
            QuerySnapshot allUserDataSnapshot = await allUserData.GetSnapshotAsync();
            documentStatus = collection.Document("Status");
            documentCoins = collection.Document("Coins");
            documentRecords = collection.Document("Records");
            rdocument = db.Collection("Ranking").Document("Top");
            foreach (DocumentSnapshot documentSnapshot in allUserDataSnapshot.Documents)
            {
                if (documentSnapshot.Id == "Status")
                {
                    getUserData = documentSnapshot;
                }
                else if (documentSnapshot.Id == "Coins")
                {
                    getUserCoins = documentSnapshot;
                }
                else if (documentSnapshot.Id == "Records")
                {
                    getBuyRecords = documentSnapshot;
                }
            }
            // 현재 자산 띄우기 & 현재 이익 띄우기 시작
            coinNum = getUserData.GetValue<int>("CoinNumber");
            if (coinNum > 0)
            {
                coinCurrent = getUserCoins.ConvertTo<CoinOwn>();
                recordCurrent = getBuyRecords.ConvertTo<BuyRecord>();

                foreach (KeyValuePair<string, double> pair in coinCurrent.CoinCurrent)
                {
                    for (int i = 0; i < coins.Count; i++)
                    {
                        if (pair.Key == coins[i].market)
                        {
                            int tempt = (int)((coins[i].trade_price - recordCurrent.BuyRecords[pair.Key]) * pair.Value);
                            profitTempt += tempt;
                            walletTempt += (int)(pair.Value * coins[i].trade_price);
                        }
                    }
                }
            }
            walletTempt += getUserData.GetValue<int>("Asset");
            await documentStatus.UpdateAsync("Wallet", walletTempt);
            header_Overall.Text = walletTempt.ToString("C");
            wallet = walletTempt;
            profit = profitTempt;
            header_Wallet.Text = getUserData.GetValue<int>("Asset").ToString("C");
            if (profitTempt > 0) { header_Profit.Text = "+" + profitTempt.ToString("C"); }
            else { header_Profit.Text = profitTempt.ToString("C"); }
            // 현재 자산 띄우기 & 현재 이익 띄우기 끝

            userName = getUserData.GetValue<string>("Name");
            header_Name.Text = userName;

            // Ranking 정보 띄우기 시작
            first = new Rankers();
            second = new Rankers();
            third = new Rankers();
            first.Money = 0;
            first.Name = "";
            second.Money = 0;
            second.Name = "";
            third.Money = 0;
            third.Name = "";
            getRanking = await rdocument.GetSnapshotAsync();
            rankCurrent = getRanking.ConvertTo<Rank>();
            foreach (KeyValuePair<string, string> pair in rankCurrent.UID)
            {
                CollectionReference userAll = db.Collection(pair.Value);
                DocumentReference userSpecify = userAll.Document("Status");
                DocumentSnapshot userSnapshot = await userSpecify.GetSnapshotAsync();
                if (third.Money < userSnapshot.GetValue<int>("Wallet"))
                {
                    if (second.Money < userSnapshot.GetValue<int>("Wallet"))
                    {
                        if (first.Money < userSnapshot.GetValue<int>("Wallet"))
                        {
                            third.Money = second.Money;
                            third.Name = second.Name;
                            second.Money = first.Money;
                            second.Name = first.Name;
                            first.Money = userSnapshot.GetValue<int>("Wallet");
                            first.Name = userSnapshot.GetValue<string>("Name");
                            continue;
                        }
                        third.Money = second.Money;
                        third.Name = second.Name;
                        second.Money = userSnapshot.GetValue<int>("Wallet");
                        second.Name = userSnapshot.GetValue<string>("Name");
                        continue;
                    }
                    third.Money = userSnapshot.GetValue<int>("Wallet");
                    third.Name = userSnapshot.GetValue<string>("Name");
                }
            }
            firstRankName.Text = first.Name;
            firstRankWallet.Text = first.Money.ToString("C");
            secondRankName.Text = second.Name;
            secondRankWallet.Text = second.Money.ToString("C");
            thirdRankName.Text = third.Name;
            thirdRankWallet.Text = third.Money.ToString("C");
            // Ranking 정보 띄우기 끝
        }

        // 실시간 차트 구현 시작
        public void Chart(string coinName)
        {
            if (coinName == "")
            {
                coinName = "KRW-BTC";
            }
            this.coinName = coinName;
          
            transactionChart.Series["Series1"]["PriceDownColor"] = "Blue";
            coin_candle = fetchcandle("100");

            lblChartCoinPrice.Text = coin_candle.ElementAt(0).trade_price.ToString("C");

            transactionChart.MouseWheel += mouseWheel;
            transactionChart.Series["Series1"].Points.Clear();
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

        private void mouseWheel(object sender, MouseEventArgs e)
        {

            if (sender.Equals(transactionChart))
            {
                var xAxis = transactionChart.ChartAreas[0].AxisX;
                int maxx = (int)xAxis.ScaleView.ViewMaximum;

                transactionChart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                transactionChart.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
                transactionChart.ChartAreas[0].AxisY.ScrollBar.Enabled = false;

                if (e.Delta > 0) // 휠 위로 Zoom in
                {
                    int start = (int)(maxx / 1.5); //오래된 캔들 데이터일수록 인덱스 증가
                    int end = 0; //제일 최근 캔들 데이터 인덱스 0

                   transactionChart.ChartAreas[0].AxisX.ScaleView.Zoom(end, start);
                }
                else //휠 아래 Zoom out
                {
                    int start = (int)(maxx * 1.5);//오래된 캔들 데이터일수록 인덱스 증가
                    int end = 0;//제일 최근 캔들 데이터 인덱스 0
                    
                    transactionChart.ChartAreas[0].AxisX.ScaleView.Zoom(end, start);
                }
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            liveWallet();
            List<Candle> new_candle = fetchcandle("100");
            coin_candle = new_candle;
            lblChartCoinPrice.Text = coin_candle.ElementAt(0).trade_price.ToString("C");
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

            // 코인 리스트 업데이트
            coins = fetchCoinInfo(market);
            for (int i = 0; i < coins.Count; i++)
            {
                double changeRate = Math.Round(coins[i].signed_change_rate * 100, 2);
                int tradePrice = (int)(coins[i].acc_trade_price_24h / 1000000);

                gridCoinList.Rows[i].Cells[2].Value = coins[i].trade_price.ToString("C");
                gridCoinList.Rows[i].Cells[3].Value = changeRate.ToString() + "%";


                gridCoinListChart.Rows[i].Cells[2].Value = coins[i].trade_price.ToString("C");
                //gridCoinListChart.Rows[i].Cells[3].Value = changeRate.ToString() + "%";
                if (changeRate < 0)
                {
                    gridCoinList.Rows[i].Cells[2].Style.ForeColor = Color.Blue;
                    gridCoinList.Rows[i].Cells[3].Style.ForeColor = Color.Blue;

                    gridCoinListChart.Rows[i].Cells[2].Style.ForeColor = Color.Blue;
                    //gridCoinListChart.Rows[i].Cells[3].Style.ForeColor = Color.Blue;
                }
                else if (changeRate == 0)
                {
                    gridCoinList.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                    gridCoinList.Rows[i].Cells[3].Style.ForeColor = Color.Black;

                    gridCoinListChart.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                    //gridCoinListChart.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                }
                else
                {
                    gridCoinList.Rows[i].Cells[2].Style.ForeColor = Color.Red;
                    gridCoinList.Rows[i].Cells[3].Style.ForeColor = Color.Red;

                    gridCoinListChart.Rows[i].Cells[2].Style.ForeColor = Color.Red;
                    //gridCoinListChart.Rows[i].Cells[3].Style.ForeColor = Color.Red;
                }
                gridCoinList.Rows[i].Cells[4].Value = tradePrice.ToString("N0") + "백만";
                //gridCoinListChart.Rows[i].Cells[4].Value = tradePrice.ToString("N0") + "백만";

            }
            txtboxBuyPrice.Text = lblChartCoinPrice.Text; // 매수 가격 
            txtboxSellPrice.Text = lblChartCoinPrice.Text;//매도 가격
        }

        private void gridCoinList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            coinName = "KRW-" + gridCoinList.Rows[e.RowIndex].Cells[0].Value.ToString();
            lblChartCoinName.Text = gridCoinList.Rows[e.RowIndex].Cells[1].Value.ToString();
            lblChartCoinPrice.Text = gridCoinList.Rows[e.RowIndex].Cells[2].Value.ToString();
            butTrans.PerformClick();
            Chart(coinName);
        }

        private void gridCoinListChart_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            coinName = "KRW-" + gridCoinListChart.Rows[e.RowIndex].Cells[0].Value.ToString();
            lblChartCoinName.Text = gridCoinListChart.Rows[e.RowIndex].Cells[1].Value.ToString();
            lblChartCoinPrice.Text = gridCoinListChart.Rows[e.RowIndex].Cells[2].Value.ToString();
            Chart(coinName);
        }

        public class Candle
        {
            string date_format = "";
            [JsonInclude]
            public string market { get; set; }

            [JsonInclude]
            public string candle_date_time_kst { get { return date_format.Substring(5, 5) + " " + date_format.Substring(11, 5); } set { date_format = value; } }

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

        // 실시간 차트 구현 끝

        // 실시간 코인 리스트 구현 시작
        private string fetchCoinList()
        {
            //코인 마켓 정보
            // 통신
            string urlMarket = "https://api.upbit.com/v1/market/all?isDetails=false";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlMarket);
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
            JsonDocument jsonDocument = JsonDocument.Parse(text, jsonDocumentOptions);

            //역직렬화
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true, // 데이터 후행의 쉼표 허용 여부
            };

            var coinObject = JsonSerializer.Deserialize<List<Coin>>(text);
            List<Coin> coinObjectKRW = new List<Coin>();
            coinObjectKRW.Add(coinObject[0]);
            string coinName = "KRW-BTC";
            for (int i = 1; i < coinObject.Count; i++)
            {
                if (coinObject[i].market.Contains("KRW-"))
                {
                    coinName += ", " + coinObject[i].market;
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

        public class Rankers
        {
            public int Money { get; set; }
            public string Name { get; set; }
        }

        [FirestoreData]
        public class CoinOwn
        {
            [FirestoreProperty]
            public Dictionary<string, double> CoinCurrent { get; set; }
        }
        [FirestoreData]
        public class BuyRecord
        {
            [FirestoreProperty]
            public Dictionary<string, double> BuyRecords { get; set; }
        }
        [FirestoreData]
        public class Rank
        {
            [FirestoreProperty]
            public Dictionary<string, string> UID { get; set; }
        }

        private void btnBuyOrderQuantity25_Click(object sender, EventArgs e)
        {

        }









        // 실시간 코인 리스트 구현 끝


    }
}
