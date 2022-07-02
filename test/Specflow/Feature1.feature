Feature: Feature One
    
  Scenario: Test
      Given post 'sample_001.md' has the following contents:
          """
          ---
          author: max
          ---
          """
      When the files are retrieved:
        | DirectoriesToSkip | FileExtensionsToTarget |
        |                   | .md, .txt              |
      Then the following:
        | Path          | Key        | Value         |
        | sample_001.md | uri        | sample_001.md |
        | sample_001.md | collection | posts         |
        | sample_001.md | author     | max         |

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
