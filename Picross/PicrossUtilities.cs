using System;

namespace Picross {
	static class PicrossUtilities {
		public static int InstructionSum(PicrossGrid picross, int rowcol) {
			int sum = 0;
			foreach(int i in picross.GetInstruction(rowcol)) {
				sum += i;
			}
			return sum;
		}

		public static int LineSum(PicrossGrid picross, int rowcol, int val) {
			int sum = 0;
			bool isRow = rowcol < picross.Size;
			for(int i = 0; i < picross.Size; i++) {
				if(picross.At(isRow ? rowcol : i, isRow ? i : rowcol - picross.Size) == val) {
					sum++;
				}
			}
			return sum;
		}

		public static bool Verify(PicrossGrid picross, int row, int col) {
			return VerifyRow(picross, row, col) && VerifyCol(picross, col, row);
		}

		public static bool VerifyRow(PicrossGrid picross, int row, int pos) {
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

		public static bool VerifyCol(PicrossGrid picross, int col, int pos) {
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
