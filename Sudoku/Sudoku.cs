using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class Sudoku
    {
        const int WidthOfBoard = 9;

        int[,] emptyCellCoordinates;
        public string BoardAsText { get; set; }
        private char[,] Matrix { get; set; }
        private char[,] PreviousMatrix = new char[9, 9];
        public bool Solved { get; private set; }

        // Sudoku - Konstruktorn som bestämmer storleken på matrisen (9x9) och skriver in brädet i BoardAsText.
        public Sudoku(string initialNumbers)
        {
            Matrix = new char[WidthOfBoard, WidthOfBoard];
            BoardAsText = StringToSudokuBoard(initialNumbers);            
        }

        // StringToSudokuBoard - Används vid initialiseringen av brädet från en lång string med alla förbestämda siffror och nollor på de tomma rutorna.
        private string StringToSudokuBoard (string initialNumbers)
        {
            string formatedMatrix = "+---------+---------+---------+\n";

            for (int row = 0; row < WidthOfBoard; row++)
            {
                for (int col = 0; col < WidthOfBoard; col++)
                {
                    if (col % 3 == 0)
                    {
                        formatedMatrix += "|";
                    }
                    if (initialNumbers[col + (row * 9)]=='0')
                    {
                        Matrix[row, col] = ' ';
                    } else
                    {
                        Matrix[row, col] = initialNumbers[col + (row * 9)];
                    }                    
                    formatedMatrix += String.Format(" {0} ", Matrix[row, col]);
                }
                formatedMatrix += "|\n";
                if ((row + 1) % 3 == 0)
                {
                    formatedMatrix += "+---------+---------+---------+\n";
                }
            }            
            return formatedMatrix;
        }

        // UpdateBoard - Uppdaterar utskriftssträngen med nuvarande bräde.
        private string UpdateBoard()
        {
            string formatedMatrix = "+---------+---------+---------+\n";

            for (int row = 0; row < WidthOfBoard; row++)
            {
                for (int col = 0; col < WidthOfBoard; col++)
                {
                    if (col % 3 == 0)
                    {
                        formatedMatrix += "|";
                    }
                    formatedMatrix += String.Format(" {0} ", Matrix[row, col]);
                }
                formatedMatrix += "|\n";
                if ((row + 1) % 3 == 0)
                {
                    formatedMatrix += "+---------+---------+---------+\n";
                }
            }
            return formatedMatrix;
        }

        // Solve - Den metod som anropas utifrån för att starta igång lösaren.
        public void Solve()
        {
            do
            {
                CopyBoard(PreviousMatrix, Matrix);

                for (int row = 0; row < WidthOfBoard; row++)
                {
                    for (int col = 0; col < WidthOfBoard; col++)
                    {
                        if (CellIsEmpty(row, col))
                        {
                            if (IsExclusive(row, col, out int exclusiveNumber))
                            {
                                FillBoard(exclusiveNumber, row, col);
                                BoardAsText = UpdateBoard();
                            }
                        }
                    }
                }

                if (BoardCheck(PreviousMatrix, Matrix))
                {
                    // Console.WriteLine("Tyvärr, jag klarade inte av att hitta en lösning...\nSå här långt kom jag: ");
                    emptyCellCoordinates = GatherRemainingEmptyCells();
                    break;
                }
            } while (BoardContainsEmptyCell());
            if (!BoardContainsEmptyCell())
            {
                Console.WriteLine("Hurra! Jag hittade en lösning som ser ut så här:");
                Solved = true;
            }
        }

        // CopyBoard - Kollar om en siffra har lagts till i brädet efter att alla tomma rutor har kollats.
        private void CopyBoard(char[,] previousBoard, char[,] currentBoard)
        {
            for (int row = 0; row < WidthOfBoard; row++)
            {
                for (int col = 0; col < WidthOfBoard; col++)
                {
                    previousBoard[row, col] = currentBoard[row, col];
                }
            }
        }

        // CellIsEmpty - Kollar om en viss rad och kolumn innehåller ett tomrum.
        private bool CellIsEmpty(int row, int col)
        {
            if (Matrix[row, col] == ' ')
                return true;
            else
                return false;
        }

        // IsExclusive - Den metod som gör de anropanden för att kolla om en specifik siffra redan finns på en rad, kolumn och enhet.
        private bool IsExclusive(int row, int col, out int exclusiveNumber)
        {
            exclusiveNumber = 0;
            bool exclusive = false;

            for (int number = 1; number <= 9; number++)
            {
                if (IsExclusiveInRow(number, row))
                {
                    if (IsExclusiveInColumn(number, col))
                    {
                        if (IsExclusiveInUnit(number, row, col))
                        {
                            if (exclusive)
                                return false;
                            else
                            {
                                exclusive = true;
                                exclusiveNumber = number;
                            }
                        }
                    }
                }
            }

            return exclusive;
        }

        // IsExclusiveInRow - Självsägande. Eftersome Matrix är en char-matris så måste man addera 48 till int:en "i" för att hamna på rätt charnummer för 1-9.
        private bool IsExclusiveInRow(int number, int row)
        {
            for (int col = 0; col < WidthOfBoard; col++)
            {
                if (Matrix[row, col] == (char)(number + 48))
                    return false;
            }
            return true;
        }

        // IsExclusiveInColumn - Självsägande. Eftersome Matrix är en char-matris så måste man addera 48 till int:en "i" för att hamna på rätt charnummer för 1-9.
        private bool IsExclusiveInColumn(int number, int col)
        {
            for (int row = 0; row < WidthOfBoard; row++)
            {
                if (Matrix[row, col] == (char)(number + 48))
                    return false;
            }
            return true;
        }

        // IsExclusiveInUnit - Självsägande. Eftersome Matrix är en char-matris så måste man addera 48 till int:en "i" för att hamna på rätt charnummer för 1-9.
        private bool IsExclusiveInUnit(int number, int row, int col)
        {
            CheckUnitStart(row, col, out int firstRowPositionInUnit, out int firstColumnPositionInUnit);

            for (int i = firstRowPositionInUnit; i < (firstRowPositionInUnit + 3); i++)
            {
                for (int j = firstColumnPositionInUnit; j < (firstColumnPositionInUnit + 3); j++)
                {
                    if (Matrix[i, j] == (char)(number + 48))
                        return false;
                }
            }

            return true;
        }

        // CheckUnitStart - Kollar i vilken av de nio enheterna man står i och returnernar dessa enheters övre-vänstra position.
        private void CheckUnitStart(int row, int col, out int firstRowPositionInUnit, out int firstColumnPositionInUnit)
        {
            if (row < 3 && col < 3)
            {
                firstColumnPositionInUnit = 0;
                firstRowPositionInUnit = 0;
            }
            else if ((row < 3) && (col > 2 && col < 6))
            {
                firstColumnPositionInUnit = 3;
                firstRowPositionInUnit = 0;
            }
            else if ((row < 3) && (col > 5))
            {
                firstColumnPositionInUnit = 6;
                firstRowPositionInUnit = 0;
            }
            else if ((row > 2 && row < 6) && (col < 3))
            {
                firstColumnPositionInUnit = 0;
                firstRowPositionInUnit = 3;
            }
            else if ((row > 5) && (col < 3))
            {
                firstColumnPositionInUnit = 0;
                firstRowPositionInUnit = 6;
            }
            else if ((row > 2 && row < 6) && (col > 2 && col < 6))
            {
                firstColumnPositionInUnit = 3;
                firstRowPositionInUnit = 3;
            }
            else if ((row > 5) && (col > 2 && col < 6))
            {
                firstColumnPositionInUnit = 3;
                firstRowPositionInUnit = 6;
            }
            else if ((row > 2 && row < 6) && (col > 5))
            {
                firstColumnPositionInUnit = 6;
                firstRowPositionInUnit = 3;
            }
            else
            {
                firstColumnPositionInUnit = 6;
                firstRowPositionInUnit = 6;
            }
        }
                
        private void FillBoard(int number, int row, int col)
        {
            Matrix[row, col] = (char)(number + 48);
        }

        // BoardCheck - Kollar om brädet har uppdaterats efter man har kollat igenom alla tomma celler en gång.
        private bool BoardCheck(char[,] previousBoard, char[,] currentBoard)
        {
            for (int row = 0; row < WidthOfBoard; row++)
            {
                for (int col = 0; col < WidthOfBoard; col++)
                {
                    if (previousBoard[row, col] != currentBoard[row, col])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // BoardContainsEmptyCell - Kollar om det fortfarande finns tomma celler kvar i brädet. 
        private bool BoardContainsEmptyCell()
        {
            for (int i = 0; i < WidthOfBoard; i++)
            {
                for (int j = 0; j < WidthOfBoard; j++)
                {
                    if (Matrix[i, j] == ' ')
                        return true;
                }
            }
            return false;
        }

        // GatherRemainingEmptyCells - Spits out a int[x,2] array with the coordinates of all empty cells on the board (row coordinate in first column, column coordinate in second column)
        private int[,] GatherRemainingEmptyCells()
        {
            int[,] emptyCellCoordinates = new int[NumberOfEmptyCells(), 2];
            int matrixPosition = 0;
            for (int row = 0; row < WidthOfBoard; row++)
            {
                for (int col = 0; col < WidthOfBoard; col++)
                {
                    if (CellIsEmpty(row, col))
                    {
                        emptyCellCoordinates[matrixPosition, 0] = row;
                        emptyCellCoordinates[matrixPosition, 1] = col;
                        matrixPosition++;
                    }
                }
            }
            return emptyCellCoordinates;
        }

        // NumberOfEmptyCells - Kollar hur många tomma rutor som finns på brädet.
        private int NumberOfEmptyCells()
        {
            int numberOfEmptyCells = 0;

            for (int row = 0; row < WidthOfBoard; row++)
            {
                for (int col = 0; col < WidthOfBoard; col++)
                {
                    if (CellIsEmpty(row, col))
                    {
                        numberOfEmptyCells++;
                    }
                }
            }
            return numberOfEmptyCells;
        }

        public void RecursiveSolveStarter()
        {
            for (int i = 9; i >= 1; i--)
            {
                SolveWithRecursion(0, emptyCellCoordinates[0, 0], emptyCellCoordinates[0, 1], i);
            }
        }

        private void SolveWithRecursion(int count, int row, int col, int number)
        {
            /*
            if (number > 9)
            {
                return;
            }
            */
            if (!IsExclusiveInRow(number, row))
            {
                return;
            }
            if (!IsExclusiveInColumn(number, col))
            {
                return;
            }
            if (!IsExclusiveInUnit(number, row, col))
            {
                return;
            }

            FillBoard(number, row, col);
            //BoardAsText = UpdateBoard();
            //Console.SetCursorPosition(0, 0);
            //Console.WriteLine(this.BoardAsText);
            //System.Threading.Thread.Sleep(1000);

            if (!BoardContainsEmptyCell())
            {
                BoardAsText = UpdateBoard();
                Console.WriteLine("Jag hittade en lösning som ser ut såhär:\n");
                Console.WriteLine(this.BoardAsText);
                Solved = true;

                return;
            }

            for (int i = 9; i >= 1; i--)
            {
                SolveWithRecursion((count + 1), emptyCellCoordinates[(count + 1), 0], emptyCellCoordinates[(count + 1), 1], i);
            }

            Matrix[row, col] = ' ';
            //BoardAsText = UpdateBoard();
            //Console.Clear();
            //Console.WriteLine(this.BoardAsText);
            //System.Threading.Thread.Sleep(200);

        }
    }
}
