Feature: User is requesting comment by its id

Scenario: Id is empty guid.
	Given Guid id is empty.
	Then ArgumentException should be thrown.

Scenario: No connection with database.
	Given Guid id not empty.
	But Something wrong with database.
	Then Exception should be thrown.

Scenario: There isn't comment with this id.
	Given Guid id not empty.
	But Comment with this id not exists.
	Then NullReferenceException should be thrown.

Scenario: Successful path.
	Given Guid id not empty.
	And Comment with this id exists.
	Then Should return mapped comment.