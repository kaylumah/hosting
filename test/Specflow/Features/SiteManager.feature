Feature: SiteManager

    Scenario: Add two numbers
        Given the following authors:
          | Id  |
          | Max |
        Given the following organizations:
          | Id       |
          | Kaylumah |
        Given the following tags:
          | Id     |
          | dotnet |
        Given 'default.html' is a layout file with the following contents:
        """
        <!DOCTYPE html>
        <html>
            <head>
            {{ page.metatags }}
            </head>
            <body>
            </body>
        </html>
        """
        Given the following articles:
          | Uri          | Title  | Description | Author | Created    | Modified   |
          | example.html | <null> | <null>      | <null> | 2022-07-03 | 2022-07-03 |
        Given the following site info:
          | Url                 | BaseUrl | SupportedFileExtensions |
          | https://example.com | <null>  | .html                   |
        Given the following collections:
          | Name  | Output | TreatAs |
          | posts | true   | <null>  |
        When the site is generated:
        Then the scenario executed successfully:
        And the following artifacts are created:
          | Path         |
          | example.html |
          | sitemap.xml  |
          | feed.xml     |
        And the atom feed artifacts has the following articles:
        And 'example.html' is a document with the following meta tags:
          | Tag                    | Value                             |
          | generator              | Kaylumah vd8b6637                 |
          | description            |                                   |
          | copyright              | © Kaylumah. All rights reserved.  |
          | keywords               |                                   |
          | og:type                | article                           |
          | og:locale              |                                   |
          | og:site_name           |                                   |
          | og:title               |                                   |
          | og:url                 | https://example.com/example.html  |
          | og:description         |                                   |
          | article:published_time | 2022-07-03T00:00:00.0000000+02:00 |
          | article:modified_time  | 2022-07-03T00:00:00.0000000+02:00 |
          | twitter:card           | summary_large_image               |
          | twitter:title          |                                   |
          | twitter:description    |                                   |

    Scenario: System Test
        Given the following defaults:
          | Scope | Path | Key     | Value   |
          | posts |      | type    | Article |
          | posts |      | feed    | True    |
          | posts |      | sitemap | True    |
        Given the following collections:
          | Name  | Output | TreatAs |
          | posts | true   | <null>  |
        Given the following articles v2:
          | Uri      | Title  | Description | Author | Created | Modified |
          | 001.html | <null> | <null>      | Max    | <null>  | <null>   |
        Given the following site info:
          | Url                 | BaseUrl | SupportedFileExtensions |
          | https://example.com | <null>  | .html                   |
        Given 'example.html' is an empty post:
        And 'authors.yml' is a data file with the following contents:
        """
        """
        And 'default.html' is a layout file with the following contents:
        """
         <!DOCTYPE html>
         <html>
             <head>
             {{ page.metatags }}
             </head>
             <body>
             </body>
         </html>
        """
        And 'image.xml' is an asset file with the following contents:
        """
        """
        When the site is generated:
        Then the following: