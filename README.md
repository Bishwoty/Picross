# Picross Solver
This program can solve picross puzzles (also known as nonograms). There are 3 algorithms that can be attempted.

### Inputing Puzzles
Puzzles are inputed via text files. File paths are relative to the Desktop.<br>
Some sample puzzles are [here](./Sample%20Puzzles).<br>

#### File Format
Valid puzzle files must be formatted in the following way:<br>
The first line is n, the size of the puzzle i.e. height and width.<br>
The next 2n lines are the hints for each row, followed by the hints for each column.<br>
Optionaly, the next n lines can contain a partial solution for the puzzle. Symbols:<br>
- Use `+` for a filled cell.
- Use `X` for an unfilled cell.
- Use ` ` (space) for an unknown cell.

See [sample puzzles](./Sample%20Puzzles) for examples.
