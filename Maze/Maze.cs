namespace Maze
{
    public class Maze
    {
        public readonly int MazeI, MazeJ;
        public Cell[,] Cells { get; set; }

        private CellPositon startPos;

        public Cell UndefinedCell { get; set; }

        public CellPositon StartPosition
        {
            get => startPos;
            private set
            {
                if (startPos != null)
                {
                    Cells[startPos.I, startPos.J].Value = CellValue.None;
                }

                Cells[value.I, value.J].Value = CellValue.Start;
                startPos = value;
            }
        }

        private CellPositon goalPos;
        public CellPositon GoalPosition
        {
            get => goalPos;
            private set
            {
                if (goalPos != null)
                {
                    Cells[goalPos.I, goalPos.J].Value = CellValue.None;
                }
                Cells[value.I, value.J].Value = CellValue.Goal;
                goalPos = value;
            }
        }

        public void SetCellValue(CellPositon position, CellValue value)
        {
            // vị trí đích hoăc bắt đầu bị đè
            if (startPos != null)
            {
                if (position == startPos && value != CellValue.Start)
                {
                    startPos = null;
                }
            }
            else if (goalPos != null)
            {
                if (position == goalPos && value != CellValue.Goal)
                {
                    goalPos = null;
                }
            }

            if (value == CellValue.Start)
            {
                StartPosition = position;
            }
            else if (value == CellValue.Goal)
            {
                GoalPosition = position;
            }

            Cells[position.I, position.J].Value = value;
        }

        public Maze(int i, int j)
        {
            Cells = new Cell[i, j];
            MazeI = i;
            MazeJ = j;
            UndefinedCell = new Cell
            {
                Position = new CellPositon { I = -1, J = -1 },
                Value = CellValue.None,
                Visited = true,
                gScore = -1,
                fScore = -1,
                hScore = -1,
                CameFrom = null,
                Wall = null
            };

            InitData();
        }

        public void InitData()
        {
            for (int i = 0; i < MazeI; i++)
            {
                for (int j = 0; j < MazeJ; j++)
                {
                    Cells[i, j] = new Cell()
                    {
                        Value = CellValue.None,
                        Position = new CellPositon { I = i, J = j }
                    };
                }
            }
        }

    }

    public class Cell
    {
        public CellValue Value { get; set; }

        public bool Visited { get; set; }

        // điểm g
        public int gScore { get; set; }

        // f= g + h
        public double hScore { get; set; }

        public double fScore { get; set; }

        public CellPositon Position { get; set; }

        public Cell CameFrom { get; set; }

        // top, right, bottom, left
        public bool[] Wall = { true, true, true, true };

        public Cell()
        {
            Position = new CellPositon();
        }

    }

    public enum CellValue
    {
        None,
        Goal,
        Start,
        Wall,
        Neighbor,
        Visited
    }

    public class CellPositon
    {
        public int I { get; set; }
        public int J { get; set; }

        public static bool operator ==(CellPositon a, CellPositon b)
        {
            if (a is null)
            {
                return b is null;
            }

            return a.Equals(b);
        }

        public static bool operator !=(CellPositon a, CellPositon b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is CellPositon b ? (I == b.I && J == b.J) : false;
        }

        public override int GetHashCode()
        {
            return (I, J).GetHashCode();
        }
    }
}
