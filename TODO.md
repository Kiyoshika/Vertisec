# TODO
This is a general TODO list for development. These items are not *necessarily* in order, but kind of are.

1. Rework WhereClause. Instead of the loop approach, use a `while (IndexOf(conditionWord) != null)` to build the token buffer to make things easier
	* can refer to the FromClause as I moved that away from the loop approach. the loops just make things a bit more annoying to deal with
2. Write unit tests for From and Where clause (MUST NEED - I broke the WhereClause parenthesis parsing after reworking the ParenthesisParser)
3. Fix error message bug with double-digit line numbers (see GitHub issue)
