namespace Game.Persistence
{

    public class Box
    {
        public int OwnerId { get; set; } = 0; // 0 = unfinished, 1 = player1, 2 = player2
        public bool Top { get; private set; } = false;
        public bool Left { get; private set; } = false;
        public bool Right { get; private set; } = false;
        public bool Bottom { get; private set; } = false;

        public bool IsComplete => Top && Right && Left && Bottom;

        public void SetEdge(Side side)
        {
            switch (side)
            {
                case Side.Left:
                    Left = true;
                    break;
                case Side.Right:
                    Right = true;
                    break;
                case Side.Top:
                    Top = true;
                    break;
                case Side.Bottom:
                    Bottom = true;
                    break;

                default:
                    break;
            }
        }

        public bool TryClaim(int playerId)
        {
            if (IsComplete && OwnerId == 0)
            {
                OwnerId = playerId;
                return true;
            }
            return false;
        }
    }
}
