Feature: Feature One

  Scenario: Different Givens
    # Given the following blog articles:
    #   | Title  | Tags   |
    #   | <null> | <null> |
    #   | <null> |        |
    #   | a      | a      |
    #   | a      | a, b   |
    #   | a,b    | a, b   |
    # * the following blog posts:
    #   | Title | Description | Tags |
    #   | A     | B           |      |
    Given file 'demo.md' has the following contents:
        """
        Hello World
        """
    When the files are retrieved:
        | directoriesToSkip | targetExtensions |
        |                   | .md, .txt        |
