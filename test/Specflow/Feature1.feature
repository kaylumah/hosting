Feature: Feature1

A short summary of the feature

@tag1
Scenario: MyFirstScenario
    Given the following extensions:
        | key | value |
        | .md | .html |
    Given the extensions '.md,.txt' are targeted
    And file '_site/file1.md' has the following contents:
        """
        ---
        title: my title
        ---
        # Hello World
        """
    And file '_site/_posts/2019-09-07-file1.md' has the following contents:
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
    Then the following pages:
        | uri                   |
        | file1.html            |
        | 2019/09/07/file1.html |