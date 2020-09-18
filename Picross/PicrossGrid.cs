using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picross {
	class PicrossGrid {

		public const int EMPTY = 0;
		public const int FILLED = 1;
		public const int X = 2;

		public int Size { get; private set; } // Height and width of the Grid

		// Constructs empty puzzle with no instructions
		public PicrossGrid(int size) {
			Size = size;
			grid = new int[Size, Size];
			for(int i = 0; i < Size; i++) {
				for(int j = 0; j < Size; j++) {
					grid[i, j] = EMPTY;
				}
			}
			instructions = new List<int>[Size * 2];
			for(int i = 0; i < Size * 2; i++) {
				instructions[i] = new List<int>();
			}
		}

		// Sets the instructions for a row or column
		public void SetInstruction(int rowcol, int[] values) {
			instructions[rowcol].Clear();
			foreach(int val in values) {
				if(val > 0) {
					instructions[rowcol].Add(val);
				}
			}
		}

		// check if the cell at the specified coordinates is empty
		public bool Empty(int row, int col) {
			return grid[row, col] == EMPTY;
		}

		// fill the cell at the specified coordinates to be FILLED or X
		// Any previous value is overwritten.
		public void Put(int row, int col, int val) {
			if(row < 0 || row >= Size) {
				throw new System.ArgumentException("Invalid Parameter", "row");
			}
			if(col < 0 || col >= Size) {
				throw new System.ArgumentException("Invalid Parameter", "col");
			}
			if(val != FILLED && val != X) {
				throw new System.ArgumentException("Invalid Parameter", "val");
			}
			grid[row, col] = val;
		}

		// Gets the current state of a cell
		public int At(int row, int col) {
			if(row < 0 || row >= Size) {
				throw new System.ArgumentException("Invalid Parameter", "row");
			}
			if(col < 0 || col >= Size) {
				throw new System.ArgumentException("Invalid Parameter", "col");
			}
			return grid[row, col];
		}

		// Gets the Instructions for a row or column
		public int[] GetInstruction(int rowcol) {
			return instructions[rowcol].ToArray();
		}

		// clear a particular cell
		public void Clear(int row, int col) {
			if(row < 0 || row >= Size) {
				throw new System.ArgumentException("Invalid Parameter", "row");
			}
			if(col < 0 || col >= Size) {
				throw new System.ArgumentException("Invalid Parameter", "col");
			}
			grid[row, col] = EMPTY;
		}

		// clear the entire grid
		public void Clear() {
			for(int i = 0; i < Size; i++) {
				for(int j = 0; j < Size; j++) {
					grid[i, j] = EMPTY;
				}
			}
		}

		// Checks if a row or column has its instructions satified
		public bool IsValid(bool isRow, int rowcol) {
			if(rowcol > Size) {
				throw new System.ArgumentException("Invalid Parameter", "rowcol");
			}

			int pos = 0; // position within the row or column

			foreach(int instruct in instructions[rowcol + ((isRow) ? 0 : Size)]) {
				// No space for next instruction
				if(pos >= Size) {
					return false;
				}
				// Case: Checking row
				if(isRow) {
					//Find first filled cell
					while(grid[rowcol, pos] != FILLED) {
						pos++;
						if(pos >= Size) {
							// End of row found without finishing
							return false;
						}
					}
					for(int i = 1; i < instruct; i++) {
						pos++;
						if(pos >= Size) {
							// End of row found without finishing
							return false;
						}
						if(grid[rowcol, pos] != FILLED) {
							// Filled Cells shorter than instruction
							return false;
						}
					}
					// Instruction must be followed by unfilled cell or end of row
					if(pos < Size - 1) {
						if(grid[rowcol, pos + 1] == FILLED) {
							return false;
						}
					}
					pos++;
				}

				// Case: Checking Column
				else {
					//Find first filled cell
					while(grid[pos, rowcol] != FILLED) {
						pos++;
						if(pos >= Size) {
							// End of column found without finishing
							return false;
						}
					}
					for(int i = 1; i < instruct; i++) {
						pos++;
						if(pos >= Size) {
							// End of column found without finishing
							return false;
						}
						if(grid[pos, rowcol] != FILLED) {
							// Filled Cells shorter than instruction
							return false;
						}
					}
					// Instruction must be followed by unfilled cell or end of column
					if(pos < Size - 1) {
						if(grid[pos + 1, rowcol] == FILLED) {
							return false;
						}
					}
					pos++;
				}
			}

			// After all instructions, all remaining cells must not be filled
			while(pos < Size) {
				// Case: Checking row
				if(isRow) {
					if(grid[rowcol, pos] == FILLED) {
						// Extra filled cell found
						return false;
					}
				}
				// Case: Checking column
				else {
					if(grid[pos, rowcol] == FILLED) {
						// Extra filled cell found
						return false;
					}
				}
				pos++;
			}
			// Valid check
			return true;
		}

		//Checks if Puzzle is completed
		public bool IsValid() {
			for(int i = 0; i < Size; i++) {
				if(!IsValid(true, i) || !IsValid(false, i)) {
					return false;
				}
			}
			return true;
		}

		public override string ToString() {
			string str = "╔";
			for(int i = 0; i < Size; i++) {
				str += '═';
			}
			str += "╗\n";

			for(int i = 0; i < Size; i++) {
				str += '║';
				for(int j = 0; j < Size; j++) {
					switch(grid[i, j]) {
						case EMPTY:
							str += ' ';
							break;
						case FILLED:
							str += '█';
							break;
						case X:
							str += 'X';
							break;
						default:
							str += '?';
							break;
					}
				}
				str += "║\n";
			}
			str += '╚';
			for(int i = 0; i < Size; i++) {
				str += '═';
			}
			str += '╝';
			return str;
		}

		private int[,] grid;

		private List<int>[] instructions;


	}
}
