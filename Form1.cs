using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms.VisualStyles;

namespace SeaWars
{
    public partial class Form1 : Form
    {
        Player user = new Player();
        Player enemy = new Bot();
        Point currentCell = new Point();
        int currentDamage = 100;
        Button currentButton;
        ListBox logText;
        Ship currentShip;
        List<Button> shipBtnChoice;
        int currentDif;
        int deathCount = 0;
        int currentItemInfo;
        int currentTurn = 0;
        protected Button[,] myMapButtons = new Button[Player.getMapSize(), Player.getMapSize()];
        protected Button[,] enemyMapButtons = new Button[Player.getMapSize(), Player.getMapSize()];

        // -1 = 75 hp left
        // -2 = 50 hp left
        // -3 = 25 hp left
        // -4 =  0 hp left
        // -5 =       miss
        //  9 = scanned
        void CheckMap()
        {
            
            int aliveCounter = 0;
            int deathCount = 0;
            for (int i = 1; i < Player.getMapSize(); i++)
            {
                for (int j = 1; j < Player.getMapSize(); j++)
                {
                    if (user.getMyMapCellDefault(i, j) == 1) { aliveCounter++; myMapButtons[i, j].BackColor = Color.Red; myMapButtons[i, j].Text = ""; }
                    if (user.getMyMapCellDefault(i, j) == -1) { myMapButtons[i, j].BackColor = Color.DarkGreen; }
                    if (user.getMyMapCellDefault(i, j) == -2) { myMapButtons[i, j].BackColor = Color.Yellow; }
                    if (user.getMyMapCellDefault(i, j) == -3) { myMapButtons[i, j].BackColor = Color.DarkRed; }
                    if (user.getMyMapCellDefault(i, j) == -4) { myMapButtons[i, j].BackColor = Color.Black; }
                    if (user.getMyMapCellDefault(i, j) == -5 && myMapButtons[i, j].BackColor != Color.Black) { myMapButtons[i, j].BackColor = Color.Blue; }
                    if (user.getMyMapCellDefault(i, j) != 1 && user.getMyMapCellDefault(i, j) != 0 && myMapButtons[i, j].Text != "X" && user.getMyMapCellDefault(i, j) != -5) { myMapButtons[i, j].Text = "X"; }


                    if (enemy.getMyMapCellDefault(i, j) == -1) { enemyMapButtons[i, j].BackColor = Color.DarkGreen; user.setEnemyMapCell(i, j, -1); }
                    if (enemy.getMyMapCellDefault(i, j) == 9)  { enemyMapButtons[i, j].BackColor = Color.HotPink; user.setEnemyMapCell(i, j, 9); }
                    if (enemy.getMyMapCellDefault(i, j) == -2) { enemyMapButtons[i, j].BackColor = Color.Yellow; user.setEnemyMapCell(i, j, -2); }
                    if (enemy.getMyMapCellDefault(i, j) == -3) { enemyMapButtons[i, j].BackColor = Color.DarkRed; user.setEnemyMapCell(i, j, -3); }
                    if (enemy.getMyMapCellDefault(i, j) == -4) { enemyMapButtons[i, j].BackColor = Color.Black; user.setEnemyMapCell(i, j, -4); deathCount++; }
                    if (enemy.getMyMapCellDefault(i, j) == -5 && enemyMapButtons[i, j].BackColor != Color.Black) { enemyMapButtons[i, j].BackColor = Color.Blue; user.setEnemyMapCell(i, j, -5); }
                    if (enemy.getMyMapCellDefault(i, j) != 1 && enemy.getMyMapCellDefault(i, j) != 0 && enemyMapButtons[i, j].Text != "X" && enemy.getMyMapCellDefault(i, j) != -5) { enemyMapButtons[i, j].Text = "X"; }
                    if (enemy.getMyMapCellDefault(i, j) == 20 || enemy.getMyMapCellDefault(i, j) == 21 || enemy.getMyMapCellDefault(i, j) == 22) { enemyMapButtons[i, j].BackColor = Color.Bisque; }

                }

            }

            if (aliveCounter == 0)
            {
                MessageBox.Show("Loss");
            }
            if (deathCount == 17) { MessageBox.Show("Congratulations!!! You won!"); }
        }

