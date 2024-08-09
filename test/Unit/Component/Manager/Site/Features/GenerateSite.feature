@SiteManager
Feature: SiteManager GenerateSite

    Background: 
        Given the current date is '2022-08-10': 
    
    Scenario: Generate website
        Given the following defaults:
          | Scope | Path | Key     | Value   |
          | posts |      | type    | Article |
          | posts |      | feed    | True    |
          | posts |      | sitemap | True    |
        Given the following collections:
          | Name  | Output | TreatAs |
          | posts | true   | <null>  |
        Given the following authors:
          | Id  | Name         | Email           | Uri                 | Picture                        |
          | Max | Max Hamulyák | max@kaylumah.nl | https://kaylumah.nl | https://kaylumah.nl/avatar.png |
        And the following organizations:
          | Id       | Name     |
          | Kaylumah | Kaylumah |
        And the following tags:
          | Id       |
          | dotnet   |
          | specflow |
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
          | Uri          | Title | Description | Author | Created    | Modified   | Tags             |
          | example.html | Title | Description | Max    | 2022-07-03 | 2022-07-03 | dotnet, specflow |
        Given the following site info:
          | Title    | Description | Language | Url                 | BaseUrl | SupportedFileExtensions |
          | Kaylumah | My Blog     | en       | https://example.com | <null>  | .html                   |
        When the site is generated:
        Then the scenario executed successfully:
        And the following artifacts are created:
          | Path         |
          | example.html |
          | sitemap.xml  |
          | feed.xml     |
          | search.json  |
        And the atom feed 'feed.xml' is verified:
        And the sitemap 'sitemap.xml' is verified:
        # And the html 'example.html' is verified:
        And 'example.html' is a document with the following meta tags:
          | Tag                    | Value                             |
          | generator              | Kaylumah vd8b6637                 |
          | description            | Description                       |
          | copyright              | © Kaylumah. All rights reserved.  |
          | keywords               | dotnet, specflow                  |
          | og:type                | article                           |
          | og:locale              | en                                |
          | og:site_name           | Kaylumah                          |
          | og:title               | Title                             |
          | og:url                 | https://example.com/example.html  |
          | og:description         | Description                       |
          | article:author         | Max Hamulyák                      |
          | article:published_time | 2022-07-03T00:00:00.0000000+02:00 |
          | article:modified_time  | 2022-07-03T00:00:00.0000000+02:00 |
          | article:tag            | dotnet                            |
          | article:tag            | specflow                          |
          | twitter:card           | summary_large_image               |
          | twitter:title          | Title                             |
          | twitter:description    | Description                       |
          | twitter:creator        | @Max                              |