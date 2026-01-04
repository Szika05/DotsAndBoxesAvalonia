
namespace Game.Persistence
{
    public interface IBoxesDataAccess
    {
        public Task<(Board, int, int, int)> LoadAsync(String path);
        public Task<(Board, int, int, int)> LoadAsync(Stream stream);
        public Task SaveAsync(string path, Board table, Player player1, Player player2, Player currentPlayer);
        public Task SaveAsync(Stream stream, Board table, Player player1, Player player2, Player currentPlayer);
    }

    public class BoxesFileDataAccess : IBoxesDataAccess
    {

        public async Task<(Board, int, int, int)> LoadAsync(String path)
        {
            return await LoadAsync(File.OpenRead(path));
        }

        public async Task<(Board, int, int, int)> LoadAsync(Stream stream)
        {
            try
            {
                using (StreamReader reader = new StreamReader(stream)) // fájl megnyitása
                {
                    int size = int.Parse(await reader.ReadLineAsync() ?? string.Empty);
                    Board board = new Board((BoardSize)size);
                    string[] playerInfos = (await reader.ReadLineAsync() ?? string.Empty).Split(" ");

                    for (int row = 0; row < board.GridSize; row++)
                    {
                        for (int col = 0; col < board.GridSize; col++)
                        {
                            string line = await reader.ReadLineAsync() ?? string.Empty;
                            string[] tokens = line.Split(" ");

                            Box box = board.GetBoxes()[row, col];
                            if (tokens[0] == "True")
                            {
                                box.SetEdge(Side.Top);
                                board.AddHorizontalLine(row, col);
                            }
                            if (tokens[1] == "True")
                            {
                                box.SetEdge(Side.Left);
                                board.AddVerticalLine(row, col);
                            }
                            if (tokens[2] == "True")
                            {
                                box.SetEdge(Side.Right);
                                board.AddVerticalLine(row, col + 1);
                            }
                            if (tokens[3] == "True")
                            {
                                box.SetEdge(Side.Bottom);
                                board.AddHorizontalLine(row + 1, col);
                            }
                            if (tokens[4] != "0") box.TryClaim(int.Parse(tokens[4]));
                        }
                    }

                    return (board, int.Parse(playerInfos[0]), int.Parse(playerInfos[1]), int.Parse(playerInfos[2]));
                }
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task SaveAsync(string path, Board board, Player player1, Player player2, Player currentPlayer)
        {
            await SaveAsync(File.OpenWrite(path), board, player1, player2, currentPlayer);
        }

        public async Task SaveAsync(Stream stream, Board board, Player Player1, Player player2, Player currentPlayer)
        {
            /// File:
            /// gridSize
            /// p1score p2score
            /// box0.top box0.left box0.right box0.bottom ownerId
            /// box1.top box1.left box1.right box1.bottom ownerId
            /// ... ...
            /// boxn.top boxn.left boxn.right boxn.bottom ownerId
            /// 

            try
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    await writer.WriteLineAsync(board.GridSize.ToString());
                    await writer.WriteLineAsync(Player1.Score + " " + player2.Score + " " + currentPlayer.PlayerId);

                    for (int row = 0; row < board.GridSize; row++)
                    {
                        for (int col = 0; col < board.GridSize; col++)
                        {
                            Box box = board.GetBoxes()[row, col];
                            await writer.WriteAsync(box.Top.ToString() + " ");
                            await writer.WriteAsync(box.Left.ToString() + " ");
                            await writer.WriteAsync(box.Right.ToString() + " ");
                            await writer.WriteAsync(box.Bottom.ToString() + " ");
                            await writer.WriteAsync(box.OwnerId.ToString());
                            await writer.WriteAsync("\n");
                        }
                    }
                }
            }
            catch
            {
                throw new Exception();
            }

        }
    }
}