        void DrawLogText()
        {
            logText = new ListBox();

            logText.Location = new Point(10, 450);
            logText.Size = new Size(600, 150);
            this.Controls.Add(logText);
        }
        void DrawMaps()
        {
            for (int i = 0; i < Player.getMapSize(); i++)
            {
                for (int j = 0; j < Player.getMapSize(); j++)
                {
                    Button myButton = new Button();
                    Button enemyButton = new Button();

                    if (j == 0 || i == 0)
                    {
                        myButton.BackColor = Color.Gray;
                        enemyButton.BackColor = Color.Gray;

                        if (i == 0 && j > 0)
                        {
                            myButton.Text = Player.getAlphabet()[j - 1].ToString();
                            enemyButton.Text = Player.getAlphabet()[j - 1].ToString();
                        }

                        if (j == 0 && i > 0)
                        {
                            myButton.Text = i.ToString();
                            enemyButton.Text = i.ToString();
                        }
                    }
                    else
                    {
                        myMapButtons[i, j] = myButton;
                        myButton.Click += DrawShips;
                        myButton.BackColor = Color.White;

                        enemyMapButtons[i, j] = enemyButton; 
                        enemyButton.BackColor = Color.White;
                        enemyButton.Click += shoot_Click;
                    }
                    myButton.Location = new Point(j * Player.getCellSize(), i * Player.getCellSize());
                    myButton.Size = new Size(Player.getCellSize(), Player.getCellSize());
                    Invoke(() => this.Controls.Add(myButton));

                    enemyButton.Location = new Point(320 + j * Player.getCellSize(), i * Player.getCellSize());
                    enemyButton.Size = new Size(Player.getCellSize(), Player.getCellSize());
                    Invoke(() => this.Controls.Add(enemyButton));
                }
            }
            Button startButton = new Button();
            startButton.Text = "Начать";
            startButton.Location = new Point(ClientSize.Width / 2 - startButton.Width / 2, Player.getMapSize() * Player.getCellSize() + 20);
            startButton.Click += new EventHandler(Start);
            Invoke(() => this.Controls.Add(startButton));
        }

        void Start(object sender, EventArgs e)
        {
            if (!user.checkShips())
            {
                MessageBox.Show("Неправильно расставлены корабли!");
                Application.Restart();
            }
            user.isPlaying = true;
            this.Controls.Remove(sender as Button);
        }
        void ChooseMenu()
        {
            shipBtnChoice = new List<Button>();
            Button defaultShoot = new Button();
            Button shipAbility = new Button();

            defaultShoot.Text = "Обычный выстрел";
            defaultShoot.AutoSize = true;
            defaultShoot.Location = new Point(150, 320);
            defaultShoot.Click += defaultShoot_Click;

            shipAbility.Text = "Использовать спец. способность";
            shipAbility.AutoSize = true;
            shipAbility.Location = new Point(300, 320);
            shipAbility.Click += abilityChoice_Click;

            shipBtnChoice.Add(defaultShoot);
            shipBtnChoice.Add(shipAbility);

            this.Controls.Add(defaultShoot);
            this.Controls.Add(shipAbility);
        }

        void abilityChoice_Click(object sender, EventArgs e)
        {
            ShowShipsWithAbility();
        }

        void ShowShipsWithAbility()
        {
            string[] ships = user.getAvailableShipsWithAbility();
            int offset = 10;
            foreach (var ship in ships)
            {
                Button shipButton = new Button
                {
                    Text = ship,
                    Location = new Point(offset, 400),
                };
                shipButton.Click += abilityShipChoice_Click;
                offset += 75;
                shipBtnChoice.Add(shipButton);
                Controls.Add(shipButton);
            }
        }

        void abilityShipChoice_Click(object sender, EventArgs e)
        {
            Button shipChoice = sender as Button;

            if (shipChoice.Text == "BattleShip") { currentShip = user.getBattleShip(); }
            else if (shipChoice.Text == "Cruiser") { currentShip = user.getCruiser(); }
            else if (shipChoice.Text == "Destroyer") { currentShip = user.getDestroyer(); }
            else if (shipChoice.Text == "Boat") { currentShip = user.getBoat(); }

            currentShip.Ability(enemy, currentCell);
            //changeButtonAfterShoot(ref currentButton, hit);

            botTurn();
            foreach (var btn in shipBtnChoice) { Controls.Remove(btn); }
        }

        // Deprecated (all changes happen in CheckMap)
        void changeButtonAfterShoot(ref Button buttonToChange, ShootResult hit)
        {
            if (hit.hpLeft == 75)
            {
                buttonToChange.BackColor = Color.DarkGreen;
                buttonToChange.Text = "X";
            }
            else if (hit.hpLeft == 50)
            {
                buttonToChange.BackColor = Color.Yellow;
                buttonToChange.Text = "X";
            }
            else if (hit.hpLeft == 25)
            {
                buttonToChange.BackColor = Color.DarkRed;
                buttonToChange.Text = "X";
            }
            else if (hit.hpLeft == 0 || buttonToChange.BackColor == Color.Black)
            {
                user.setEnemyMapCell(currentCell.X, currentCell.Y, 1);
                buttonToChange.BackColor = Color.Black;
                buttonToChange.Text = "X";
                deathCount++;
            }
            else { buttonToChange.BackColor = Color.Blue; user.setEnemyMapCell(currentCell.X, currentCell.Y, -1); }
        }

