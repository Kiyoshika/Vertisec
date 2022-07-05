# TODO
This is a general TODO list for development. These items are not *necessarily* in order, but kind of are.

1. Finish unit tests for where clause
2. from clause parsing (with support for derived tables / parenthesis).See WhereClause for example with parenthesis.
	* NOTE: instead of iterating over each fromToken (like currently), parse everything at once similar to how I look at chunks at a time in different clauses when I hit certain keywords. There's no need to iterate token-by-token for a FromClause
3. Fix error message bug with double-digit line numbers (see GitHub issue)
