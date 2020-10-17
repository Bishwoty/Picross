using System;

namespace Picross {
	class BruteForce : IPicrossSolver {
		public bool Solve(PicrossGrid picross, bool showProgress) {
			return RecursiveStep(picross, 0, 0, showProgress);
		}

		bool RecursiveStep(PicrossGrid picross, int row, int col, bool showProgress) {
			if(row >= picross.Size) {
				return true;
			}

			int nextRow = (col + 1 == picross.Size) ? row + 1 : row;
			int nextCol = (col + 1) % picross.Size;
			if(picross.Empty(row, col)) {
				if(showProgress) {
					Console.WriteLine(picross);
				}

				picross.Put(row, col, PicrossGrid.FILLED);
				if(PicrossUtilities.Verify(picross, row, col) && RecursiveStep(picross, nextRow, nextCol, showProgress)) {
					return true;
				}

				picross.Clear(row, col);
				picross.Put(row, col, PicrossGrid.X);
				if(PicrossUtilities.Verify(picross, row, col) && RecursiveStep(picross, nextRow, nextCol, showProgress)) {
					return true;
				}
				picross.Clear(row, col);
				return false;
			} else {
				return PicrossUtilities.Verify(picross, row, col) && RecursiveStep(picross, nextRow, nextCol, showProgress);
			}
		}
	}
}
