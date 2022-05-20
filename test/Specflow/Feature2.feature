Feature: Feature2

A short summary of the feature

@tag1
Scenario: A
    Given the following files:
        | Name         |
        | template.txt |
    Given a file named 'template.txt' has the following contents:
        """
        # Hello World
        """
    When the file 'template.txt' is parsed:
