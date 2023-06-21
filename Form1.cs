using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Form1 : Form
    {
        Graphics graphics;
        
        private int currentGeneration = 0;
        private int resolution;
        private bool[,] field;
        private int rows;
        private int columns;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = $"Game of Life";
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            StartGame();
            DrawField();
            UpdateGenerationCount();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            DrawField();
            UpdateGenerationCount();
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopGame();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                var validationPosition = ValidateMousePosition(x, y);
                if (validationPosition)
                {
                    field[x, y] = true;
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                var validationPosition = ValidateMousePosition(x, y);
                if (validationPosition)
                {
                    field[x, y] = false;
                }
            }
        }

        private void StartGame()
        {
            if (timer1.Enabled)
            {
                return;
            }

            InitializeGame();
            nudResolution.Enabled = false;
            nudDensity.Enabled = false;

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            timer1.Start();
        }

        private void InitializeGame()
        {
            currentGeneration = 0;
            Text = $"Generation {currentGeneration}";

            resolution = Convert.ToInt32(nudResolution.Value);
            rows = pictureBox1.Height / resolution;
            columns = pictureBox1.Width / resolution;
            field = new bool[columns, rows];

            RandomizeField();
        }

        private void UpdateGenerationCount()
        {
            Text = $"Generation {currentGeneration}";
        }

        private void RandomizeField()
        {
            using (RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[columns * rows];
                rngCryptoServiceProvider.GetBytes(randomBytes);

                int index = 0;
                for (int x = 0; x < columns; x++)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        field[x, y] = randomBytes[index] % (int)nudDensity.Value == 0;
                        index++;
                    }
                }
            }
        }

        private void DrawField()
        {
            graphics.Clear(Color.White);
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (field[x, y])
                    {
                        graphics.FillRectangle(Brushes.Crimson, x * resolution, y * resolution, resolution, resolution);
                    }
                }
            }
            pictureBox1.Refresh();
        }

        private void NextGeneration()
        {
            var newField = new bool[columns, rows];

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    int neighboursCount = CountNeighbours(x, y);
                    bool hasLife = field[x, y];

                    if (!hasLife && neighboursCount == 3)
                    {
                        newField[x, y] = true;
                    }
                    else if (hasLife && (neighboursCount < 2 || neighboursCount > 3))
                    {
                        newField[x, y] = false;
                    }
                    else
                    {
                        newField[x, y] = field[x, y];
                    }
                }
            }
            field = newField;
            currentGeneration++;
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int col = (x + i + columns) % columns;
                    int row = (y + j + rows) % rows;
                    bool isSelfChecking = col == x && row == y;
                    bool hasLife = field[col, row];
                    if (hasLife && !isSelfChecking)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private void StopGame()
        {
            if (!timer1.Enabled)
            {
                return;
            }

            timer1.Stop();
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
        }

        private bool ValidateMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < columns && y < columns;
        }
    }
}
