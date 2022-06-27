# TODO
This is a general TODO list for development. These items are not *necessarily* in order, but kind of are.

1. Finish unit tests for where clause
2. Disallow subqueries as left-hand expression in WhereClause, e.g.
```sql
where (select x from y) = jimmy
```
3. Validate fields being used in where clause match with the select fields (i.e. that we are using proper column names and not some random column)
4. from clause parsing (with support for derived tables / parenthesis).See WhereClause for example with parenthesis.
5. Fix error message bug with double-digit line numbers (see GitHub issue)


Sample complicated parenthesis SQL for testing:
```sql
select
	wh_id,
	location_id
from
	thingy
where
	jimmy = (3 = 1 or johnny like 'yo jimmy')
	and x != 12
	or location_id in (select thing from other_thing)
```