# TODO
This is a general TODO list for development. These items are not *necessarily* in order, but kind of are.

1. Finish unit tests for where clause
2. Validate fields being used in where clause match with the select fields (i.e. that we are using proper column names and not some random column)
3. from clause parsing (with support for derived tables / parenthesis).See WhereClause for example with parenthesis.
4. Fix error message bug with double-digit line numbers (see GitHub issue)