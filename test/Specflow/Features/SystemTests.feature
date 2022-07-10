Feature: SystemTests

    Scenario: System Test
        Given the following defaults:
          | Scope | Path | Key    | Value |
          | posts |      | author | Max   |
          | posts |      | type | Article   |
        Given the following collections:
          | Name  | Output | TreatAs |
          | posts | true   | <null>  |
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