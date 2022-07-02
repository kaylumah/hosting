Feature: Feature Two

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
    Given post 'demo.md' has the following contents:
        """
        Hello World
        """
    And a test post named 'not-demo.md':
    And a test post v2 named 'with-frontmatter.md':
    When the files are retrieved:
        | directoriesToSkip | targetExtensions |
        |                   | .md, .txt        |
