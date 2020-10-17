using System;


namespace Picross {
	class LineSolve2 : LineSolve {
		protected override bool Check(PicrossGrid picross, int val, int row, int col, bool checkRow) {
			if(picross.At(row, col) == PicrossGrid.EMPTY) {
				picross.Put(row, col, val);

				//Console.WriteLine(picross);
				int nextRow;
				int nextCol;
				bool result;

				nextRow = checkRow ? row : 0;
				nextCol = checkRow ? 0 : col;

				result = Recurse(picross, nextRow, nextCol, checkRow);


				picross.Clear(row, col);
				return result;

			} else {
				throw new System.ArgumentException("Check on non empty cell");
			}
		}

		bool Recurse(PicrossGrid picross, int row, int col, bool checkRow) {

			if(row >= picross.Size || col >= picross.Size) {
				return true;
			}

			int nextRow = checkRow ? row : row + 1;
			int nextCol = checkRow ? col + 1 : col;

			if(picross.Empty(row, col)) {
				//Console.WriteLine(picross);
				picross.Put(row, col, PicrossGrid.FILLED);
				if((checkRow ? PicrossUtilities.VerifyRow(picross, row, col) :
					PicrossUtilities.VerifyCol(picross, col, row)) && 
					Recurse(picross, nextRow, nextCol, checkRow)) {
					picross.Clear(row, col);
					return true;
				}
				picross.Clear(row, col);
				picross.Put(row, col, PicrossGrid.X);
				if((checkRow ? PicrossUtilities.VerifyRow(picross, row, col) : 
					PicrossUtilities.VerifyCol(picross, col, row)) && 
					Recurse(picross, nextRow, nextCol, checkRow)) {
					picross.Clear(row, col);
					return true;
				}
				picross.Clear(row, col);
				return false;
			} else {
				return (checkRow ? PicrossUtilities.VerifyRow(picross, row, col) : 
					PicrossUtilities.VerifyCol(picross, col, row)) && 
					Recurse(picross, nextRow, nextCol, checkRow);
			}
		}
	}
}
