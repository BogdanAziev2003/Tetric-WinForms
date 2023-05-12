using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tetris
{
    public partial class Tetris : Form
    {
        Shape currentShape;
        static int rows = 8;
        static int columns = 16;
        int size;
        int[,] map = new int[columns, rows];
        int linesRemoved;
        int score;
        int Interval;
        public Tetris()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(keyFunc);
            Init(); 

        }

        private void Init()
        {


            size = 25;
            score = 0;
            linesRemoved = 0;
            currentShape = new Shape(3, 0);
            Interval = 300;

            label1.Text = "Score: " + score;
            label2.Text = "Lines Removed: " + linesRemoved;

            timer1.Interval = Interval;
            timer1.Tick += new EventHandler(update);
            timer1.Start();


            Invalidate();
        }

        private void keyFunc(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    if (!IsIntersects())
                    {
                        ResetArea();
                        currentShape.RotateShape();
                        Merge();
                        Invalidate();
                    }
                    break;
                case Keys.Space:
                    timer1.Interval = 10;
                    break;
                case Keys.Left:
                    if (!CollideHor(-1))
                    {
                        ResetArea();
                        currentShape.MoveLeft();
                        Merge();
                        Invalidate();
                    }
                    break;
                case Keys.Right:
                    if (!CollideHor(1))
                    {
                        ResetArea();
                        currentShape.MoveRight();
                        Merge();
                        Invalidate();
                    }
                    break;
            }
        }

        private void update(object sender, EventArgs e)/*Функция срабатывает каждый тик*/
        {

            ResetArea();
            if (!Collide())
            {
                currentShape.MoveDown();
            }
            else
            {
                Merge();
                SliceMap();
                timer1.Interval = Interval;
                currentShape.ResetShape(3, 0);
                if (Collide())
                {
                    for(int i = 0; i < 16; i++)
                    {
                        for(int j = 0; j < 8; j++)
                        {
                            map[i, j] = 0;
                        }
                    }
                    timer1.Tick -= new EventHandler(update);
                    timer1.Stop();
                    MessageBox.Show("Вы проирали, ваш счет: " + score);
                    Init();
                }
            }
            Merge();
            Invalidate();
        }

        public void ShowNextShape(Graphics e)
        {
            for (int i = 0; i < currentShape.sizeNextMatrix; i++)
            {
                for (int j = 0; j < currentShape.sizeNextMatrix; j++)
                {
                    if (currentShape.nextMatrix[i, j] == 1)
                    {
                        e.FillRectangle(Brushes.Red, new Rectangle(300 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (currentShape.nextMatrix[i, j] == 2)
                    {
                        e.FillRectangle(Brushes.Blue, new Rectangle(300 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (currentShape.nextMatrix[i, j] == 3)
                    {
                        e.FillRectangle(Brushes.Yellow, new Rectangle(300 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (currentShape.nextMatrix[i, j] == 4)
                    {
                        e.FillRectangle(Brushes.Green, new Rectangle(300 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (currentShape.nextMatrix[i, j] == 5)
                    {
                        e.FillRectangle(Brushes.Violet, new Rectangle(300 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                }
            }
        }

        public void SliceMap()
        {
            int count = 0;
            int curRemovesLines = 0;
            for(int i = 0; i < 16; i++)
            {
                count = 0;
                for(int j = 0; j < 8; j++)
                {
                    if (map[i, j] != 0)
                    count++;
                }
                if(count == 8)
                {
                    curRemovesLines++;
                    for (int k = i; k >= 1; k--)
                    {
                        for (int o = 0; o < 8; o++)
                        {
                            map[k, o] = map[k - 1, o];
                        }
                    }
                }
            }
            for(int i = 0; i < curRemovesLines; i++)
            {
                score += 10 * (i + 1);
            }
            linesRemoved += curRemovesLines;

            if (linesRemoved % 5 == 0)
            {
                if(Interval > 60)
                {
                    Interval -= 10;
                }
            }


            label1.Text = "Score: " + score;
            label2.Text = "Lines Removed: " + linesRemoved;
        }

        public bool IsIntersects()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (j >= 0 && j <= 7)
                    {
                        if (map[i, j] != 0 && currentShape.matrix[i - currentShape.y, j - currentShape.x] == 0)
                            return true;
                    }
                }
            }
            return false;
        }


        private void Merge() /*Эта функция изменят матрицу карты в зависимости от фигуры, ее местоположения и ворме*/
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x]!=0) /*Проверка, что б мерджить только не нулевые ячейки*/
                    map[i, j] = currentShape.matrix[i - currentShape.y, j - currentShape.x];
                }
            }
        }

        private bool Collide()/*Функция, которая проверяет выход за границы карты по вертикали*/
        {
            for (int i = currentShape.y + currentShape.sizeMatrix - 1; i >= currentShape.y; i--)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                    {
                        if (i + 1 == columns)
                        {
                            return true;
                        }
                        if (map[i + 1, j] != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private bool CollideHor(int dir)/*Функция, которая проверяет выход за границы карты по горизонтали*/
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                    {
                        if (j + 1 * dir > 7 || j + 1 * dir < 0)
                            return true;

                        if (map[i, j + 1 * dir] != 0)
                        {
                            if (j - currentShape.x + 1 * dir >= currentShape.sizeMatrix || j - currentShape.x + 1 * dir < 0)
                            {
                                return true;
                            }
                            if (currentShape.matrix[i - currentShape.y, j - currentShape.x + 1 * dir] == 0)
                                return true;
                        }
                    }
                }
            }
            return false;
        }





        private void ResetArea() /*Эта функция нужна что б убрать цвет на клетках, на который фигура уже не находится*/
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {   
                    if(i >= 0 && j >= 0 && i < 16 && j < 8)
                    {
                        if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                        {
                            map[i, j] = 0;
                        }
                    }
                }
            }
        }


        private void DrowMap(Graphics e) /*Эта функция заполняет квадратиками поле, в зависимости от значения матрицы*/
        {
            for(int i = 0; i < 16; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if (map[i, j] == 1)
                    {
                        e.FillRectangle(Brushes.Red, new Rectangle(50 + (j * size) + 1, 50 + (i * size) + 1, size, size));
                    }
                    if (map[i, j] == 2)
                    {
                        e.FillRectangle(Brushes.Blue, new Rectangle(50 + (j * size) + 1, 50 + (i * size) + 1, size, size));
                    }
                    if (map[i, j] == 3)
                    {
                        e.FillRectangle(Brushes.Yellow, new Rectangle(50 + (j * size) + 1, 50 + (i * size) + 1, size, size));
                    }
                    if (map[i, j] == 4)
                    {
                        e.FillRectangle(Brushes.Green, new Rectangle(50 + (j * size) + 1, 50 + (i * size) + 1, size, size));
                    }
                    if (map[i, j] == 5)
                    {
                        e.FillRectangle(Brushes.Violet, new Rectangle(50 + (j * size) + 1, 50 + (i * size) + 1, size, size));
                    }
                }
            }
        }



        private void DrawGrid(Graphics g)/*Отрисовка карты*/
        {
            for(int i = 0; i <= columns; i++)/*Отрисовка линий горизонтальных*/
            {
                g.DrawLine(Pens.Black, new Point(50, 50 + i * size), new Point(50 + 8 * size, 50 + i * size));
            }
            for (int i = 0; i <= rows; i++)/*Отрисовка линий вертикальных*/
            {
                g.DrawLine(Pens.Black, new Point(50 + i * size, 50), new Point(50 + i * size, 50 + 16 * size));
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            DrawGrid(e.Graphics);
            DrowMap(e.Graphics);
            ShowNextShape(e.Graphics);
        }
    }
}
