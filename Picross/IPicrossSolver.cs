using System;

namespace Picross {
	interface IPicrossSolver {
		bool Solve(PicrossGrid picross, bool showProgress);
	}
}
