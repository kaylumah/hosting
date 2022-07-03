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

    Scenario: An empty file gets default data
        Given the following defaults:
          | Scope  | Path | Key    | Value |
          | <null> |      | author | Max   |
        Given '001.md' is an empty post:
        And '002.md' is an empty page:
        When the files are retrieved:
          | DirectoriesToSkip | FileExtensionsToTarget |
          |                   | .md                    |
        Then the following articles are returned:
          | Uri    | Title  | Description | Author | Created | Modified |
          | 001.md | <null> | <null>      | Max    | <null>  | <null>   |
          | 002.md | <null> | <null>      | Max    | <null>  | <null>   |

    Scenario: only scopes apply
        Given the following defaults:
          | Scope | Path | Key    | Value |
          | posts |      | author | Max   |
        Given the following collections:
          | Name  | Output | TreatAs |
          | posts | true   | <null>  |
        Given '001.md' is an empty post:
        And '002.md' is an empty page:
        When the files are retrieved:
          | DirectoriesToSkip | FileExtensionsToTarget |
          |                   | .md                    |
        Then the following articles are returned:
          | Uri    | Title  | Description | Author | Created | Modified |
          | 001.md | <null> | <null>      | Max    | <null>  | <null>   |
          | 002.md | <null> | <null>      | <null> | <null>  | <null>   |

    Scenario Outline: a file defines a custom output location

    Every file has a default output location of `/:year/:month/:day/:name:ext`

        Given the following extension mapping:
          | Key | Value |
          | .md | .html |
        Given '<OriginalFileName>' is a post with the following contents:
        """
        ---
        outputlocation: :name:ext
        ---
        """
        When the files are retrieved:
          | DirectoriesToSkip | FileExtensionsToTarget |
          |                   | .html, .txt            |
        Then the following articles are returned:
          | Uri              | Title  | Description | Author | Created   | Modified   |
          | <OutputLocation> | <null> | <null>      | <null> | <Created> | <Modified> |

        Examples:
          | Description         | OriginalFileName   | OutputLocation | Created                | Modified               |
          | extension changes   | 001.md             | 001.html       | <null>                 | <null>                 |
          | date prefix removed | 2022-01-01-002.txt | 002.txt        | 2022-01-01T00:00+01:00 | 2022-01-01T00:00+01:00 |
          | no changes applied  | 001.txt            | 001.txt        | <null>                 | <null>                 |