namespace Game.Persistence
{
    public enum BoardSize : int { Small = 3, Medium = 5, Large = 9 }

    public class Board
    {
        private int _gridSize;
        private bool[,] _horizontalLines;
        private bool[,] _verticalLines;
        private Box[,] _boxes;

        public int GridSize => _gridSize;
        public bool[,] GetHLines() => _horizontalLines;
        public bool[,] GetVLines() => _verticalLines;
        public Box[,] GetBoxes() => _boxes;

        public Board(BoardSize size)
        {
            _gridSize = (int)size;
            _horizontalLines = new bool[_gridSize + 1, _gridSize];
            _verticalLines = new bool[_gridSize, _gridSize + 1];


            _boxes = new Box[_gridSize, _gridSize];
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    _boxes[i, j] = new Box();
                }
            }
        }
        // 3 row 4 col
        public bool AddHorizontalLine(int row, int col)
        {
            if (row < 0 || row > _gridSize || col < 0 || col >= _gridSize)
                return false;
            if (_horizontalLines[row, col])
                return false;

            _horizontalLines[row, col] = true;

            if (row >= 0 && row < _gridSize)
                _boxes[row, col].SetEdge(Side.Top);
            if (row - 1 >= 0 && row - 1 < _gridSize)
                _boxes[row - 1, col].SetEdge(Side.Bottom);

            return true;
        }

        // 4 row 3 col
        public bool AddVerticalLine(int row, int col)
        {
            if (row < 0 || row >= _gridSize || col < 0 || col > _gridSize)
                return false;
            if (_verticalLines[row, col])
                return false;

            _verticalLines[row, col] = true;
            // add to boxes

            if (col >= 0 && col < _gridSize)
                _boxes[row, col].SetEdge(Side.Left);
            if (col - 1 >= 0 && col - 1 < _gridSize)
                _boxes[row, col - 1].SetEdge(Side.Right);
            return true;
        }

        public int CheckHorizontalCompletion(int row, int col, int playerId)
        {
            int completed = 0;

            // Box above
            if (row > 0 && AllEdgesFilled(row - 1, col))
                completed = ClaimBox(row - 1, col, playerId);

            // Box below
            if (row < _gridSize && AllEdgesFilled(row, col))
                completed += ClaimBox(row, col, playerId);

            return completed;
        }

        public int CheckVerticalCompletion(int row, int col, int playerId)
        {
            int completed = 0;

            // Box to the left
            if (col > 0 && AllEdgesFilled(row, col - 1))
                completed += ClaimBox(row, col - 1, playerId);

            // Box to the right
            if (col < _gridSize && AllEdgesFilled(row, col))
                completed += ClaimBox(row, col, playerId);

            return completed;
        }

        public bool IsFull()
        {
            int total = _gridSize * _gridSize;
            int owned = 0;
            foreach (Box b in _boxes)
                if (b.OwnerId != 0) owned++;
            return owned == total;
        }

        private int ClaimBox(int r, int c, int playerId)
        {
            Box box = _boxes[r, c];
            if (box.TryClaim(playerId))
                return 1;
            return 0;
        }

        private bool AllEdgesFilled(int r, int c)
        {
            return _horizontalLines[r, c] && _horizontalLines[r + 1, c] &&
                   _verticalLines[r, c] && _verticalLines[r, c + 1];
        }
    }
}
