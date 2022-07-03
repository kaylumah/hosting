Feature: FileProcessor_OutputLocation

Every file has a default output location of `/:year/:month/:day/:name:ext`

    Scenario Outline: a file defines a custom output location

        Given '<OriginalFileName>' is a post with the following contents:
        """
        ---
        outputlocation: :name:ext
        ---
        """
        When the files are retrieved:
          | DirectoriesToSkip | FileExtensionsToTarget |
          |                   | .md                    |
        Then the following articles are returned:
          | Uri              | Title  | Description | Author | Created   | Modified   |
          | <OutputLocation> | <null> | <null>      | <null> | <Created> | <Modified> |

        Examples:
          | Description         | OriginalFileName  | OutputLocation | Created                | Modified               |
          | unmodified          | 001.md            | 001.md         | <null>                 | <null>                 |
          | date prefix removed | 2022-01-01-001.md | 001.md         | 2022-01-01T00:00+01:00 | 2022-01-01T00:00+01:00 |