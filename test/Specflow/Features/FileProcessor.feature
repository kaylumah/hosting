Feature: File Processor Tests

    Scenario: empty filter results in 0 files
        Given '001.md' is an empty post:
        When the files are retrieved:
          | DirectoriesToSkip | FileExtensionsToTarget |
          |                   |                        |
        Then no articles are returned:

    Scenario: a file whoes extension changes is not returned
        Given '001.md' is an empty post:
        And the following extension mapping:
          | Key | Value |
          | .md | .html |
        When the files are retrieved:
          | DirectoriesToSkip | FileExtensionsToTarget |
          |                   | .md                    |
        Then no articles are returned:

    Scenario: a file whoes extension changes can be returned
        Given '001.md' is an empty post:
        And the following extension mapping:
          | Key | Value |
          | .md | .html |
        When the files are retrieved:
          | DirectoriesToSkip | FileExtensionsToTarget |
          |                   | .html                  |
        Then the following articles are returned:
          | Uri      | Title  | Description | Author | Created | Modified |
          | 001.html | <null> | <null>      | <null> | <null>  | <null>   |

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

    Scenario: An file with a date pattern gets timestamps
        Given '2022-03-05-001.md' is an empty post:
        When the files are retrieved:
          | DirectoriesToSkip | FileExtensionsToTarget |
          |                   | .md                    |
        Then the following articles are returned:
          | Uri               | Title  | Description | Author | Created    | Modified   |
          | 2022/03/05/001.md | <null> | <null>      | <null> | 2022-03-05 | 2022-03-05 |

    Scenario: a file with a custom output location controls output location
        Given '001.md' is a post with the following contents:
        """
        ---
        outputlocation: changed/:name:ext
        ---
        """
        When the files are retrieved:
          | DirectoriesToSkip | FileExtensionsToTarget |
          |                   | .md                    |
        Then the following articles are returned:
          | Uri            | Title | Description | Author | Created | Modified |
          | changed/001.md | <null> | <null>      | <null> | <null>  | <null>   |


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