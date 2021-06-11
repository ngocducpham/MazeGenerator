using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Maze
{
    public partial class Form1 : Form
    {
        private const int CELL_SIZE = 20;
        private readonly int MAZE_WIDTH, MAZE_HETGHT;
        private Maze Maze;
        private Thread MazeGeneratorThread;
        private Graphics MazeGraphics;
        private Color WallColor = Color.Black;
        private int TimeCouter = 0;

        public Form1()
        {
            InitializeComponent();
            MAZE_HETGHT = pictureBox1.Height;
            MAZE_WIDTH = pictureBox1.Width;
            Maze = new Maze(MAZE_HETGHT / CELL_SIZE, MAZE_WIDTH / CELL_SIZE);
            MazeGraphics = pictureBox1.CreateGraphics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            MazeGeneratorThread = new Thread(MazeGenerator);
            MazeGeneratorThread.IsBackground = true;
            MazeGeneratorThread.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeCouter++;
            Console.WriteLine("tick");
        }

        private Point ConvertToXY(int i, int j)
        {
            return new Point { Y = i * CELL_SIZE, X = j * CELL_SIZE };
        }

        #region Algorithm

        private void MazeGenerator()
        {
            Stack<Cell> stack = new Stack<Cell>();
            Cell current;
            Cell next;
            Point pos;

            Thread.Sleep(1000);
            timer1.Start();

            while (true)
            {
                Maze.InitData();

                TimeCouter = 0;
                timer1.Enabled = true;
                

                Maze.Cells[0, 0].Wall[0] = false;
                pictureBox1.Invalidate();

                stack.Clear();
                current = Maze.Cells[0, 0];
                current.Visited = true;
                stack.Push(current);

                while (stack.Count != 0)
                {
                    //pos = ConvertToXY(current.Position.I, current.Position.J);
                    //MazeGraphics.FillRectangle(Brushes.Orange, pos.X + 2, pos.Y + 2, CELL_SIZE - 4, CELL_SIZE - 4);
                    Thread.Sleep(1);
                    next = GetNeighbor(current);
                    if (next != null)
                    {
                        next.Visited = true;

                        stack.Push(current);

                        RemoveWall(current, next);

                        current = next;
                    }
                    else if (next == null)
                        current = stack.Pop();
                }

                pictureBox1.Invalidate();
                
                var again = MessageBox.Show($"Time: {TimeCouter} seconds", "Again?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (again == DialogResult.No)
                    break;

                //Thread.Sleep(5000);
                
                
            }
        }

        private void RemoveWall(Cell current, Cell neighbor)
        {
            // top:0, right:1, bottom:2, left:3

            var curPos = current.Position;
            var neiPos = neighbor.Position;
            Point pos = ConvertToXY(curPos.I, curPos.J);
            Color color = Color.White;
            Pen pen = new Pen(color);
            bool removeValue = false;

            // cur - top, nei - bottom
            if (curPos.I > neiPos.I)
            {
                current.Wall[0] = removeValue;
                neighbor.Wall[2] = removeValue;
                MazeGraphics.DrawLine(pen, pos.X + 1, pos.Y, pos.X + CELL_SIZE - 1, pos.Y);
            }

            // cur - right, nei - left
            else if (curPos.J < neiPos.J)
            {
                current.Wall[1] = removeValue;
                neighbor.Wall[3] = removeValue;
                MazeGraphics.DrawLine(pen, pos.X + CELL_SIZE, pos.Y + 1, pos.X + CELL_SIZE, pos.Y + CELL_SIZE - 1);
            }

            // cur - bottom, nei - top
            else if (curPos.I < neiPos.I)
            {
                current.Wall[2] = removeValue;
                neighbor.Wall[0] = removeValue;
                MazeGraphics.DrawLine(pen, pos.X + 1, pos.Y + CELL_SIZE, pos.X + CELL_SIZE - 1, pos.Y + CELL_SIZE);
            }

            // cur - left, nei - right
            else if (curPos.J > neiPos.J)
            {
                current.Wall[3] = removeValue;
                neighbor.Wall[1] = removeValue;
                MazeGraphics.DrawLine(pen, pos.X, pos.Y + 1, pos.X, pos.Y + CELL_SIZE - 1);
            }


        }

        private Cell GetNeighbor(Cell current)
        {
            List<Cell> neighbors = new List<Cell>();
            var pos = current.Position;

            var top = GetCell(pos.I - 1, pos.J);
            if (!top.Visited)
            {
                neighbors.Add(top);
            }

            var right = GetCell(pos.I, pos.J + 1);
            if (!right.Visited)
            {
                neighbors.Add(right);
            }

            var bottom = GetCell(pos.I + 1, pos.J);
            if (!bottom.Visited)
            {
                neighbors.Add(bottom);
            }

            var left = GetCell(pos.I, pos.J - 1);
            if (!left.Visited)
            {
                neighbors.Add(left);
            }

            if (neighbors.Count == 0)
                return null;


            var rd = new Random();
            var index = rd.Next(0, neighbors.Count);
            return Maze.Cells[neighbors[index].Position.I, neighbors[index].Position.J];
        }

        private Cell GetCell(int i, int j)
        {
            if (i < 0 || j < 0 || i > Maze.MazeI - 1 || j > Maze.MazeJ - 1)
                return Maze.UndefinedCell;
            return Maze.Cells[i, j];
        }
        #endregion

        #region Draw
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawGrid(e.Graphics);
        }

        private void DrawGrid(Graphics g)
        {
            int x = 0, y = 0;
            Pen pen = new Pen(WallColor);
            for (int i = 0; i < Maze.MazeI; i++)
            {
                for (int j = 0; j < Maze.MazeJ; j++)
                {
                    // top
                    if (Maze.Cells[i, j].Wall[0])
                    {
                        g.DrawLine(pen, x, y, x + CELL_SIZE, y);
                    }

                    // right
                    if (Maze.Cells[i, j].Wall[1])
                    {
                        g.DrawLine(pen, x + CELL_SIZE, y, x + CELL_SIZE, y + CELL_SIZE);
                    }

                    // bottom
                    if (Maze.Cells[i, j].Wall[2])
                    {
                        g.DrawLine(pen, x, y + CELL_SIZE, x + CELL_SIZE, y + CELL_SIZE);
                    }

                    // left
                    if (Maze.Cells[i, j].Wall[3])
                    {
                        g.DrawLine(pen, x, y, x, y + CELL_SIZE);
                    }

                    x += CELL_SIZE;
                }
                y += CELL_SIZE;
                x = 0;
            }
        }


        #endregion
    }
}
