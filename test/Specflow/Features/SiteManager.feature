Feature: SiteManager

    Scenario: Add two numbers
        Given the following site info:
          | Url                 | BaseUrl |
          | https://example.com | <null>  |
        Given the following collections:
          | Name  | Output | TreatAs |
          | posts | true   | <null>  |
        When the site is generated:
        Then the scenario executed successfully: