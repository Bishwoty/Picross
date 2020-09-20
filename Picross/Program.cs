using System;
using System.IO;
using System.Diagnostics;

namespace Picross {
	class Program {
		static void Main(string[] args) {
			while(true) {
				Console.WriteLine("Solve Version?");
				int solveVersion = int.Parse(Console.ReadLine());
				PicrossGrid game = new PicrossGrid(0);
				Console.WriteLine("Show Progress? (y/n)");
				bool showProgress = Char.ToUpper((char)Console.Read()) == 'Y';
				Console.ReadLine();
				ReadPuzzle(ref game);
				bool result = false;
				Stopwatch timer = new Stopwatch();
				switch(solveVersion) {
					case 1:
						timer.Start();
						result = Solve1(game, Check, showProgress);
						timer.Stop();
						break;
					case 2:
						timer.Start();
						result = Solve2(game, 0, 0, showProgress);
						timer.Stop();
						break;
					case 3:
						timer.Start();
						result = Solve1(game, Check2, showProgress);
						timer.Stop();
						break;
				}
				if(result) {
					Console.WriteLine(game);
					Console.WriteLine($"Puzzle solved in {timer.ElapsedMilliseconds} ms");
				} else {
					Console.WriteLine("Couldn't Solve");
				}
			}
		}


		static void ReadPuzzle(ref PicrossGrid picross) {
			Console.WriteLine("Puzzle Name:");
			string puzzleName = Console.ReadLine();
			string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{puzzleName}";
			while(!File.Exists(path)) {
				Console.WriteLine("Puzzle not Found.");
				Console.WriteLine("Puzzle Name:");
				puzzleName = Console.ReadLine();
				path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{puzzleName}";
			}

			using(StreamReader sr = File.OpenText(path)) {
				int puzzleSize;
				puzzleSize = int.Parse(sr.ReadLine());
				picross = new PicrossGrid(puzzleSize);
				for(int i = 0; i < puzzleSize * 2; i++) {
					string[] s = sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					int[] arr = new int[s.Length];
					for(int j = 0; j < s.Length; j++) {
						arr[j] = int.Parse(s[j]);
					}
					picross.SetInstruction(i, arr);
				}

				for(int i = 0; i < puzzleSize; i++) {
					for(int j = 0; j < puzzleSize; j++) {
						if(sr.EndOfStream) {
							return;
						}
						switch((char)sr.Read()) {
							case 'x':
							case 'X':
								picross.Put(i, j, PicrossGrid.X);
								break;
							case '+':
								picross.Put(i, j, PicrossGrid.FILLED);
								break;

						}
					}
					if(sr.EndOfStream) {
						return;
					}
					sr.Read();
					sr.Read();
				}
			}
		}

