using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace Quixo
{
    public class Board
    {
        private const int BOARD_SIZE = 4;

        public const int BLANK = 0;
        public const int X = 1;
        public const int O = 2;


        const int SLIDE_LEFT = 0;
        const int SLIDE_UP = 1;
        const int SLIDE_RIGHT = 2;
        const int SLIDE_DOWN = 3;


        public Matrix<Single> board;
        public int Turn;

        public Board(Board _board = null, int startTurn = Board.X)
        {
            if (_board == null)
            {
                this.board = Matrix<Single>.Build.Dense(BOARD_SIZE + 1, BOARD_SIZE + 1);
                this.Turn = startTurn;
            }
            else
            {
                this.board = Matrix<Single>.Build.DenseOfMatrix(_board.board);
                this.Turn = _board.Turn;
            }
        }

        public bool Play(Piece piece, int move)
        {
            bool legalMove = checkMove(piece, move);
            if (!legalMove)
                throw new Exception("Invalid move");
            //return false;

            this.board.At(piece.x, piece.y, this.Turn);
            this.shift(piece, move);
            this.changeTurn();
            return true;
        }

        public Player isGameEnd()
        {
            var firstDiagonal = this.board.Diagonal();
            var secondDiagonal = Vector<Single>.Build.Dense(5);
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                int col = BOARD_SIZE - i;

                secondDiagonal[i] = (int)this.board.At(i, col);
            }

            Func<float, bool> XPredicate = (e) =>
            {
                return e == Board.X;
            };

            Func<float, bool> OPredicate = (e) =>
            {
                return e == Board.O;
            };

            if (firstDiagonal.ForAll(XPredicate) || firstDiagonal.ForAll(OPredicate))
            {
                return (Player)firstDiagonal.At(0);
            }
            if (secondDiagonal.ForAll(XPredicate) || secondDiagonal.ForAll(OPredicate))
            {
                return (Player)secondDiagonal.At(0);
            }

            foreach (var row in this.board.EnumerateRows())
            {
                if (row.ForAll(XPredicate) || row.ForAll(OPredicate))
                    return (Player)row.At(0);
            }

            foreach (var col in this.board.EnumerateColumns())
            {
                if (col.ForAll(XPredicate) || col.ForAll(OPredicate))
                    return (Player)col.At(0);
            }


            return Player.None;
        }

        public void PrintBoard()
        {
            Console.WriteLine(this.board.ToString());
        }

        public void PrintTurn()
        {
            if (this.Turn == X)
                Console.WriteLine("Turn: X");
            else
                Console.WriteLine("Turn: O");
        }

        private bool checkMove(Piece piece, int move)
        {
            int cellValue = (int)this.board[piece.x, piece.y];
            if (!(cellValue == BLANK || cellValue == this.Turn))
                return false;

            List<int> possibleMoves = getPossiblePieceMoves(piece);
            if (possibleMoves.Contains(move))
                return true;
            else
                return false;
        }

        public List<Move> getPossibleMoves()
        {
            int opponent;
            if (this.Turn == X)
                opponent = Board.O;
            else
                opponent = Board.X;

            var innerMatrix = Matrix<Single>.Build.Dense(3, 3, opponent);
            var searchMatrix = Matrix<Single>.Build.DenseOfMatrix(this.board);
            searchMatrix.SetSubMatrix(1, 1, innerMatrix);

            List<Move> allMoves = new List<Move>();

            foreach (var cell in searchMatrix.EnumerateIndexed(Zeros.Include).Where(k => k.Item3 != opponent))
            {
                Piece piece;
                piece.x = cell.Item1;
                piece.y = cell.Item2;

                float _val;
                cell.Deconstruct(out piece.x, out piece.y, out _val);
                uint val = (uint)_val;

                var moves = getPossiblePieceMoves(piece);
                foreach (var move in moves)
                {
                    var pieceMove = new Move(piece,move);
                    allMoves.Add(pieceMove);
                }
            }

            allMoves.Shuffle();

            return allMoves;

        }

        private List<int> getPossiblePieceMoves(Piece piece)
        {
            int row = piece.x;
            int col = piece.y;

            List<int> possibleMoves = new List<int>(4);

            if (row < BOARD_SIZE)
                possibleMoves.Add(SLIDE_UP);
            if (row > 0)
                possibleMoves.Add(SLIDE_DOWN);
            if (col < BOARD_SIZE)
                possibleMoves.Add(SLIDE_LEFT);
            if (col > 0)
                possibleMoves.Add(SLIDE_RIGHT);

            return possibleMoves;
        }


        private void changeTurn()
        {
            if (this.Turn == X)
                this.Turn = O;
            else
                this.Turn = X;
        }

        private void shift(Piece piece, int move)
        {
            int row = piece.x;
            int col = piece.y;

            var shiftRow = this.board.Row(row);
            var shiftCol = this.board.Column(col);

            if (move == SLIDE_RIGHT)
            {
                //1,2,3,4,5
                //2 slided into
                //2,1,3,4,5
                for (int i = col; i > 0; i--)
                {
                    int temp = (int)shiftRow[i];
                    shiftRow.At(i, shiftRow.At(i - 1));
                    shiftRow.At(i - 1, temp);
                }

                this.board.SetRow(row, shiftRow);
            }
            else if (move == SLIDE_LEFT)
            {
                //1,2,3,4,5
                //2 slided into
                //1,3,4,5,2
                for (int i = col; i < BOARD_SIZE; i++)
                {
                    int temp = (int)shiftRow[i];
                    shiftRow.At(i, shiftRow.At(i + 1));
                    shiftRow.At(i + 1, temp);
                }

                this.board.SetRow(row, shiftRow);
            }
            else if (move == SLIDE_UP)
            {
                //1,2,3,4,5
                //2 slided into
                //2,1,3,4,5
                for (int i = row; i > row; i--)
                {
                    int temp = (int)shiftCol[i];
                    shiftCol.At(i, shiftCol.At(i - 1));
                    shiftCol.At(i - 1, temp);
                }

                this.board.SetColumn(col, shiftCol);
            }
            else if (move == SLIDE_DOWN)
            {
                //1,2,3,4,5
                //2 slided into
                //2,1,3,4,5
                for (int i = row; i < BOARD_SIZE; i++)
                {
                    int temp = (int)shiftCol[i];
                    shiftCol.At(i, shiftCol.At(i + 1));
                    shiftCol.At(i + 1, temp);
                }

                this.board.SetColumn(col, shiftCol);
            }
        }

        public struct Piece
        {
            public int x;
            public int y;

            public Piece(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public struct Move
        {
            public Piece piece;
            public int move;

            public Move(Piece piece, int move)
            {
                this.piece = piece;
                this.move = move;
            }
        }
    }
}

public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle(this IList ts)
    {
        var count = ts.Count;
        var last = count - 1;
        var rnd = new Random();
        for (var i = 0; i < last; ++i)
        {
            var r = rnd.Next(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
