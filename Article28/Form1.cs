using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Media;

namespace Article28
{
    public partial class Form1 : Form
    {
        int second = 0;
        int minute = 0;
        int eggCount = 0;
        int currentLevel = 1;  // Biến để lưu màn hiện tại
        int maxEggsToCollect;  // Số trứng cần thu thập để qua màn


        Timer tmStopwatch = new Timer();
        Label lblDisplay = new Label();
        Label lblEggCount = new Label();
        PictureBox pbBasket =new PictureBox();
        PictureBox pbEgg = new PictureBox();
        PictureBox pbChicken = new PictureBox();
        PictureBox pbBackground = new PictureBox
        {
            Image = Image.FromFile("../../Images/download (1).jpg"), // Hình nền trong 2 giây đầu
            SizeMode = PictureBoxSizeMode.StretchImage,
            Dock = DockStyle.Fill // Đặt hình nền cho toàn bộ Form
        };
        Timer tmEgg = new Timer();
        Timer tmChicken = new Timer();
        Timer tmLayEgg = new Timer();
        Timer tmBackground = new Timer();
        int xBasket = 300;
        int yBasket = 375;
        int xDeltaBasket = 30;

        int xChicken = 300;
        int yChicken = 10;
        int xDeltaChicken = 4; //3

        int xEgg = 300;
        int yEgg = 10;
        int yDeltaEgg = 5;
       

