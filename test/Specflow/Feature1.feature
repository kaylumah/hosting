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
    Given something:
    When the files are retrieved:
    # Then 'a,b,c' are valid
