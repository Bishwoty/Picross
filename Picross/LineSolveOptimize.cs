using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picross {
	class LineSolveOptimize : LineSolve2 {
		public override bool Solve(PicrossGrid picross, bool showProgress) {
			checkNext = new bool[2 * picross.Size];
			for(int i = 0; i < checkNext.Length; i++) {
				// Check every cell on first pass
				checkNext[i] = true;
			}

			bool stuck;
			do {
				// If a cell's row and column did not recieve any updates in the last pass,
				// it will not be checked this time.
				checkThese = checkNext;
				checkNext = new bool[2 * picross.Size];
				ResetCheckTable();
				stuck = true;
				for(int i = 0; i < picross.Size; i++) {
					for(int j = 0; j < picross.Size; j++) {
						if(picross.Empty(i, j) && (checkThese[i] || checkThese[picross.Size + j])) {
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
								checkNext[i] = true;
								checkNext[picross.Size + j] = true;
								if(showProgress) {
									Console.WriteLine(picross);
								}
							}
							// Case: Filled not possible, must be X
							else if(!tryFill1 && tryX1) {
								picross.Put(i, j, PicrossGrid.X);
								stuck = false;
								checkNext[i] = true;
								checkNext[picross.Size + j] = true;
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
									checkNext[i] = true;
									checkNext[picross.Size + j] = true;
									if(showProgress) {
										Console.WriteLine(picross);
									}
								}
								// Case: Filled not possible, must be X
								else if(!tryFill2 && tryX2) {
									picross.Put(i, j, PicrossGrid.X);
									stuck = false;
									checkNext[i] = true;
									checkNext[picross.Size + j] = true;
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

		private void ResetCheckTable() {
			for(int i = 0; i < checkNext.Length; i++) {
				checkNext[i] = false;
			}
		}

		private bool[] checkThese;
		private bool[] checkNext;

	}
}
