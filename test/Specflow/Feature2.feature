Feature: Feature2

A short summary of the feature

@tag1
Scenario: A
    Given the following default metadata:
        | scope  | path | key | value |
        | <null> |      |     |       |
        | <null> | 2022 |     |       |
    * the following extension mapping:
            | key | value |
            | .md | .html |
    Given the following files:
        | Name         |
        | template.txt |
    Given a file named 'template.txt' has the following contents:
        """
        # Hello World
        """
    When the file 'template.txt' is parsed:
    Then the following pages2:
        | Uri          |
        | template.txt |
