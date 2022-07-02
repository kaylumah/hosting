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
        |                   | .md, .txt              |
    Then the following:
        | Path                | Key        | Value               |
        | demo.md             | uri        | demo.md             |
        | demo.md             | collection | posts               |
        | not-demo.md         | uri        | not-demo.md         |
        | not-demo.md         | collection | posts               |
        | with-frontmatter.md | uri        | with-frontmatter.md |
        | with-frontmatter.md | collection | posts               |
        | with-frontmatter.md | output     | true                |