        List<PictureBox> eggs = new List<PictureBox>();
        List<int> eggYPositions = new List<int>(); // Lưu vị trí Y của từng quả trứng
        List<bool> eggCollected = new List<bool>();
        Button btnStart = new Button();
        public Form1()
        {
            InitializeComponent();
            this.Resize += new EventHandler(Form1_Resize);
            this.Load += new EventHandler(Form1_Load);

        }
        private Stopwatch stopwatch = new Stopwatch();
        private SoundPlayer _backgroundMusic;
        private void Form1_Resize(object sender, EventArgs e)
        {
            CenterStartButton();
            UpdateBasketPosition();
        }
        private void UpdateBasketPosition()
        {
            // Đặt giỏ ở dưới cùng của Form, cách đáy một khoảng
            int margin = 10; // Khoảng cách giữa giỏ và đáy Form
            pbBasket.Location = new Point((this.ClientSize.Width - pbBasket.Width) / 2, this.ClientSize.Height - pbBasket.Height - margin);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            btnStart.Text = "Bắt đầu";
            btnStart.Font = new Font("Arial", 18, FontStyle.Bold);
            btnStart.Size = new Size(150, 60);
            btnStart.Location = new Point((this.ClientSize.Width - btnStart.Width) / 2, (this.ClientSize.Height - btnStart.Height) / 2);
            btnStart.BackColor = Color.Green;
            btnStart.ForeColor = Color.DarkBlue;
            btnStart.Click += BtnStart_Click;

            this.Controls.Add(btnStart);
            CenterStartButton();
            UpdateBasketPosition();
            btnStart.BringToFront();

            
            maxEggsToCollect = 2; //số trứng cần thu thập ở màn 1
            tmStopwatch.Interval = 1000;
            tmStopwatch.Tick += tmStopwatch_Tick;
            tmStopwatch.Stop();

            //tmBackground.Interval = 2000; // 2 giây
            //tmBackground.Tick += TmBackground_Tick;
            //tmBackground.Stop();

            lblDisplay.Font = new Font("Arial", 24, FontStyle.Bold);
            lblDisplay.Size = new Size(150, 50);
            lblDisplay.Location = new Point(this.ClientSize.Width - 320, 10);
            lblDisplay.BackColor = Color.FromArgb(100, 0, 0, 0);
            lblDisplay.ForeColor = Color.White;
            lblDisplay.TextAlign = ContentAlignment.MiddleCenter;
            lblDisplay.Text = "00:00";
            lblDisplay.Visible = false;
            this.Controls.Add(lblDisplay);
            lblDisplay.BringToFront();

            // Cấu hình nhãn hiển thị số trứng đã hứng
            lblEggCount.Font = new Font("Arial", 18, FontStyle.Bold);
            lblEggCount.Size = new Size(150, 40);
            lblEggCount.Location = new Point(lblDisplay.Right + 10, lblDisplay.Top); // Đặt bên cạnh đồng hồ
            lblEggCount.BackColor = Color.Transparent;
            lblEggCount.ForeColor = Color.Black;
            lblEggCount.TextAlign = ContentAlignment.MiddleCenter;
            lblEggCount.Text = "Trứng: 00"; // Khởi tạo giá trị ban đầu
            lblEggCount.Visible = false;
            this.Controls.Add(lblEggCount);


            int spaceBelowClock = 20; // Khoảng cách giữa gà và đồng hồ
            yChicken = lblDisplay.Bottom + spaceBelowClock;

            // Thiết lập timer cho gà đẻ trứng
            tmLayEgg.Interval = 2000; // Gà đẻ trứng mỗi 2 giây
            tmLayEgg.Tick += TmLayEgg_Tick;
            tmLayEgg.Stop();

            // Thiết lập timer cho trứng
            tmEgg.Interval = 10;
            tmEgg.Tick += tmEgg_Tick;
            tmEgg.Stop();

            // Thiết lập timer cho gà
            tmChicken.Interval = 10;
            tmChicken.Tick += tmChicken_Tick;
            tmChicken.Stop();


            // Thiết lập giỏ
            pbBasket.SizeMode = PictureBoxSizeMode.StretchImage;
            pbBasket.Size = new Size(70, 70);
            pbBasket.Location = new Point(xBasket, yBasket);
            pbBasket.BackColor = Color.Transparent;
            this.Controls.Add(pbBasket);
            pbBasket.Image = Image.FromFile("../../Images/basket.png");
            pbBasket.Visible = false;
            

            // Thiết lập trứng
            pbEgg.SizeMode = PictureBoxSizeMode.StretchImage;
            pbEgg.Size = new Size(50, 50);
            pbEgg.Location = new Point(xEgg, yEgg);
            pbEgg.BackColor = Color.Transparent;
            this.Controls.Add(pbEgg);
            pbEgg.Visible = false;

            // Thiết lập gà
            pbChicken.SizeMode = PictureBoxSizeMode.StretchImage;
            pbChicken.Size = new Size(100, 100);
            pbChicken.Location = new Point(xChicken, yChicken + lblDisplay.Height); // Đặt gà dưới đồng hồ
            pbChicken.BackColor = Color.Transparent;
            this.Controls.Add(pbChicken);
            pbChicken.Image = Image.FromFile("../../Images/gamai.png");
            pbChicken.Visible = false;

            pbChicken.BringToFront();

            this.KeyPreview = true; // Cho phép Form nhận sự kiện bàn phím
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.Focus();

            this.BackgroundImage = Image.FromFile("../../Images/background.jpg");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.DoubleBuffered = true;
            this.DoubleBuffered = true;
            this.UpdateStyles();
            if (!tmEgg.Enabled)
            {
                Console.WriteLine("tmEgg chưa được khởi động. Khởi động lại...");
                tmEgg.Start();
            }
            // Thêm hình nền vào Form
            this.Controls.Add(pbBackground);
            _backgroundMusic = new SoundPlayer("../../Music/nhac.wav");
            _backgroundMusic.PlayLooping();
            pbBackground.SizeMode = PictureBoxSizeMode.Zoom;

        }
        private void BtnStart_Click(object sender, EventArgs e)
        {
            // Ẩn nút bắt đầu sau khi nhấn
            stopwatch.Start();
            btnStart.Visible = false;
            pbBackground.Visible = false;
            tmStopwatch.Start();
          tmBackground.Start();
            tmEgg.Start();
            tmChicken.Start();
            tmLayEgg.Start();
           
            lblDisplay.Visible = true; // Hiện đồng hồ
            lblEggCount.Visible = true; // Hiện bộ đếm trứng
            pbBasket.Visible = true;
            pbEgg.Visible = true;
            pbChicken.Visible = true;
            btnStart.Visible = false;

            // Khởi động trò chơi
            StartGame();
        }
        private void CenterStartButton()
        {
            btnStart.Location = new Point(
                (this.ClientSize.Width - btnStart.Width) / 2,
                (this.ClientSize.Height - btnStart.Height) / 2
            );
        }
        //private void TmBackground_Tick(object sender, EventArgs e)
        //{
        //    // Dừng timer chạy nền
        //    tmBackground.Stop();

        //    // Xóa background sau 2 giây
        //    this.Controls.Remove(pbBackground);

        //    // Hiển thị lại tất cả các thành phần


        //    // Khởi động trò chơi
        //    StartGame();
        //}