        void botTurn()
        {
           // if (currentTurn % 3 == 0 || user.GetShips().Length < 3) { user.restoreShoots(); }
            Point enemyShoot = (enemy as Bot).DefaultShoot();
            var enemy_hit = user.EnemyShoot(enemyShoot, currentDif);
            if (enemy_hit.Hit)
            {
                if (enemy_hit.hpLeft == 0) { enemy.setEnemyMapCell(enemyShoot.X, enemyShoot.Y, -4); }
                else if (enemy_hit.hpLeft == 25) { enemy.setEnemyMapCell(enemyShoot.X, enemyShoot.Y, -3); }
                else if (enemy_hit.hpLeft == 50) { enemy.setEnemyMapCell(enemyShoot.X, enemyShoot.Y, -2); }
                else if (enemy_hit.hpLeft == 75) { enemy.setEnemyMapCell(enemyShoot.X, enemyShoot.Y, -1); }
            }
            else enemy.setEnemyMapCell(enemyShoot.X, enemyShoot.Y, -5);
            logText.Items.Add($"\nBot shooted cell {Player.getAlphabet()[enemyShoot.Y - 1]} {enemyShoot.X}");
            CheckMap();
            foreach (var btn in shipBtnChoice) { Controls.Remove(btn); }

            (enemy as Bot).ItemDropIfCounter();

        }

        void ItemInfo(ShootResult hit)
        {
            switch(hit.hpLeft)
            {
               
                case -10:
                    MessageBox.Show("Вы получили аптечку! Выберите корабль, который хотите отремонтировать!");
                    ShowShips(-10);
                    break;
                case -11:
                    MessageBox.Show("Вы получили самонаводящийся снаряд! Выберите корабль, которым хотите выстрелить!");
                    ShowShips(-11);
                    break;
                case -12:
                    MessageBox.Show("Вы получили заряд для спец. способности! Выберите корабль, способность которого хотите восстановить!");
                    ShowShips(-12);
                    break;
            }
        }

        void defaultShootShipChoice_Click(object sender, EventArgs e)
        {
            Button shipChoice = sender as Button;
            if (shipChoice.Text == "BattleShip") { currentShip = user.getBattleShip(); }
            else if (shipChoice.Text == "Cruiser") { currentShip = user.getCruiser(); }
            else if (shipChoice.Text == "Destroyer") { currentShip = user.getDestroyer(); }
            else if (shipChoice.Text == "Boat") { currentShip = user.getBoat(); }
            if (user.getEnemyMapCell(currentCell.X, currentCell.Y) == -4 || user.getEnemyMapCell(currentCell.X, currentCell.Y) == -5) { MessageBox.Show("Choose another cell!"); foreach (var btn in shipBtnChoice) { Controls.Remove(btn); } return; }
            var hit = enemy.EnemyShoot(currentCell, Int32.Parse(currentShip.Shoot()));

            if (hit.Hit == true && hit.hpLeft < 0)
            {
                ItemInfo(hit);
                return;
            }

           // changeButtonAfterShoot(ref currentButton, hit);
            botTurn();
            
        }

        void ShowShips(int itemInfo)
        {
            foreach (var btn in shipBtnChoice) { Controls.Remove(btn); }
            string[] ships = user.GetShips();
            currentItemInfo = itemInfo;
            int offset = 10;
            foreach (var ship in ships)
            {
                Button shipButton = new Button();
                shipButton.Text = ship;
                shipButton.Location = new Point(offset, 400);
                shipButton.Click += itemShipChoice_Click;
                offset += 75;
                shipBtnChoice.Add(shipButton);
                this.Controls.Add(shipButton);
            }

        }

        void itemShipChoice_Click(object sender, EventArgs e)
        {
            Button shipChoice = sender as Button;
            if (shipChoice.Text == "BattleShip") { currentShip = user.getBattleShip(); }
            else if (shipChoice.Text == "Cruiser") { currentShip = user.getCruiser(); }
            else if (shipChoice.Text == "Destroyer") { currentShip = user.getDestroyer(); }
            else if (shipChoice.Text == "Boat") { currentShip = user.getBoat(); }

            switch (currentItemInfo)
            {
                case -10:
                    new AidKit().Use(currentShip, user);
                    break;
                case -11:
                    new HomingMissile().Use(currentShip, enemy);
                    break;
                case -12:
                    new AbilityCharger().Use(currentShip, user);
                    break;
            }
            foreach (var btn in shipBtnChoice) { Controls.Remove(btn); }
            botTurn();
        }

