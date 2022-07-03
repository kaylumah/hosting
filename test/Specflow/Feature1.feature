Feature: File Processor Tests

    Scenario: empty filter results in 0 files
        Given '001.md' is an empty post:
        When the files are retrieved:
          | DirectoriesToSkip | FileExtensionsToTarget |
          |                   |                        |
        Then no articles are returned:

    Scenario: only files matching the filter are returned
        Given '001.md' is an empty post:
        Given '001.txt' is an empty post:
        When the files are retrieved:
          | DirectoriesToSkip | FileExtensionsToTarget |
          |                   | .txt                   |
        Then the following articles are returned:
          | Uri     | Title  | Description | Author | Created | Modified |
          | 001.txt | <null> | <null>      | <null> | <null>  | <null>   |

    Scenario: An empty file gets no data
        Given '001.md' is an empty post:
        When the files are retrieved:
          | DirectoriesToSkip | FileExtensionsToTarget |
          |                   | .md                    |
        Then the following articles are returned:
          | Uri    | Title  | Description | Author | Created | Modified |
          | 001.md | <null> | <null>      | <null> | <null>  | <null>   |


    #    Scenario Outline: Attemp1
    #        Given the following defaults:
    #          | Scope  | Path | Key    | Value |
    #          | <null> |      | author | Max   |
    #        * the following extension mapping:
    #            | Key | Value |
    #            | .md | .html |
    #        Given the following collections:
    #            | Name  | Output | TreatAs |
    #            | posts | true   | <null>  |
    #        Given '<FileName>' is an empty post:
    #        When the files are retrieved:
    #          | DirectoriesToSkip | FileExtensionsToTarget |
    #          |                   | .md, .txt, .html       |
    #        Then the following V2:
    #          | Uri   | Title  | Description | Author   | Created   | Modified   |
    #          | <Uri> | <null> | <null>      | <Author> | <Created> | <Modified> |
    #        Examples:
    #         | FileName   | Uri          | Author | Created | Modified |
    #         | example.md | example.html | Max    | <null>  | <null>   |