using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quixo
{
    class Play
    {
        public static void random_random_play(int iteration = 1)
        {
            int xWin = 0;
            int oWin = 0;

            Random rnd = new Random();
            for (int i = 0; i < iteration; i++)
            {
                Board b = new Board();
                //Console.WriteLine($"Playing iteration: {i}");
                while (true)
                {
                    var possibleMoves = b.getPossibleMoves();
                    var choice = rnd.Next(0, possibleMoves.Count);
                    var move = possibleMoves[choice];

                    b.Play(move.piece, move.move);
                    //b.PrintBoard();
                    Player gameEnd = b.isGameEnd();
                    if (gameEnd == Player.None)
                        continue;

                    if (gameEnd == Player.X)
                        xWin++;
                    else
                        oWin++;
                    break;
                }
            }

            Console.WriteLine($"X Wins:{xWin}, O Wins:{oWin}");
        }

        public static void ai_random_play(int iteration = 1, bool parallel = false)
        {
            int xWin = 0;
            int oWin = 0;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();


            if (parallel)
            {
                Parallel.For(0, iteration, (i) =>
                {
                    Random rnd = new Random();
                    Board b = new Board();
                    while (true)
                    {
                        var move = Ai.GetBestMove(b);
                        b.Play(move.piece, move.move);

                        Player gameEnd = b.isGameEnd();
                        if (gameEnd != Player.None)
                        {
                            if (gameEnd == Player.X)
                                Interlocked.Increment(ref xWin);
                            else
                                Interlocked.Increment(ref oWin);
                            break;
                        }

                        var possibleMoves = b.getPossibleMoves();
                        var choice = rnd.Next(0, possibleMoves.Count);
                        move = possibleMoves[choice];

                        b.Play(move.piece, move.move);
                        //b.PrintBoard();
                        gameEnd = b.isGameEnd();
                        if (gameEnd != Player.None)
                        {
                            if (gameEnd == Player.X)
                                Interlocked.Increment(ref xWin);
                            else
                                Interlocked.Increment(ref oWin);
                            break;
                        }
                    }

                    //Console.WriteLine($"{xWin + oWin} Games Played");
                });
            }
            else
            {
                Random rnd = new Random();

                for (int i = 0; i < iteration; i++)
                {

                    Board b = new Board();
                    //Console.WriteLine($"Playing iteration: {i}");
                    while (true)
                    {
                        var move = Ai.GetBestMove(b);
                        b.Play(move.piece, move.move);

                        Player gameEnd = b.isGameEnd();
                        if (gameEnd != Player.None)
                        {
                            if (gameEnd == Player.X)
                                xWin++;
                            //Interlocked.Increment(ref xWin);
                            else
                                oWin++;
                            //Interlocked.Increment(ref oWin);
                            break;
                        }

                        var possibleMoves = b.getPossibleMoves();
                        var choice = rnd.Next(0, possibleMoves.Count);
                        move = possibleMoves[choice];

                        b.Play(move.piece, move.move);
                        //b.PrintBoard();
                        gameEnd = b.isGameEnd();
                        if (gameEnd != Player.None)
                        {
                            if (gameEnd == Player.X)
                                xWin++;
                            //Interlocked.Increment(ref xWin);
                            else
                                oWin++;
                            //Interlocked.Increment(ref oWin);
                            break;
                        }
                    }

                    //Console.WriteLine($"{xWin + oWin} Games Played");
                }
            }
            sw.Stop();
            Console.WriteLine("Elapsed: " + sw.Elapsed.TotalSeconds);

            Console.WriteLine($"X Wins:{xWin}, O Wins:{oWin}");
        }

        public static void parallel_ai_ai_play(int iteration = 1)
        {
            int xWin = 0;
            int oWin = 0;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Parallel.For(0, iteration, (i) =>
            {
                Random rnd = new Random();
                Board b = new Board();
                while (true)
                {
                    var move = Ai.GetBestMove(b);
                    b.Play(move.piece, move.move);

                    //b.PrintBoard();

                    Player gameEnd = b.isGameEnd();
                    if (gameEnd != Player.None)
                    {
                        if (gameEnd == Player.X)
                            Interlocked.Increment(ref xWin);
                        else
                            Interlocked.Increment(ref oWin);
                        break;
                    }
                }
            });
            sw.Stop();
            Console.WriteLine("Elapsed: " + sw.Elapsed.TotalSeconds);

            Console.WriteLine($"X Wins:{xWin}, O Wins:{oWin}");
        }

        public static void ai_ai_play(int iteration = 1)
        {

            int xWin = 0;
            int oWin = 0;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();


            Random rnd = new Random();
            for (int i = 0; i < iteration; i++)
            {
                Board b = new Board();
                while (true)
                {
                    var move = Ai.GetBestMove(b);
                    b.Play(move.piece, move.move);

                    //b.PrintBoard();

                    Player gameEnd = b.isGameEnd();
                    if (gameEnd != Player.None)
                    {
                        if (gameEnd == Player.X)
                            Interlocked.Increment(ref xWin);
                        else
                            Interlocked.Increment(ref oWin);
                        break;
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("Elapsed: " + sw.Elapsed.TotalSeconds);

            Console.WriteLine($"X Wins:{xWin}, O Wins:{oWin}");
        }
    }
}