        private void StartGame()
        {
            // Khởi động các timer cho gà và trứng
            tmLayEgg.Start();
            tmEgg.Start();
            tmChicken.Start();
            tmStopwatch.Start();

            // Thay đổi trạng thái bắt đầu trò chơi
            // Có thể đặt lại vị trí giỏ, gà, hoặc bất kỳ thiết lập nào ở đây
        }

        private void tmStopwatch_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = stopwatch.Elapsed;  // Lấy thời gian đã trôi qua

            int minutes = elapsed.Minutes;
            int seconds = elapsed.Seconds;

            // Cập nhật đồng hồ theo định dạng MM:SS
            lblDisplay.Text = $"{minutes:D2}:{seconds:D2}";
        }


        private void TmLayEgg_Tick(object sender, EventArgs e)
        {
            CreateEgg();  // Chỉ tạo trứng mới, không kiểm tra gì khác.
        }


        private void CreateEgg()
        {
            // Kiểm tra duplicate
            if (eggs.Count > 0)
            {
                PictureBox pbEgg = eggs[eggs.Count - 1];
                if (pbEgg.Location == new Point(xChicken + pbChicken.Width / 2 - 25, yChicken + pbChicken.Height))
                {
                    return;
                }
            }

            pbEgg = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(50, 50),
                Location = new Point(xChicken + pbChicken.Width / 2 - 25, yChicken + pbChicken.Height),
                BackColor = Color.Transparent,
                Image = Image.FromFile("../../Images/trung.png")
            };

            this.Controls.Add(pbEgg);
            pbEgg.BringToFront(); // Đưa trứng lên trên cùng

            eggs.Add(pbEgg);
            eggYPositions.Add(pbEgg.Top);
            eggCollected.Add(false);

