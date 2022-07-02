Feature: Feature One

  Scenario: Different Givens
    Given post 'demo.md' has the following contents:
        """
        Hello World
        """
    And a test post named 'not-demo.md':
    And a test post v2 named 'with-frontmatter.md':
    When the files are retrieved:
        | DirectoriesToSkip | FileExtensionsToTarget |
        |                   | .md, .txt        |
