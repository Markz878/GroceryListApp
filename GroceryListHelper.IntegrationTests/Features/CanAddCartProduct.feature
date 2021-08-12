@CanAddCartProduct
Feature: User can add cart product to cart

Scenario: Add product to cart
	Given a user in the index page
	When the product properties are written to the input fields
	And the add button is clicked
	Then the item should be added to the cart