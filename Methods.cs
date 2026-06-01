using System;
using System.Collections.Generic;

namespace ConsoleApp23
{
    class Program
    {
        // Engedélyezett karakterek listája
        static readonly char[] validChars = new char[]
        {
            '.', '█', '╬', '═', '╦', '╩', '║', '╣', '╠', '╗', '╝', '╚', '╔'
        };

        // Melyik karakter merre "nyit" (észak, dél, nyugat, kelet)
        // true = nyitott abba az irányba
        static bool OpensNorth(char c)
        {
            return c == '║' || c == '╬' || c == '╣' || c == '╠' || c == '╩' || c == '╝' || c == '╚';
        }
        static bool OpensSouth(char c)
        {
            return c == '║' || c == '╬' || c == '╣' || c == '╠' || c == '╦' || c == '╗' || c == '╔';
        }
        static bool OpensWest(char c)
        {
            return c == '═' || c == '╬' || c == '╩' || c == '╦' || c == '╣' || c == '╝' || c == '╗';
        }
        static bool OpensEast(char c)
        {
            return c == '═' || c == '╬' || c == '╩' || c == '╦' || c == '╠' || c == '╚' || c == '╔';
        }

        // Segédfüggvény: egy karakter járat-e?
        static bool IsPassage(char c)
        {
            return c != '.' && c != '█';
        }

        /// <summary>
        /// Megadja, hogy hány termet tartamaz a térkép
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>Termek száma</returns>
        static int GetRoomNumber(char[,] map)
        {
            int count = 0;
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == '█')
                        count++;
                }
            }
            return count;
        }

        /// <summary>
        /// A kapott térkép széleit végignézve megállapítja, hogy hány kijárat van.
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>Az alkalmas kijáratok száma</returns>
        static int GetSuitableEntrance(char[,] map)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            int count = 0;

            // Első és utolsó sor
            for (int j = 0; j < cols; j++)
            {
                if (IsPassage(map[0, j])) count++;
                if (IsPassage(map[rows - 1, j])) count++;
            }

            // Első és utolsó oszlop (sarkokat ne számoljuk kétszer)
            for (int i = 1; i < rows - 1; i++)
            {
                if (IsPassage(map[i, 0])) count++;
                if (IsPassage(map[i, cols - 1])) count++;
            }

            return count;
        }

        /// <summary>
        /// Megnézi, hogy van-e a térképen meg nem engedett karakter?
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>true - A térkép tartalmaz szabálytalan karaktert, false - nincs benne ilyen</returns>
        static bool IsInvalidElement(char[,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    bool found = false;
                    foreach (char valid in validChars)
                    {
                        if (map[i, j] == valid)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Visszaadja azoknak a járatkaraktereknek a pozícióját, amelyekhez egyetlen szomszéd pozícióból sem lehet eljutni.
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>A pozíciók "sor_index:oszlop_index" formátumban szerepelnek a lista elemeiként</returns>
        static List<string> GetUnavailableElements(char[,] map)
        {
            List<string> unavailables = new List<string>();
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Csak járatkaraktereket vizsgálunk
                    if (!IsPassage(map[i, j])) continue;

                    char c = map[i, j];
                    bool reachable = false;

                    // Észak: az i-1 sor, j oszlop karaktere dél felé nyit-e?
                    if (i > 0 && IsPassage(map[i - 1, j]) && OpensSouth(map[i - 1, j]) && OpensNorth(c))
                        reachable = true;

                    // Dél: az i+1 sor, j oszlop karaktere észak felé nyit-e?
                    if (i < rows - 1 && IsPassage(map[i + 1, j]) && OpensNorth(map[i + 1, j]) && OpensSouth(c))
                        reachable = true;

                    // Nyugat: az i sor, j-1 oszlop karaktere kelet felé nyit-e?
                    if (j > 0 && IsPassage(map[i, j - 1]) && OpensEast(map[i, j - 1]) && OpensWest(c))
                        reachable = true;

                    // Kelet: az i sor, j+1 oszlop karaktere nyugat felé nyit-e?
                    if (j < cols - 1 && IsPassage(map[i, j + 1]) && OpensWest(map[i, j + 1]) && OpensEast(c))
                        reachable = true;

                    // Ha a térkép szélén van, a kijáraton keresztül is "elérhető"
                    if (i == 0 || i == rows - 1 || j == 0 || j == cols - 1)
                        reachable = true;

                    if (!reachable)
                    {
                        unavailables.Add(i + ":" + j);
                    }
                }
            }

            return unavailables;
        }

        /// <summary>
        /// Labiritust generál a kapott pozíciókat tartalmazó lista alapján. A lista elemei egymáshoz kapcsolódó járatok pozíciói.
        /// </summary>
        /// <param name="positionsList">"sor_index:oszlop_index" formátumban az egymáshoz kapcsolódó járatok pozícióit tartalmazó lista </param>
        /// <returns>A létrehozott labirintus térképe</returns>
        static char[,] GenerateLabyrinth(List<string> positionsList)
        {
            if (positionsList == null || positionsList.Count == 0)
                return null;

            // Megkeressük a térkép méretét (min/max sor és oszlop)
            int minRow = int.MaxValue, maxRow = int.MinValue;
            int minCol = int.MaxValue, maxCol = int.MinValue;

            foreach (string pos in positionsList)
            {
                string[] parts = pos.Split(':');
                int row = int.Parse(parts[0]);
                int col = int.Parse(parts[1]);

                if (row < minRow) minRow = row;
                if (row > maxRow) maxRow = row;
                if (col < minCol) minCol = col;
                if (col > maxCol) maxCol = col;
            }

            int rows = maxRow - minRow + 1;
            int cols = maxCol - minCol + 1;

            // Létrehozzuk a térképet, alapból '.' karakterekkel töltjük fel
            char[,] map = new char[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    map[i, j] = '.';

            // A listában szereplő pozíciókra '═' karaktert rakunk
            foreach (string pos in positionsList)
            {
                string[] parts = pos.Split(':');
                int row = int.Parse(parts[0]) - minRow;
                int col = int.Parse(parts[1]) - minCol;
                map[row, col] = '═';
            }

            return map;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
