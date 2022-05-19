Feature: Feature1

A short summary of the feature

@tag1
Scenario: MyFirstScenario
    Given the extensions '.md,.txt' are targeted
    And file '_site/file2.md' has the following contents:
        """
        ---
        title: my title
        ---
        # Hello World
        """
	Given scope '[string]' has the following metadata:
        | key    | value |
        | number | 1     |
        | text   | abc   |
        | expr   | true  |
    When something else
    When something
    Then something
        | key | value   |
        | A   |         |
        | B   | ''      |
        | C   | <value> |
        | D   | <null> |
        | uri | 2       |