        void ShowShipsToShoot()
        {
            string[] ships = user.getAvailableShipsToShoot();
            int offset = 10;
            foreach (var ship in ships)
            {
                Button shipButton = new Button();
                shipButton.Text = ship;
                shipButton.Location = new Point(offset, 400);
                shipButton.Click += defaultShootShipChoice_Click;
                offset += 75;
                shipBtnChoice.Add(shipButton);
                this.Controls.Add(shipButton);
            }
        }
        void defaultShoot_Click(object sender, EventArgs e)
        {
            ShowShipsToShoot();
        }

        void shoot_Click(object sender, EventArgs e)
        {
            if (user.isPlaying)
            {
                Button button = sender as Button;
                currentCell = new Point(button.Location.Y / Player.getCellSize(), button.Location.X / Player.getCellSize() - 10);
                currentButton = sender as Button;
                ChooseMenu();
            }
        }
        void DrawShips(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;

            if (!user.isPlaying)
            {
                if (user.getMyMapCell(pressedButton.Location.Y, pressedButton.Location.X) == 0)
                {
                    pressedButton.BackColor = Color.Red;
                    user.setMyMapCell(pressedButton.Location.Y, pressedButton.Location.X, 1);
                }
                else
                {
                    pressedButton.BackColor = Color.White;
                    user.setMyMapCell(pressedButton.Location.Y, pressedButton.Location.X, 0);
                }
            }
        }

        void startGame()
        {
            this.Controls.Clear();
            MessageBox.Show("Расставьте по порядку: один 4-ех палубный корабль, два 3-ех палубных, два 2-ух палубных и три однопалубных корабля");
            DrawMaps();
            DrawLogText();
        }

        void easyDif_Click(object sender, EventArgs e)
        {
            currentDif = 25;
            startGame();
        }

        void midDif_Click(object sender, EventArgs e)
        {
            currentDif = 50;
            startGame();
        }

        void hardDif_Click(object sender, EventArgs e)
        {
            currentDif = 100;
            startGame();
        }

        void chooseDif()
        {
            this.Controls.Clear();

            Label chsDif = new Label
            {
                Font = new Font(FontFamily.GenericSerif, 21, FontStyle.Regular),
                AutoSize = true,
                Text = "Выберите сложность игры",
                Location = new Point(150, 100),
            };

            Button easyDif = new Button
            {
                Text = "Easy",
                Location = new Point(275, 200),
            };
            easyDif.Click += easyDif_Click;
            Button midDif = new Button
            {
                Text = "Middle",
                Location = new Point(275, 300),
            };
            midDif.Click += midDif_Click;
            Button hardDif = new Button
            {
                Text = "Hard",
                Location = new Point(275, 400),
            };
            hardDif.Click += hardDif_Click;
            Controls.Add(easyDif);
            Controls.Add(midDif);
            Controls.Add(hardDif);
            Controls.Add(chsDif);
        }
        void singlePlay_Click(object sender, EventArgs e)
        {
            chooseDif();
        }

        void InitMenu()
        {
            Button singlePlay = new Button();
            Button onlinePlay = new Button();
            Label choosePlayMode = new Label
            {
                Font = new Font(FontFamily.GenericSerif, 21, FontStyle.Regular),
                AutoSize = true,
                Text = "Выберите режим игры",
                Location = new Point(180, 100),
            };

            singlePlay.Text = "Одиночная игра";
            singlePlay.Size = new Size(260, 80);
            singlePlay.Location = new Point(180, 450);
            singlePlay.Click += singlePlay_Click;

            onlinePlay.Text = "Игра по сети";
            onlinePlay.AutoSize = true;
            onlinePlay.Location = new Point(270, 250);

            this.Controls.Add(choosePlayMode);
            this.Controls.Add(singlePlay);
            //this.Controls.Add(onlinePlay);

        }
        public Form1()
        {
     
            InitializeComponent();
            this.Size = new Size(640, 680);
            this.Text = "Sea Wars by thienlao";
            BackgroundImage = Image.FromFile("banner5.jpg");
            BackgroundImageLayout = ImageLayout.Center;
            this.DoubleBuffered = true;
            InitMenu();
        }
    }
}