            Console.WriteLine("Trung duoc tao tai: " + pbEgg.Location.ToString()); // Debug
        }



        private void tmEgg_Tick(object sender, EventArgs e)
        {
            Console.WriteLine($"isGameOver: {isGameOver}\nisLevelCompleted: {isLevelCompleted}");
            if (isGameOver || isLevelCompleted) return;

            List<int> eggsToRemove = new List<int>();

            for (int i = 0; i < eggs.Count; i++)
            {
                
                eggYPositions[i] += yDeltaEgg;
                eggs[i].Location = new Point(eggs[i].Left, eggYPositions[i]);

                // Kiểm tra nếu trứng vào giỏ
                if (IsEggInBasket(eggs[i]) && !eggCollected[i])
                {
                    eggCollected[i] = true;
                    eggsToRemove.Add(i);
                    eggCount++;
                    Console.WriteLine(eggCount);
                    lblEggCount.Text = $"Trứng: {eggCount:D2}";

                    if (eggCount >= maxEggsToCollect)
                    {

                        tmEgg.Stop();
                        tmChicken.Stop();
                        tmLayEgg.Stop();
                        tmStopwatch.Stop();

                        NextLevel();
                        return;
                    }
                }
                else if (eggYPositions[i] > this.ClientSize.Height - eggs[i].Height)
                {
                    ShowBrokenEgg(eggs[i].Location);
                    GameOver();
                    return;
                }
                if (i > 0)
                    Console.WriteLine($"Trung {i} o vi tri: {eggs[i].Location}"); // Debug
            }

            foreach (int index in eggsToRemove.OrderByDescending(x => x))
            {
                this.Controls.Remove(eggs[index]);
                eggs.RemoveAt(index);
                eggYPositions.RemoveAt(index);
                eggCollected.RemoveAt(index);
            }
        }


        private void ShowBrokenEgg(Point eggLocation)
        {
            PictureBox brokenEgg = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(50, 50),
                Location = eggLocation,
                Image = Image.FromFile("../../Images/trung.jpg"), // Đường dẫn hình trứng vỡ
                BackColor = Color.Transparent
            };

            this.Controls.Add(brokenEgg);
            this.Refresh();

            // Sau 1 giây, xóa hình trứng vỡ
            Timer timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) =>
            {
                this.Controls.Remove(brokenEgg);
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }


        private void tmChicken_Tick(object sender, EventArgs e)
        {
            xChicken += xDeltaChicken;
            if(xChicken>this.ClientSize.Width -pbChicken.Width|| xChicken<=0)
                xDeltaChicken = -xDeltaChicken;
            pbChicken.Location =new Point(xChicken,yChicken);
            
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 39 && (xBasket < this.ClientSize.Width - pbBasket.Width))
                xBasket += xDeltaBasket;
            if (e.KeyValue == 37 && xBasket > 0)
                xBasket -= xDeltaBasket;
            pbBasket.Location = new Point(xBasket, yBasket);
        }
        private bool IsEggInBasket(PictureBox egg)
        {
            bool isInBasket = egg.Bounds.IntersectsWith(pbBasket.Bounds);
            //Console.WriteLine($"Trung vao gio: {isInBasket}"); // Debug
            return isInBasket;
        }


        private bool isGameOver = false;  // Cờ kiểm soát trạng thái trò chơi
        private bool isLevelCompleted = false;  // Cờ kiểm soát trạng thái qua màn


        private void GameOver()
        {
            if (isGameOver || isLevelCompleted) return;  // Ngăn gọi trùng

            isGameOver = true;  // Đánh dấu trò chơi đã kết thúc

            // Dừng tất cả các timer
            tmEgg.Stop();
            tmChicken.Stop();
            tmLayEgg.Stop();
            tmStopwatch.Stop();
            _backgroundMusic.Stop();
            MessageBox.Show("Trò chơi kết thúc! Trứng đã rơi ngoài giỏ và bị vỡ.",
                            "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Thêm độ trễ 1 giây trước khi thoát ứng dụng
            Timer exitTimer = new Timer { Interval = 1000 };
            exitTimer.Tick += (s, e) =>
            {
                exitTimer.Stop();
                Application.Exit(); // Thoát ứng dụng
            };
            exitTimer.Start();
        }

        public class Egg
        {
            public PictureBox Picture { get; set; }
            public int YPosition { get; set; }
            public bool IsCollected { get; set; } = false; // Mặc định chưa được thu thập

            public Egg(PictureBox picture)
            {
                Picture = picture;
                YPosition = picture.Top;
            }
        }
        private void NextLevel()
        {
            if (isGameOver || isLevelCompleted) return;

            isLevelCompleted = true;

            // Ẩn tất cả các thành phần hiện tại khi chuyển sang màn 2
            lblDisplay.Visible = false;
            lblEggCount.Visible = false;
            pbBasket.Visible = false;
            pbEgg.Visible = false;
            pbChicken.Visible = false;

            this.BackgroundImage = Image.FromFile("../../Images/background2.jpg");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            if (currentLevel == 1)
            {
                currentLevel++;
                maxEggsToCollect = 10;/*/Số trứng cần thu thập ở màn 2;*/
                MessageBox.Show("Chúc mừng! Bạn đã qua màn 1. Bắt đầu màn 2 với độ khó cao hơn!");

                // Tăng độ khó cho màn 2
                yDeltaEgg = 6; // Tăng tốc độ rơi của trứng
                xDeltaChicken = 6; // Tăng tốc độ di chuyển của gà
                tmLayEgg.Interval = 1500; // Giảm thời gian giữa các lần gà đẻ trứng

                ResetGame();
            }
            else
            {
                MessageBox.Show("Chúc mừng! Bạn đã hoàn thành trò chơi!", "Game Completed", MessageBoxButtons.OK);
                Application.Exit();
            }

            // Khởi động lại các Timer
            //tmLayEgg.Start();
           // tmEgg.Start();
            //tmChicken.Start();
        }

        private void ResetGame()
        {
            eggCount = 0;
            lblEggCount.Text = "Trứng: 00";
            isLevelCompleted = false;

            // Xóa tất cả trứng trên màn hình và làm trống các danh sách
            foreach (var egg in eggs)
            {
                this.Controls.Remove(egg);
                egg.Dispose(); // Giải phóng bộ nhớ của các đối tượng trứng cũ
            }

            eggs.Clear();
            eggYPositions.Clear();
            eggCollected.Clear();

            stopwatch.Restart();

            // Đặt lại vị trí của gà và giỏ
            xBasket = 300;
            pbBasket.Location = new Point(xBasket, yBasket);

            xChicken = 300;
            pbChicken.Location = new Point(xChicken, yChicken);

            // Đảm bảo ẩn các thành phần nếu cần thiết
            lblDisplay.Visible = true; // Ẩn đồng hồ
            lblEggCount.Visible = true; // Ẩn bộ đếm trứng
            pbBasket.Visible = true; // Ẩn giỏ
            pbEgg.Visible = true; // Ẩn trứng
            pbChicken.Visible = true; // Ẩn gà

            // Khởi động lại các Timer
            tmChicken.Start();
            tmEgg.Start();
            tmLayEgg.Start();
            tmStopwatch.Start();
        }


    }
}
