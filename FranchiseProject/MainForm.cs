﻿//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using Newtonsoft.Json;
//using System.Diagnostics;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace FranchiseProject
{
    public partial class MainForm : Form
    {
        // 지역명, 위도, 경도  (ex. "문정동", "37.412412", "124.512512")
        List<Tuple<string, double, double>> tuples = new List<Tuple<string, double, double>>();

        // DB 불러오기
        private const string ConnectionString = "Host=10.10.20.103;Username=postgres;Password=1234;Database=franchise";

        // 검색 버튼 클릭 됐는지 감지
        bool clickDetected = false;

        // 마우스 드래그를 위한 offset 변수
        private Point offset;

        // 전역 변수
        private string minCostValue_;
        private string maxCostValue_;
        private string salesIncome_;
        private string salesPeople_;
        private string facilityCnt_;
        private string resultRate_;
        private string resultCnt_;
        private string resultCompete_;

        // 생성자
        public MainForm()
        {
            InitializeComponent();
            InitializeComboBoxes();
        }

        // 지도
        private void MainForm_Load(object sender, EventArgs e)
        {
            SetFontList();
            // 로드될 때 생성
            // WebBrowser 컨트롤에 "kakaoMap.html" 을 표시한다. 
            Version ver = webBrowser1.Version;
            string name = webBrowser1.ProductName;
            string str = webBrowser1.ProductVersion;
            string html = "kakaoMap.html";
            string dir = Directory.GetCurrentDirectory();
            string path = Path.Combine(dir, html);
            webBrowser1.Navigate(path);
            SetFontList();
        }

        // 메인폼 테두리 설정
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                Color.Black, 2, ButtonBorderStyle.Solid,
                Color.Black, 2, ButtonBorderStyle.Solid,
                Color.Black, 2, ButtonBorderStyle.Solid,
                Color.Black, 2, ButtonBorderStyle.Solid);
        }

        // 폰트 불러오는 함수
        public static Font FontLoad(int fontNum, int fontSize)
        {
            string[] fontPaths = { @"font\Pretendard-Regular.ttf", @"font\Maplestory_Bold.ttf" };
            string baseDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            using (PrivateFontCollection privateFonts = new PrivateFontCollection())
            {
                foreach (string fontPath in fontPaths)
                {
                    string fontFilePath = Path.Combine(baseDirectory, fontPath);
                    privateFonts.AddFontFile(fontFilePath);
                }

                if (fontNum >= 0 && fontNum < privateFonts.Families.Length)
                {
                    return new Font(privateFonts.Families[fontNum], fontSize);
                }

                else
                {
                    return SystemFonts.DefaultFont;
                }
            }
        }

        public void SetFont(Control control, int fontNum, int fontSize)
        {
            control.Font = FontLoad(fontNum, fontSize);
        }

        // MainForm 내부의 폰트를 수정합니다.
        // 폰트의 수정이 필요하면 아래의 값만 수정해주세요. SetFont(라벨명, 폰트번호, 폰트사이즈)
        // index 0: 프리텐다드 1: 메이플스토리 볼드
        public void SetFontList()
        {
            flatComboBox1.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            flatComboBox2.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            tabControl1.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));

            checkBox1.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            checkBox2.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            checkBox3.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            checkBox4.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            checkBox5.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            checkBox6.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            checkBox7.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            checkBox8.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            checkBox9.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            checkBox10.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));

            button2.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            button3.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            button4.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));

            resultButton.Font = new Font(FontManager.fontFamilys[1], 15, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            minimizeButton.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            exitButton.Font = new Font(FontManager.fontFamilys[0], 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));

            //SetFont(flatComboBox1, 0, 9);
            //SetFont(flatComboBox2, 0, 9);
            //SetFont(tabControl1, 0, 9);
            //SetFont(checkBox1, 0, 9);
            //SetFont(checkBox2, 0, 9);
            //SetFont(checkBox3, 0, 9);
            //SetFont(checkBox4, 0, 9);
            //SetFont(checkBox5, 0, 9);
            //SetFont(checkBox6, 0, 9);
            //SetFont(checkBox7, 0, 9);
            //SetFont(checkBox8, 0, 9);
            //SetFont(checkBox9, 0, 9);
            //SetFont(checkBox10, 0, 9);
            //SetFont(button2, 0, 9);
            //SetFont(button3, 0, 9);
            //SetFont(button4, 0, 9);
            //SetFont(resultButton, 1, 15);
            //SetFont(minimizeButton, 0, 9);
            //SetFont(exitButton, 0, 9);
        }

        // DB
        // 특정 테이블에서 특정 칼럼의 값을 반환하는 함수
        public static List<string> GetValuesFromTable(string tableName, string columnName, string criteria = null, bool distinct = false)
        {
            List<string> results = new List<string>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                string distinctClause = distinct ? "DISTINCT" : ""; // distinct 값에 따라 쿼리 조각 결정
                string query = $"SELECT {distinctClause} \"{columnName}\" FROM \"{tableName}\"";

                if (!string.IsNullOrWhiteSpace(criteria))
                {
                    query += $" WHERE {criteria}";
                }

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return results;
        }
        
        // 특정 테이블의 여러 컬럼 값을 반환할 
        public static List<Dictionary<string, object>> GetValuesFromMultipleColumns(string tableName, List<string> columnNames, string criteria = null, bool distinct = false)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                string columns = string.Join(", ", columnNames.Select(c => $"\"{c}\""));
                string distinctClause = distinct ? "DISTINCT" : ""; // distinct 값에 따라 쿼리 조각 결정
                string query = $"SELECT {distinctClause} {columns} FROM \"{tableName}\"";

                if (!string.IsNullOrWhiteSpace(criteria))
                {
                    query += $" WHERE {criteria}";
                }
                Console.WriteLine(query);
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            results.Add(row);
                        }
                    }
                }
            }

            return results;
        }
        
        // 특정 테이블의 모든 행의 값을 반환하는 함수
        public static List<Dictionary<string, object>> GetAllRowsFromTable(string tableName, string criteria = null)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                string query = $"SELECT * FROM \"{tableName}\"";

                if (!string.IsNullOrWhiteSpace(criteria))
                {
                    query += $" WHERE {criteria}";
                }

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            results.Add(row);
                        }
                    }
                }
            }

            return results;
        }

        // 콤보박스
        private void InitializeComboBoxes()
        {
            //콤보박스
            string[] data = { "북구", "서구", "동구", "남구", "광산구" };
            flatComboBox1.Items.Add("선택");
            flatComboBox1.Items.AddRange(data); // 콤보박스에 자료 넣기
            flatComboBox1.SelectedIndex = 0; // 첫번째 아이템 선택
        }

        //private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        //{
        //    // 첫 번째 콤보박스의 선택에 따라 두 번째 콤보박스의 항목을 설정
        //    string selectedGu = comboBox1.SelectedItem.ToString();
        //    update_combobox2(selectedGu);
        //    comboBox2.SelectedIndex = 0;
        //}

        private void flatComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 첫 번째 콤보박스의 선택에 따라 두 번째 콤보박스의 항목을 설정
            string selectedGu = flatComboBox1.SelectedItem.ToString();
            UpdateComboBox2(selectedGu);
            flatComboBox2.SelectedIndex = 0;
        }

        private void UpdateComboBox2(string guName)
        {
            // 두 번째 콤보박스의 항목을 초기화
            flatComboBox2.Items.Clear();
            List<string> DongNames = GetValuesFromTable("TB_DONG", "H_DONG_NAME", $"\"GU_NAME\" = '{guName}' ORDER BY \"H_DONG_NAME\"", true);
            flatComboBox2.Items.Add("선택");
            foreach (string dong in DongNames)
            {
                flatComboBox2.Items.Add(dong); // ComboBox에 d를 추가합니다.
            }
        }

        private void UpdateTabPage(string guName, string dongName)
        {
            // 현재 comboBox2에서 선택된 동(Dong) 이름을 가져옴
            string dong = flatComboBox2.Text;

            // DB로부터 가져올 칼럼 이름들을 리스트로 정의
            var columns = new List<string> { "DEAL_TYPE", "DEAL_USE", "DEAL_GU", "DEAL_DONG", "DEAL_ADDR", "DEAL_DEPOSIT", "DEAL_PRICE", "DEAL_RENT_PRICE", "DEAL_SPACE" };

            // DB로부터 지정된 조건의 레코드들을 가져옴
            var data = GetValuesFromMultipleColumns("TB_DEAL", columns, $"\"DEAL_DONG\" = '{dongName}'");

            // 기존 리스트 뷰 아이템을 모두 지움
            listView1.Items.Clear();
            listView2.Items.Clear();

            // 가져온 각 행(레코드)에 대해 아래의 작업을 수행
            foreach (var row in data)
            {
                // 해당 행에서 필요한 데이터를 가져옴
                string dealType = row["DEAL_TYPE"].ToString();
                string dealUse = row["DEAL_USE"].ToString();
                string dealDeposit = row["DEAL_DEPOSIT"].ToString();
                string dealPrice = row["DEAL_PRICE"].ToString();
                string dealRentprice = row["DEAL_RENT_PRICE"].ToString();
                string dealSpace = row["DEAL_SPACE"].ToString();

                // 거래 유형이 '매매'인 경우
                if (dealType == "매매")
                {
                    // listView1에 해당 아이템을 추가
                    var item = new ListViewItem(dealType);
                    item.SubItems.Add($"{dealUse}");
                    item.SubItems.Add($"{dealPrice}");
                    item.SubItems.Add($"{dealSpace}");
                    listView1.Items.Add(item);
                }

                // 거래 유형이 '월세'인 경우
                else if (dealType == "월세")
                {
                    // listView2에 해당 아이템을 추가
                    var item = new ListViewItem(dealType);
                    item.SubItems.Add($"{dealUse}");
                    item.SubItems.Add($"{dealDeposit}");
                    item.SubItems.Add($"{dealRentprice}");
                    item.SubItems.Add($"{dealSpace}");
                    listView2.Items.Add(item);
                }

                // 출력 확인용: 현재 행의 모든 열(칼럼) 데이터를 콘솔에 출력
                foreach (var keyValuePair in row)
                {
                    Console.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value}");
                }
            }
        }

        private void UpdatePictureBox(string guName, string dongName)
        {
            // 필요한 열 이름들을 리스트에 저장
            var columns = new List<string> { "GU_NAME", "H_DONG_NAME" };

            // GetValuesFromMultipleColumns 메서드를 사용하여 데이터베이스에서 해당 조건에 맞는 데이터를 가져옴
            var data = GetValuesFromMultipleColumns("TB_DONG", columns, $" \"GU_NAME\" = '{guName}' and \"H_DONG_NAME\" = '{dongName}' ");

            // 가져온 데이터가 없으면 함수를 종료
            if (data.Count == 0)
            {
                return;
            }

            // 첫 번째 행의 데이터를 가져옴
            var row = data[0];
            string dongName_ = row["H_DONG_NAME"].ToString();
            string guName_ = row["GU_NAME"].ToString();

            // 이미지 파일들이 저장된 폴더 경로들을 배열에 저장
            string[] folderNames = {
                @"graph\00_동별_다중이용시설",
                @"graph\01_동별_인구비율",
                @"graph\02_동별_면적범위별_평균보증금_임대료_pastel",
                @"graph\03_구별_1030인구대비_월평균추정매출",
                @"graph\04_구별_월평균추정매출_경쟁업체",
                @"graph\05_전역_광주광역시_화장품상가_분포도"
            };

            string currentDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            // 각 폴더에서 해당 구와 동 이름을 포함하는 이미지 파일들의 경로를 가져옴
            for (int idx = 0; idx < folderNames.Length; idx++)
            {
                string folderPath = Path.Combine(currentDirectory, folderNames[idx]);
                List<string> imageFiles = GetMatchingImageFiles(folderPath, guName, dongName);

                // 이미지 파일이 없는 경우 none_data 이미지로 설정
                if (imageFiles.Count == 0)
                {
                    string noneDataImagePath = Path.Combine(folderPath, "none_data.png");
                    Image noneDataImage = Image.FromFile(noneDataImagePath);

                    // 현재 폴더 인덱스에 따라 해당 PictureBox에 none_data를 설정
                    if (idx == 0)
                    {
                        facPictureBox.Image = noneDataImage;
                    }
                    else if (idx == 1)
                    {
                        popPictureBox.Image = noneDataImage;
                    }
                    else if (idx == 2)
                    {
                        pricePictureBox.Image = noneDataImage;
                    }
                    else if (idx == 3)
                    {
                        guPictureBox1.Image = noneDataImage;
                    }
                    else if (idx == 4)
                    {
                        guPictureBox2.Image = noneDataImage;
                    }
                }
                else
                {
                    // 이미지 파일이 있는 경우, 해당 이미지를 설정
                    string imagePath = imageFiles[0];
                    Image image = Image.FromFile(imagePath);

                    // 현재 폴더 인덱스에 따라 해당 PictureBox에 이미지를 설정
                    if (idx == 0)
                    {
                        facPictureBox.Image = image;
                    }
                    else if (idx == 1)
                    {
                        popPictureBox.Image = image;
                    }
                    else if (idx == 2)
                    {
                        pricePictureBox.Image = image;
                    }
                    else if (idx == 3)
                    {
                        guPictureBox1.Image = image;
                    }
                    else if (idx == 4)
                    {
                        guPictureBox2.Image = image;
                    }
                }

                // 광주광역시 전체 데이터는 직접 경로와 파일명을 가져와서 pictureBox에 저장
                rivalPictureBox1.Image = Image.FromFile(GetImagePath(folderNames[5], $"광주광역시_전체상가_지도.png"));
                rivalPictureBox2.Image = Image.FromFile(GetImagePath(folderNames[5], $"광주광역시_화장품상가_지도.png"));
            }
        }

        // 주어진 폴더 경로와 파일 이름을 기반으로 이미지 파일의 전체 경로를 생성하는 함수
        private string GetImagePath(string folderPath, string fileName)
        {
            // 상대 경로를 가져옴
            string currentDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            // Path.Combine 메서드를 사용하여 현재 디렉토리, 폴더 경로, 파일 이름을 결합하여 이미지 파일의 전체 경로를 반환함
            return Path.Combine(currentDirectory, folderPath, fileName);
        }

        // 주어진 기본 폴더 경로(baseFolderPath), 구이름(guName), 동이름(dongName)에 맞는 이미지 파일들을 찾아서 리스트로 반환
        private List<string> GetMatchingImageFiles(string baseFolderPath, string guName, string dongName)
        {
            // 결과로 반환할 이미지 파일들을 담을 리스트 생성
            List<string> imageFiles = new List<string>();

            // 지정된 폴더 경로에서 특정 패턴에 맞는 파일들을 찾아옴 ('구이름_동이름.*' 형태)
            string[] filesWithDong = Directory.GetFiles(baseFolderPath, $"{guName}_{dongName}.*");
            string[] filesWithoutDong = Directory.GetFiles(baseFolderPath, $"{guName}.*");

            // '구이름_동이름.*' 형태의 파일이 없을 경우 '구이름.*' 형태의 파일들을 가져옴
            if (filesWithDong.Length == 0)
            {
                imageFiles.AddRange(filesWithoutDong);
            }

            else
            {
                // '구이름_동이름.*' 형태의 파일이 있을 경우 해당 파일들을 반환할 리스트에 추가
                imageFiles.AddRange(filesWithDong);
            }

            // 결과 리스트 반환
            return imageFiles;
        }

        // 지역 검색
        public void Search(string area)
        {
            // 요청을 보낼 url 
            string site = "https://dapi.kakao.com/v2/local/search/address.json";
            string query = string.Format("{0}?query={1}", site, area);
            WebRequest request = WebRequest.Create(query); // 요청 생성. 
            string apiKey = "106e805bafc9548f37b878db306c0484"; // API 인증키 입력. (각자 발급한 API 인증키를 입력하자)
            string header = "KakaoAK " + apiKey;


            request.Headers.Add("Authorization", header); // HTTP 헤더 "Authorization" 에 header 값 설정. 
            WebResponse response = request.GetResponse(); // 요청을 보내고 응답 객체를 받는다. 
            Stream stream = response.GetResponseStream(); // 응답객체의 결과물


            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            String json = reader.ReadToEnd(); // JOSN 포멧 문자열
            //Console.WriteLine("결과물" + json);

            JavaScriptSerializer js = new JavaScriptSerializer(); // (Reference 에 System.Web.Extensions.dll 을 추가해야한다)
            var dob = js.Deserialize<dynamic>(json);

            var docs = dob["documents"];
            object[] buf = docs;
            int length = buf.Length;

            for (int i = 0; i < length; i++) // 지역명, 위도, 경도 읽어오기. 
            {
                string addressName = docs[i]["address_name"];
                double x = double.Parse(docs[i]["x"]); // 위도
                double y = double.Parse(docs[i]["y"]); // 경도
                tuples.Add(new Tuple<string, double, double>(addressName, x, y));
                Console.WriteLine("저장한주소값: " + addressName + x + y);
            }
        }

        // 지도 확대
        private void ZoomInMap(object sender, EventArgs e)
        {
            webBrowser1.Document.InvokeScript("zoomIn"); // 줌인
        }

        // 지도 축소
        private void ZoomOutMap(object sender, EventArgs e)
        {
            webBrowser1.Document.InvokeScript("zoomOut"); // 줌아웃
        }

        // 검색 버튼 눌렀을 때 연결
        private void SearchButtonClick(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            SetFontList();
            //정보 불러오기
            tuples.Clear();
            string gu = flatComboBox1.Text;
            string dong = flatComboBox2.Text;
            string newAddr = "광주광역시 " + gu + dong;

            if (gu == "선택" | dong == "선택")
            {
                MessageBox.Show("구와 동을 선택해주세요!", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 클릭 됐는지 감지
            clickDetected = true;

            // 튜플에 값 넣기
            Search(newAddr);
            var sel = tuples[0];

            // 위도, 경도 불러와서 이동
            object[] arr = new object[] { sel.Item3, sel.Item2 }; // 위도, 경도
            object res = webBrowser1.Document.InvokeScript("panTo", arr);
            UpdateTabPage(gu, dong);
            UpdatePictureBox(gu, dong);

            // 올리브영 위치 찍기
            var columns = new List<string> { "LOC_NAME", "LOC_ADDR", "LOC_X", "LOC_Y" };
            var condition = $"\"LOC_GU\" = '{gu}' AND \"LOC_DONG\" = \'{dong}\'";

            var data = GetValuesFromMultipleColumns("TB_LOCATION", columns, condition, false);
            StringBuilder jsCode = new StringBuilder();
            jsCode.AppendLine($"remove_markers('olive_young');");

            SetFontList();

            if (data != null && data.Count > 0)
            {
                jsCode.AppendLine($"add_markers('olive_young', [");
                foreach (var row in data)
                {
                    string name = row["LOC_NAME"].ToString(); // 업체명
                    string addr = row["LOC_ADDR"].ToString();  // 주소 
                    string x = row["LOC_X"].ToString(); //x좌표
                    string y = row["LOC_Y"].ToString(); //y좌표
                    Console.WriteLine(name + addr + x + y); // 확인용

                    // 각 시설의 정보를 바탕으로 JavaScript 코드를 추가
                    jsCode.AppendLine($"{{ title: '{name}', addr: '{addr}', latlng: new kakao.maps.LatLng({x}, {y}) }},");
                }
                jsCode.AppendLine("]);");
                Console.WriteLine(jsCode.ToString());

                // 생성된 JavaScript 코드를 웹 브라우저 컨트롤을 통해 실행
                webBrowser1.Document.InvokeScript("eval", new object[] { jsCode.ToString() });
            }

            // 버스 위치 찍기
            var busColumns = new List<string> { "BUS_NAME", "BUS_ADDR", "BUS_X", "BUS_Y" };
            var busCondition = $"\"BUS_GU\" = '{gu}' AND \"BUS_DONG\" = \'{dong}\'";

            var busData = GetValuesFromMultipleColumns("TB_BUS", busColumns, busCondition, false);
            StringBuilder busJsCode = new StringBuilder();
            busJsCode.AppendLine($"remove_markers('bus');");

            if (busData != null && busData.Count > 0)
            {
                busJsCode.AppendLine($"add_markers('bus', [");
                foreach (var row in busData)
                {
                    string name = row["BUS_NAME"].ToString(); // 업체명
                    string addr = row["BUS_ADDR"].ToString();  // 주소 
                    string x = row["BUS_X"].ToString(); //x좌표
                    string y = row["BUS_Y"].ToString(); //y좌표
                    Console.WriteLine(name + addr + x + y); // 확인용

                    // 각 시설의 정보를 바탕으로 JavaScript 코드를 추가
                    busJsCode.AppendLine($"{{ title: '{name}', addr: '{addr}', latlng: new kakao.maps.LatLng({x}, {y}) }},");
                }
                busJsCode.AppendLine("]);");
                Console.WriteLine(busJsCode.ToString());

                // 생성된 JavaScript 코드를 웹 브라우저 컨트롤을 통해 실행
                webBrowser1.Document.InvokeScript("eval", new object[] { busJsCode.ToString() });
            }

            SetFontList();

            //예상 창업 비용 작업 완
            var columnsDeal = new List<string> { "DEAL_DEPOSIT", "DEAL_RENT_PRICE", "DEAL_SPACE" };
            var conditionDeal = $"\"DEAL_TYPE\" = \'월세\' and \"DEAL_DONG\" = \'{dong}\'";
            var dataDeal = GetValuesFromMultipleColumns("TB_DEAL", columnsDeal, conditionDeal, false);
            int franchiseCost = 1100; // 가맹비
            int premium = 10000; // 권리금
            int furniture = 13000; // 집기비용
            int systemCost = 1000; // 전산비용
            int startGoods = 10000; // 초도상품구매비용
            int workCost = 1200; // 공사비
            int etc = 200; // 기타
            int deposit = 0; // 보증금
            int rentPrice = 0; // 임대료
            float space = 0; // 면적
            int interiorCost = 0; // 인테리어비 면적 // 3.3 * 198

            List<int> depositList = new List<int>();
            List<int> interiorList = new List<int>();
            foreach (var row in dataDeal)
            {
                deposit = Convert.ToInt32(row["DEAL_DEPOSIT"]);
                rentPrice = Convert.ToInt32(row["DEAL_RENT_PRICE"]);
                space = Convert.ToSingle(row["DEAL_SPACE"]);
                double result = space / 3.3 * 198;
                interiorCost = Convert.ToInt32(result);
                depositList.Add(deposit);
                interiorList.Add(interiorCost);
            }

            int minDeposit = depositList.Count > 0 ? depositList.Min() : 0;
            int maxDeposit = depositList.Count > 0 ? depositList.Max() : 0;
            int minInteriorCost = interiorList.Count > 0 ? interiorList.Min() : 0;
            int maxInteriorCost = interiorList.Count > 0 ? interiorList.Max() : 0;

            int totalMin = franchiseCost + premium + furniture + systemCost + startGoods + workCost + etc + minDeposit + minInteriorCost;
            int totalMax = franchiseCost + premium + furniture + systemCost + startGoods + workCost + etc + maxDeposit + maxInteriorCost;

            string minCost = FormatWon(totalMin); // 최종 최소 금액
            string maxCost = FormatWon(totalMax); // 최종 최고 금액

            // ↓ 월평균매출, 유동인구 로직
            string condition1 = $"\"FACILITY_GU\" = '{gu}' AND \"FACILITY_DONG\" = '{dong}'";
            var faciltiyData = GetAllRowsFromTable("TB_FACILITY", condition1);
            int facilityCnt = faciltiyData.Count(); // 다중이용시설 갯수

            var salsesColumns = new List<string> { "SALES_INCOME", "SALES_PEOPLE" };
            var salesCon = $"\"SALES_GU\" = '{gu}' AND \"SALES_DONG\" = '{dong}'";
            var salesData = GetValuesFromMultipleColumns("TB_SALES", salsesColumns, salesCon, false);

            // 월평균매출, 유동인구
            string salesIncome = salesData[0]["SALES_INCOME"].ToString(); // 월평균매출 
            string salesPeople = salesData[0]["SALES_PEOPLE"].ToString(); // 유동인구

            // 데이터 정규화 및 추천 기능 테이블
            // List<string> Result = GetValuesFromTable("TB_RESULT", "RESULT_RATE", $"\"RESULT_GU\" = '{gu}' AND \"RESULT_DONG\" = '{dong}'", false);
            var resultColumns = new List<string> { "RESULT_RATE", "RESULT_CNT" };
            var result_ = GetValuesFromMultipleColumns("TB_RESULT", resultColumns, $"\"RESULT_GU\" = '{gu}' AND \"RESULT_DONG\" = '{dong}'", false);
            string resultRate = result_[0]["RESULT_RATE"].ToString();
            string resultCnt = result_[0]["RESULT_CNT"].ToString(); // 해당동의 올리브영 갯수

            // 경쟁업체 수 조회
            var competeColumns = new List<string> { "SALES_COMPETE" };
            var competeQuery = GetValuesFromMultipleColumns("TB_SALES", competeColumns, $"\"SALES_GU\" = '{gu}' AND \"SALES_DONG\" = '{dong}'", false);
            string resultCompete = competeQuery[0]["SALES_COMPETE"].ToString();

            // 전역 변수에 할당
            minCostValue_ = minCost;
            maxCostValue_ = maxCost;
            salesIncome_ = salesIncome;
            salesPeople_ = salesPeople;
            facilityCnt_ = facilityCnt.ToString();
            resultRate_ = resultRate;
            resultCnt_ = resultCnt;
            resultCompete_ = resultCompete;

            // 체크박스 비활성화 설정
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            checkBox6.Checked = false;
            checkBox7.Checked = false;
            checkBox8.Checked = false;
            checkBox9.Checked = false;
            checkBox10.Checked = false;
            SetFontList();
        }

        // ~억 ~만원 이라고 표현해주는 함수
        static string FormatWon(int price)
        {
            int eok = price / 10000;
            int man = (price / 10);

            return $"{eok}억 {man}만원";
        }

        // 체크박스의 상태(선택/해제)에 따라 지도 상에 마커를 표시하거나 삭제
        private void ShowCheckBoxMarkers(CheckBox checkBox)
        {
            List<Dictionary<string, object>> facility_rows = GetFacilitiesByTypeAndLocation(checkBox, flatComboBox1, flatComboBox2);

            StringBuilder jsCode = new StringBuilder(); // JavaScript 코드를 동적으로 생성하기 위한 StringBuilder
            string facilityType = checkBox.Tag.ToString();  //체크박스의 태그 값을 사용하여 시설 유형을 가져옴 -> ui에서 수정함

            // 체크박스 선택되었을 때
            if (checkBox.Checked)
            {
                jsCode.AppendLine($"add_markers('{facilityType}', [");
                foreach (var row in facility_rows)
                {
                    string name = row["FACILITY_NAME"].ToString(); // 업체명
                    string addr = row["FACILITY_ADDR"].ToString();  // 주소 
                    string x = row["FACILITY_X"].ToString(); //x좌표
                    string y = row["FACILITY_Y"].ToString(); //y좌표
                    Console.WriteLine(name + addr + x + y); // 확인용

                    // // 각 시설의 정보를 바탕으로 JavaScript 코드를 추가
                    jsCode.AppendLine($"{{ title: '{name}', addr: '{addr}', latlng: new kakao.maps.LatLng({x}, {y}) }},");
                }
                jsCode.AppendLine("]);");
            }
            else // 체크박스 해제되었을 때 마커 삭제 명령 이동
            {
                jsCode.AppendLine($"remove_markers('{facilityType}');");
            }

            // 생성된 JavaScript 코드를 웹 브라우저 컨트롤을 통해 실행
            webBrowser1.Document.InvokeScript("eval", new object[] { jsCode.ToString() });
        }

        // 체크박스의 이름을 참조해서 db에서 값을 가져온다.(인자: 체크박스, 구 콤보박스, 동 콤보박스)
        private List<Dictionary<string, object>> GetFacilitiesByTypeAndLocation(CheckBox checkBox, ComboBox guComboBox, ComboBox dongComboBox)
        {
            // 데이터 가져옴
            string facilityType = checkBox.Tag.ToString(); // Tag에서 시설 타입 가져오기
            string gu = guComboBox.Text;
            string dong = dongComboBox.Text;
            string condition = "";

            // 편의시설 합친 것 때문에 수정해줌
            if (facilityType == "음식점")
            {
                condition = $"\"FACILITY_GU\" = '{gu}' AND \"FACILITY_DONG\" = '{dong}' AND \"FACILITY_TYPE\" IN ('음식점', '패스트푸드', '피자', '제빵', '음식점', '치킨', '분식', '술집')";
            }
            else if (facilityType == "쇼핑몰")
            {
                condition = $"\"FACILITY_GU\" = '{gu}' AND \"FACILITY_DONG\" = '{dong}' AND \"FACILITY_TYPE\" IN ('쇼핑몰', '할인점')";
            }
            else if (facilityType == "중고등학교")
            {
                condition = $"\"FACILITY_GU\" = '{gu}' AND \"FACILITY_DONG\" = '{dong}' AND \"FACILITY_TYPE\" IN ('중학교', '고등학교')";
            }
            else if (facilityType == "문화시설")
            {
                condition = $"\"FACILITY_GU\" = '{gu}' AND \"FACILITY_DONG\" = '{dong}' AND \"FACILITY_TYPE\" IN ('문화시설', '영화관')";
            }
            else
            {
                condition = $"\"FACILITY_GU\" = '{gu}' AND \"FACILITY_DONG\" = '{dong}' AND \"FACILITY_TYPE\" = '{facilityType}'";
            }

            return GetAllRowsFromTable("TB_FACILITY", condition);
        }

        private void resultButton_Click(object sender, EventArgs e)
        {
            if (flatComboBox1.Text == "선택" || flatComboBox2.Text == "선택")
            {
                MessageBox.Show("구와 동을 선택해주세요!", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else if (clickDetected == false)
            {
                MessageBox.Show("검색 버튼을 클릭해주세요!", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else
            {
                string guName = flatComboBox1.Text;
                string dongName = flatComboBox2.Text;

                DialogForm dialogForm = new DialogForm(guName, dongName, minCostValue_, maxCostValue_, salesIncome_, salesPeople_, facilityCnt_, resultRate_, resultCnt_, resultCompete_);
                dialogForm.ShowDialog();
            }

        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // 다중이용시설 체크박스 이벤트 연결
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //편의점
            ShowCheckBoxMarkers(sender as CheckBox);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            //카페
            ShowCheckBoxMarkers(sender as CheckBox);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            //은행
            ShowCheckBoxMarkers(sender as CheckBox);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            //쇼핑몰
            ShowCheckBoxMarkers(sender as CheckBox);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            //병원
            ShowCheckBoxMarkers(sender as CheckBox);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            //음식점
            ShowCheckBoxMarkers(sender as CheckBox);
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            //공용주차장
            ShowCheckBoxMarkers(sender as CheckBox);
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            // 중 고등학교
            ShowCheckBoxMarkers(sender as CheckBox);
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            // 대학교
            ShowCheckBoxMarkers(sender as CheckBox);
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            //문화시설
            ShowCheckBoxMarkers(sender as CheckBox);
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                offset = new Point(e.X, e.Y);
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point newLocation = this.Location;
                newLocation.X += e.X - offset.X;
                newLocation.Y += e.Y - offset.Y;
                this.Location = newLocation;
            }
        }

        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                offset = new Point(e.X, e.Y);
            }
        }

        private void label2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point newLocation = this.Location;
                newLocation.X += e.X - offset.X;
                newLocation.Y += e.Y - offset.Y;
                this.Location = newLocation;
            }
        }
    }
}
