Feature: File Processor Tests

  Scenario: empty filter results in 0 files
    Given '001.md' is an empty post:
    When the files are retrieved:
      | RootDirectory | DirectoriesToSkip | FileExtensionsToTarget |
      | _site         |                   |                        |
    Then no articles are returned:

  Scenario: a file whoes extension changes is not returned
    Given '001.md' is an empty post:
    And the following extension mapping:
      | Key | Value |
      | .md | .html |
    When the files are retrieved:
      | RootDirectory | DirectoriesToSkip | FileExtensionsToTarget |
      | _site         |                   | .md                    |
    Then no articles are returned:

  Scenario: a file whoes extension changes can be returned
    Given '001.md' is an empty post:
    And the following extension mapping:
      | Key | Value |
      | .md | .html |
    When the files are retrieved:
      | RootDirectory | DirectoriesToSkip | FileExtensionsToTarget |
      | _site         |                   | .html                  |
    Then the following articles are returned:
      | Uri      | Title  | Description | Author | Created | Modified |
      | 001.html | <null> | <null>      | <null> | <null>  | <null>   |

  Scenario: only files matching the filter are returned
    Given '001.md' is an empty post:
    Given '001.txt' is an empty post:
    When the files are retrieved:
      | RootDirectory | DirectoriesToSkip | FileExtensionsToTarget |
      | _site         |                   | .txt                   |
    Then the following articles are returned:
      | Uri     | Title  | Description | Author | Created | Modified |
      | 001.txt | <null> | <null>      | <null> | <null>  | <null>   |

  Scenario: An empty file gets no data
    Given '001.md' is an empty post:
    When the files are retrieved:
      | RootDirectory | DirectoriesToSkip | FileExtensionsToTarget |
      | _site         |                   | .md                    |
    Then the following articles are returned:
      | Uri    | Title  | Description | Author | Created | Modified |
      | 001.md | <null> | <null>      | <null> | <null>  | <null>   |

  Scenario: An empty file gets default data
    Given the following defaults:
      | Scope  | Path | Key    | Value |
      | <null> |      | author | Max   |
    Given '001.md' is an empty post:
    And '002.md' is an empty page:
    And the following extension mapping:
      | Key | Value |
      | .md | .html |
    When the files are retrieved:
      | RootDirectory | DirectoriesToSkip | FileExtensionsToTarget |
      | _site         |                   | .html                  |
    Then the following articles are returned:
      | Uri      | Title  | Description | Author | Created | Modified |
      | 001.html | <null> | <null>      | Max    | <null>  | <null>   |
      | 002.html | <null> | <null>      | Max    | <null>  | <null>   |

  Scenario: An file with a date pattern gets timestamps
    Given '2022-03-05-001.md' is an empty post:
    And the following extension mapping:
      | Key | Value |
      | .md | .html |
    When the files are retrieved:
      | RootDirectory | DirectoriesToSkip | FileExtensionsToTarget |
      | _site         |                   | .html                  |
    Then the following articles are returned:
      | Uri                 | Title  | Description | Author | Created    | Modified   |
      | 2022/03/05/001.html | <null> | <null>      | <null> | 2022-03-05 | 2022-03-05 |

  Scenario: only scopes apply
    Given the following defaults:
      | Scope | Path | Key    | Value |
      | posts |      | author | Max   |
    Given the following collections:
      | Name  | Output | TreatAs |
      | posts | true   | <null>  |
    Given '001.md' is an empty post:
    And '002.md' is an empty page:
    And the following extension mapping:
      | Key | Value |
      | .md | .html |
    When the files are retrieved:
      | RootDirectory | DirectoriesToSkip | FileExtensionsToTarget |
      | _site         |                   | .html                  |
    Then the following articles are returned:
      | Uri      | Title  | Description | Author | Created | Modified |
      | 001.html | <null> | <null>      | Max    | <null>  | <null>   |
      | 002.html | <null> | <null>      | <null> | <null>  | <null>   |

  Scenario Outline: most specfic scopes apply
    Given the following defaults:
      | Scope  | Path | Key    | Value          |
      | <null> |      | author | everyone       |
      | posts  |      | author | all-posts      |
      | posts  | 2022 | author | all-2022-posts |
    Given '<OriginalFileName>' is an empty file:
    And the following extension mapping:
      | Key | Value |
      | .md | .html |
    When the files are retrieved:
      | RootDirectory | DirectoriesToSkip | FileExtensionsToTarget |
      | _site         |                   | .html                  |
    Then the following articles are returned:
      | Uri              | Title  | Description | Author   | Created   | Modified   |
      | <OutputLocation> | <null> | <null>      | <Author> | <Created> | <Modified> |

    Examples:
      | Description | OriginalFileName         | OutputLocation      | Author         | Created                | Modified               |
      |             | _pages/001.md            | 001.html            | everyone       | <null>                 | <null>                 |
      |             | _posts/002.md            | 002.html            | all-posts      | <null>                 | <null>                 |
      |             | _posts/2022-01-01-003.md | 2022/01/01/003.html | all-2022-posts | 2022-01-01T00:00+01:00 | 2022-01-01T00:00+01:00 |

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
      | RootDirectory | DirectoriesToSkip | FileExtensionsToTarget |
      | _site         |                   | .html, .txt            |
    Then the following articles are returned:
      | Uri              | Title  | Description | Author | Created   | Modified   |
      | <OutputLocation> | <null> | <null>      | <null> | <Created> | <Modified> |

    Examples:
      | Description         | OriginalFileName    | OutputLocation | Created                | Modified               |
      | extension changes   | 001.md              | 001.html       | <null>                 | <null>                 |
      | date prefix removed | 2022-01-01-002.html | 002.html       | 2022-01-01T00:00+01:00 | 2022-01-01T00:00+01:00 |
      | no changes applied  | 001.html            | 001.html       | <null>                 | <null>                 |