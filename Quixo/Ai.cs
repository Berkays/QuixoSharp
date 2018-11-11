using System.Linq;
using static Quixo.Board;

namespace Quixo
{
    public class Ai
    {
        private struct r
        {
            public int score;
            public Move move;
            public r(int score, Move m)
            {
                this.score = score;
                this.move = m;
            }

        }

        private static r alphabeta(Board board, int player, int alpha, int beta, int depth)
        {
            var score = evaluate(board, depth);
            if (score != 999)
                return new r(score, default(Move));

            depth--;

            Move bestMove = new Move();

            if (player == (int)Player.X)
            {
                foreach (var move in board.getPossibleMoves())
                {
                    var s = new Board(board);
                    s.Play(move.piece, move.move);
                    var val = alphabeta(s, s.Turn, alpha, beta, depth).score;
                    if (val > alpha)
                    {
                        alpha = val;
                        bestMove = move;
                    }
                    if (alpha >= beta)
                        break;
                }
                return new r(alpha, bestMove);
            }
            else
            {
                foreach (var move in board.getPossibleMoves())
                {
                    var s = new Board(board);
                    s.Play(move.piece, move.move);
                    var val = alphabeta(s, s.Turn, alpha, beta, depth).score;
                    if (val < beta)
                    {
                        beta = val;
                        bestMove = move;
                    }
                    if (alpha >= beta)
                        break;
                }
                return new r(beta, bestMove);
            }
        }

        private static int evaluate(Board board, int depth)
        {
            var isTerminal = board.isGameEnd();
            if (isTerminal == Player.X)
                return 100 + depth;
            else if (isTerminal == Player.O)
                return -100 - depth;
            else if (depth == 0)
            {
                var xCount = board.board.Enumerate().Count(n => n == 1);
                var yCount = board.board.Enumerate().Count(n => n == 2);
                return (xCount - yCount);
            }
            else
                return 999;
        }

        public static Move GetBestMove(Board board)
        {
            var possibleMoves = board.getPossibleMoves();

            var possibleMoveCount = possibleMoves.Count;

            var depth = 0;
            if (possibleMoveCount > 0)
            {
                depth = 5;
            }
            if (possibleMoveCount > 17)
            {
                depth = 4;
            }
            if (possibleMoveCount > 22)
            {
                depth = 3;
            }
            if (possibleMoveCount > 30)
            {
                depth = 2;
            }
            if (possibleMoveCount > 35)
                depth = 1;
            //if (possibleMoveCount > 30)
            //{
            //    depth = 3;
            //}
            //if(possibleMoveCount > 40)
            //{
            //    depth = 2;
            //}


            var bestMove = alphabeta(board, board.Turn, int.MinValue, int.MaxValue, depth);

            return bestMove.move;
        }
    }
}
