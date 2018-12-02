using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TicTacToe.ViewModels
{
    public class Board
    {
        public int Rows { get; set; }
        public int Columns { get; set; }

        private ObservableCollection<BoardCell> _cells;
        public ObservableCollection<BoardCell> Cells
        {
            get
            {
                if (_cells == null)
                {
                    _cells = new ObservableCollection<BoardCell>(
                        Enumerable.Range(0, Rows * Columns).Select(i => new BoardCell())
                    );
                }
                return _cells;
            }
        }

        private bool IsLine(int index0, int index1, int index2, string piece)
        {
            var cells = Cells.ToArray();
            return cells[index0].Sign == piece && cells[index1].Sign == piece && cells[index2].Sign == piece;
        }

        public bool IsAnyLine(int index0, int index1, int index2)
        {
            var cells = Cells.ToArray();
            return IsLine(index0, index1, index2, cells[index0].Sign);
        }

        public bool CheckWin() // Win checker method ================================================
        {
            return IsAnyLine(1, 2, 3) || // Horizontal 
                   IsAnyLine(4, 5, 6) || // Horizontal 
                   IsAnyLine(7, 8, 9) || // Horizontal 
                   IsAnyLine(1, 5, 9) || // Diagonal
                   IsAnyLine(7, 5, 3) || // Diagonal
                   IsAnyLine(1, 4, 7) || // Vertical
                   IsAnyLine(2, 5, 8) || // Vertical
                   IsAnyLine(3, 6, 9);   // Vertical
        }
    }

   
    
}
