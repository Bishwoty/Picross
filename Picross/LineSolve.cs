using System;

namespace Picross {
	class LineSolve : IPicrossSolver {
		public bool Solve(PicrossGrid picross, bool showProgress) {
			bool stuck;
			do {
				stuck = true;
				for(int i = 0; i < picross.Size; i++) {
					for(int j = 0; j < picross.Size; j++) {
						if(picross.Empty(i, j)) {
							if(showProgress) {
								Console.WriteLine($"{i}, {j}");
							}
							bool tryFill1 = Check(picross, PicrossGrid.FILLED, i, j, true);
							bool tryX1 = Check(picross, PicrossGrid.X, i, j, true);
							bool tryFill2;
							bool tryX2;

							// Case: Neither option is possible, invalid puzzle
							if(!tryFill1 && !tryX1) {
								return false;
							}
							// Case: X not possible, must be filled
							else if(tryFill1 && !tryX1) {
								picross.Put(i, j, PicrossGrid.FILLED);
								stuck = false;
								if(showProgress) {
									Console.WriteLine(picross);
								}
							}
							// Case: Filled not possible, must be X
							else if(!tryFill1 && tryX1) {
								picross.Put(i, j, PicrossGrid.X);
								stuck = false;
								if(showProgress) {
									Console.WriteLine(picross);
								}
							} else {
								// Check other dimension
								tryFill2 = Check(picross, PicrossGrid.FILLED, i, j, false);
								tryX2 = Check(picross, PicrossGrid.X, i, j, false);

								// Case: Neither option is possible, invalid puzzle
								if(!tryFill1 && !tryX1) {
									return false;
								}
								// Case: X not possible, must be filled
								else if(tryFill2 && !tryX2) {
									picross.Put(i, j, PicrossGrid.FILLED);
									stuck = false;
									if(showProgress) {
										Console.WriteLine(picross);
									}
								}
								// Case: Filled not possible, must be X
								else if(!tryFill2 && tryX2) {
									picross.Put(i, j, PicrossGrid.X);
									stuck = false;
									if(showProgress) {
										Console.WriteLine(picross);
									}
								}
								// Else Case: Both options are possible, cannot deduce
							}
						}
					}
				}
			} while(!stuck && !picross.IsValid());
			return !stuck;
		}

		protected virtual bool Check(PicrossGrid picross, int val, int row, int col, bool checkRow) {
			if(picross.At(row, col) == PicrossGrid.EMPTY) {
				picross.Put(row, col, val);

				//Console.WriteLine(picross);


				// Checks to reduce recursion
				int targetFill = PicrossUtilities.InstructionSum(picross, checkRow ? row : picross.Size + col);
				int currentFill = PicrossUtilities.LineSum(picross, checkRow ? row : picross.Size + col, PicrossGrid.FILLED);
				int currentX = PicrossUtilities.LineSum(picross, checkRow ? row : picross.Size + col, PicrossGrid.X);
				if(currentFill > targetFill || currentX > picross.Size - targetFill) {
					picross.Clear(row, col);
					return false;
				}

				int nextRow;
				int nextCol;
				bool result;
				for(int i = 0; i < picross.Size; i++) {
					if(picross.At(checkRow ? row : i, checkRow ? i : col) == PicrossGrid.EMPTY) {

						nextRow = checkRow ? row : i;
						nextCol = checkRow ? i : col;

						// Recurse
						if(targetFill - currentFill > picross.Size - targetFill - currentX) {
							result = Check(picross, PicrossGrid.FILLED, nextRow, nextCol, checkRow) || Check(picross, PicrossGrid.X, nextRow, nextCol, checkRow);
						} else {
							result = Check(picross, PicrossGrid.X, nextRow, nextCol, checkRow) || Check(picross, PicrossGrid.FILLED, nextRow, nextCol, checkRow);
						}

						picross.Clear(row, col);
						return result;
					}
				}

				// Filled row/column, check if valid
				result = picross.IsValid(checkRow, checkRow ? row : col);

				picross.Clear(row, col);
				return result;

			} else {
				throw new System.ArgumentException("Check on non empty cell");
			}
		}
	}
}
