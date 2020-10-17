# Picross Solver
This program can solve picross puzzles (also known as nonograms). There are currently 3 algorithms that can be attempted.

## Inputing Puzzles
Puzzles are inputed via text files. File paths are relative to the Desktop.<br>
Some sample puzzles are [here](./Sample%20Puzzles).<br>

### File Format
Valid puzzle files must be formatted in the following way:<br>
The first line is n, the size of the puzzle i.e. height and width.<br>
The next 2n lines are the hints for each row, followed by the hints for each column.<br>
Optionaly, the next n lines can contain a partial solution for the puzzle. Symbols:<br>
- Use `+` for a filled cell.
- Use `X` for an unfilled cell.
- Use ` ` (space) for an unknown cell.

See [sample puzzles](./Sample%20Puzzles) for examples.

## Solve Versions
### Version 1
This version tries to solve the puzzle using a technique called line solving (see [this page](https://webpbn.com/index.cgi?page=solving.html) for explanation). The jist of the algorithim is that for a given cell, check if it is possible to fill or leave empty, while there still exists a configuration that satisfies the instructions for that row/column. If both are possible, then the cell stays unknown. If one is possible while the other isn't, then that cell must be filled with the possible option. If neither option is possible, then the puzzle is impossible to solve. The slowest part of this algorithim is the check to see if a possible configuration still exists, because if one doesn't, the program will check every possible combination for the remaining cells in the row or column. For a n x n puzzle, this is O(2^n). See version 3 to see an improved version of this. Some puzzles cannot be solved with just line solving, so this algorithm cannot solve all puzzles.

### Version 2
This version attempts to solve the puzzle using exhaustive search with backtracking. The way it checks if the row or column is invalid is much more flexible than in version 1. This version is the most negativly affected by choosing the "Show Progress" option due to the large amount of console ouput (eagle.txt is a good example of this). This approach should be able to solve any puzzle as long as a solution exists (eventually). The solve speed can vary greatly. For example, this is the fastest version to solve eagle.txt, but takes incredibly long to solve a puzzle of the same size like canada.txt.

### Version 3
This is my attempt to improve version 1 with the lessons learned from version 2. The overall approach is the same, but checking function now uses the exhaustive search and backtracking stratagy. The end result is a considerable improvement to speed. I would consider this the "best" version. It may take longer than version 2 on some puzzles, but it is still a reasonable amount of time, and it is able to solve the really bad ones for version 2 realtivly quickly (try canada.txt, toriel.txt, 10.txt). 
