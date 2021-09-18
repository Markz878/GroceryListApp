@AnonymousUserCartInteraction
Feature: Anonymous User Cart Interaction

Background: User is not logged in, but interacts with the cart

Scenario: Add valid product to cart
	Given a user in the index page
	When a valid product's properties are written to the input fields
	And the add button is clicked
	Then the item should be added to the cart

Scenario: Add invalid product to cart
	Given a user in the index page
	When an invalid product's properties are written to the input fields
	And the add button is clicked
	Then an error message should be shown

Scenario: Reorder cart items
	Given a user in the index page
	And the following products in cart
		| Name | Amount | Price |
		| Milk | 2		| 2.5	|
		| Meat | 1		| 4		|
	When the products are reordered using the Reorder-button
	Then the items should be in the order
		| Name | Amount | Price |
		| Meat | 1		| 4		|
		| Milk | 2		| 2.5	|

Scenario: User can delete product from cart
	Given a user in the index page
	And the following products in cart
		| Name | Amount | Price |
		| Milk | 2		| 2.5	|
	When a user clicks the delete button on the item
	Then the item should be removed from the cart

Scenario: Cart shows correct total sum
	Given a user in the index page
	And the following products in cart
		| Name | Amount | Price |
		| Milk | 2		| 2.5	|
		| Meat | 1		| 4		|
	Then the cart total should be 9

Scenario: Edit cart items
	Given a user in the index page
	And the following products in cart
		| Name | Amount | Price |
		| Milk | 2		| 2.5	|
	When user starts to edit item
	And changes the item amount to 1 and price to 3
	Then the item amount should be 1 and price 3

Scenario: Cart shows when all items are collected
	Given a user in the index page
	And the following products in cart
		| Name | Amount | Price |
		| Milk | 2		| 2.5	|
		| Meat | 1		| 4		|
	When the 2 products are checked to have been collected
	Then the all-collected check should say "All collected!"
