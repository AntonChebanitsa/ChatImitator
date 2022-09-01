Feature: You should get comment by its id

Scenario: Id is empty guid.
	Given Guid id is empty.
	Then ArgumentException should be thrown.

Scenario: No connection with database.
	Given Guid is correct.
	But Something wrong with database connention.
	Then DbException should be thrown.

Scenario: There isn't comment with this id.
	Given Guid isn't empty.
	And Database connection is ok.
	But There isn't comment with this id.
	Then ArgumentException should be thrown.

Scenario: Succesfull path.
	Given Correct guid id.
	And Database connection is ok.
	And Comment with this id exists.
	Then Should return mapped comment.