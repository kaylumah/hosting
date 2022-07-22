Feature: SiteManager

    Scenario: Add two numbers
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
        When the site is generated v2:
        Then the following: