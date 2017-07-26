Feature: Login Account
	In order to automate repetitive tasks
	As an smart web tester
	I want to create account in automation practice website for script development

@CreateAccount
Scenario: Login Account in Automation Practice Website
	Given I Navigate to website "Automation Practice"
    When I login to website for user "test@testauto.com" with default password
	Then I should be logged in successfully
	And My account page is displayed