		static bool Solve1(PicrossGrid picross, Func<PicrossGrid, int, int, int, bool, bool> checkFunc, bool showProgress) {
			bool stuck;
			do {
				stuck = true;
				for(int i = 0; i < picross.Size; i++) {
					for(int j = 0; j < picross.Size; j++) {
						if(picross.Empty(i, j)) {
							if(showProgress) {
								Console.WriteLine($"{i}, {j}");
							}
							bool tryFill1 = checkFunc(picross, PicrossGrid.FILLED, i, j, true);
							bool tryX1 = checkFunc(picross, PicrossGrid.X, i, j, true);
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
								tryFill2 = checkFunc(picross, PicrossGrid.FILLED, i, j, false);
								tryX2 = checkFunc(picross, PicrossGrid.X, i, j, false);

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

		static bool Check(PicrossGrid picross, int val, int row, int col, bool checkRow) {
			if(picross.At(row, col) == PicrossGrid.EMPTY) {
				picross.Put(row, col, val);

				//Console.WriteLine(picross);


				// Checks to reduce recursion
				int targetFill = InstructionSum(picross, checkRow ? row : picross.Size + col);
				int currentFill = LineSum(picross, checkRow ? row : picross.Size + col, PicrossGrid.FILLED);
				int currentX = LineSum(picross, checkRow ? row : picross.Size + col, PicrossGrid.X);
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

		static bool Check2(PicrossGrid picross, int val, int row, int col, bool checkRow) {
			if(picross.At(row, col) == PicrossGrid.EMPTY) {
				picross.Put(row, col, val);

				//Console.WriteLine(picross);

				/*
				// Checks to reduce recursion
				int targetFill = InstructionSum(picross, checkRow ? row : picross.Size + col);
				int currentFill = LineSum(picross, checkRow ? row : picross.Size + col, PicrossGrid.FILLED);
				int currentX = LineSum(picross, checkRow ? row : picross.Size + col, PicrossGrid.X);
				if(currentFill > targetFill || currentX > picross.Size - targetFill) {
					picross.Clear(row, col);
					return false;
				}
				*/
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

		static bool Recurse(PicrossGrid picross, int row, int col, bool checkRow) {

			if(row >= picross.Size || col >= picross.Size) {
				return true;
			}

			int nextRow = checkRow ? row : row + 1;
			int nextCol = checkRow ? col + 1 : col;

			if(picross.Empty(row, col)) {
				//Console.WriteLine(picross);
				picross.Put(row, col, PicrossGrid.FILLED);
				if((checkRow ? VerifyRow(picross, row, col) : VerifyCol(picross, col, row)) && Recurse(picross, nextRow, nextCol, checkRow)) {
					picross.Clear(row, col);
					return true;
				}
				picross.Clear(row, col);
				picross.Put(row, col, PicrossGrid.X);
				if((checkRow ? VerifyRow(picross, row, col) : VerifyCol(picross, col, row)) && Recurse(picross, nextRow, nextCol, checkRow)) {
					picross.Clear(row, col);
					return true;
				}
				picross.Clear(row, col);
				return false;
			} else {
				return (checkRow ? VerifyRow(picross, row, col) : VerifyCol(picross, col, row)) && Recurse(picross, nextRow, nextCol, checkRow);
			}
		}

		static int InstructionSum(PicrossGrid picross, int rowcol) {
			int sum = 0;
			foreach(int i in picross.GetInstruction(rowcol)) {
				sum += i;
			}
			return sum;
		}

		static int LineSum(PicrossGrid picross, int rowcol, int val) {
			int sum = 0;
			bool isRow = rowcol < picross.Size;
			for(int i = 0; i < picross.Size; i++) {
				if(picross.At(isRow ? rowcol : i, isRow ? i : rowcol - picross.Size) == val) {
					sum++;
				}
			}
			return sum;
		}

		static bool Solve2(PicrossGrid picross, int row, int col, bool showProgress) {
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
				if(Verify(picross, row, col) && Solve2(picross, nextRow, nextCol, showProgress)) {
					return true;
				}

				picross.Clear(row, col);
				picross.Put(row, col, PicrossGrid.X);
				if(Verify(picross, row, col) && Solve2(picross, nextRow, nextCol, showProgress)) {
					return true;
				}
				picross.Clear(row, col);
				return false;
			} else {
				return Verify(picross, row, col) && Solve2(picross, nextRow, nextCol, showProgress);
			}
		}

		static bool Verify(PicrossGrid picross, int row, int col) {
			return VerifyRow(picross, row, col) && VerifyCol(picross, col, row);
		}

		static bool VerifyRow(PicrossGrid picross, int row, int pos) {
			int[] instructions = picross.GetInstruction(row);

			if(picross.At(row, pos) == PicrossGrid.FILLED) {
				int groups = 0;
				int last = PicrossGrid.EMPTY;
				for(int i = 0; i <= pos; i++) {
					if(picross.At(row, i) == PicrossGrid.FILLED && last != PicrossGrid.FILLED) {
						groups++;
					}
					if(groups > instructions.Length) {
						return false;
					}
					last = picross.At(row, i);
				}
				int length = 0;
				for(int i = pos; i >= 0 && picross.At(row, i) == PicrossGrid.FILLED; i--) {
					length++;
					if(length > instructions[groups - 1]) {
						return false;
					}
				}


			} else if(pos > 0 && picross.At(row, pos - 1) == PicrossGrid.FILLED) {
				int groups = 0;
				int last = PicrossGrid.EMPTY;
				for(int i = 0; i <= pos; i++) {
					if(picross.At(row, i) == PicrossGrid.FILLED && last != PicrossGrid.FILLED) {
						groups++;
					}
					last = picross.At(row, i);
				}
				int length = 0;
				for(int i = pos - 1; i >= 0 && picross.At(row, i) == PicrossGrid.FILLED; i--) {
					length++;
				}
				if(length < instructions[groups - 1]) {
					return false;
				}
			}

			if(pos == picross.Size - 1) {
				return picross.IsValid(true, row);
			}
			return true;
		}

		static bool VerifyCol(PicrossGrid picross, int col, int pos) {
			int[] instructions = picross.GetInstruction(col + picross.Size);

			if(picross.At(pos, col) == PicrossGrid.FILLED) {
				int groups = 0;
				int last = PicrossGrid.EMPTY;
				for(int i = 0; i <= pos; i++) {
					if(picross.At(i, col) == PicrossGrid.FILLED && last != PicrossGrid.FILLED) {
						groups++;
					}
					if(groups > instructions.Length) {
						return false;
					}
					last = picross.At(i, col);
				}
				int length = 0;
				for(int i = pos; i >= 0 && picross.At(i, col) == PicrossGrid.FILLED; i--) {
					length++;
					if(length > instructions[groups - 1]) {
						return false;
					}
				}


			} else if(pos > 0 && picross.At(pos - 1, col) == PicrossGrid.FILLED) {
				int groups = 0;
				int last = PicrossGrid.EMPTY;
				for(int i = 0; i <= pos; i++) {
					if(picross.At(i, col) == PicrossGrid.FILLED && last != PicrossGrid.FILLED) {
						groups++;
					}
					last = picross.At(i, col);
				}
				int length = 0;
				for(int i = pos - 1; i >= 0 && picross.At(i, col) == PicrossGrid.FILLED; i--) {
					length++;
				}
				if(length < instructions[groups - 1]) {
					return false;
				}
			}

			if(pos == picross.Size - 1) {
				return picross.IsValid(false, col);
			}
			return true;
		}
	}
}