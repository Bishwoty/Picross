using System;
using System.IO;
using System.Diagnostics;

namespace Picross {
	class Program {
		static void Main(string[] args) {
			while(true) {
				// Choose version to use
				IPicrossSolver solver = new LineSolve();
				for(bool valid = false; !valid;) {
					valid = true;
					Console.WriteLine("Solve Version?");
					int solveVersion;
					valid = int.TryParse(Console.ReadLine(), out solveVersion);
					switch(solveVersion) {
						case 1:
							solver = new LineSolve();
							break;
						case 2:
							solver = new BruteForce();
							break;
						case 3:
							solver = new LineSolve2();
							break;
						case 4:
							solver = new LineSolveOptimize();
							break;
						default:
							valid = false;
							Console.WriteLine("Invalid Solve Version.");
							break;
					}
				}

				// Show in termediate steps?
				Console.WriteLine("Show Progress? (y/n)");
				bool showProgress = Char.ToUpper((char)Console.Read()) == 'Y';
				Console.ReadLine(); // Eat rest of line

				// Read in Puzzle
				PicrossGrid game = new PicrossGrid(0);
				ReadPuzzle(ref game);


				bool result = false;
				Stopwatch timer = new Stopwatch();
				timer.Start();
				result = solver.Solve(game, showProgress);
				timer.Stop();
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
	}
}