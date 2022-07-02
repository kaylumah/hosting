Feature: Feature One
  
  @ignore
  Scenario: Empty file gets default metadata
      Given '2022-01-01-example.md' is an empty post:
      When the files are retrieved:
        | DirectoriesToSkip | FileExtensionsToTarget |
        |                   | .md, .txt              |
      Then the following:
        | Path       | Key        | Value                 |
        | example.md | uri        | 2022/01/01/example.md |
        | example.md | collection | posts                 |
        | example.md | published  | 2022-1-1              |
        | example.md | modified   | 2022-1-1              |
        
    
  Scenario: Test
      Given the following defaults:
          | scope  | path | key    | value |
#          | <null> |      | author | max   |
          | posts  |      | feed   | true  |
#          |        |      | feed   | true  |
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
        | sample_001.md | author     | max           |
        | sample_001.md | feed     | true           |

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
