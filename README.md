# Kaylumah.GitHub.io

<p align="center">
  <img alt="Kaylumah Logo" width="460" height="300" src="meta/resources/logo.svg">
</p>

[![CI/CD](https://github.com/kaylumah/hosting/actions/workflows/azure-static-web-apps-green-field-0353fee03.yml/badge.svg?branch=main)](https://github.com/kaylumah/hosting/actions/workflows/azure-static-web-apps-green-field-0353fee03.yml)

---

## Description

This repository is the home for all content of kaylumah.nl. Here, you can find the source code used to generate pages and all my blog posts.

This GitHub project board tracks the status of any planned changes and articles to my website.

## Powered by

| Icon | Name | Description |
| - | - | - |
| <img src="https://github.com/scriban/scriban/raw/master/img/scriban.png" alt="" width="50"/> | Scriban | Template engine|
| <img src="https://github.com/xoofx/markdig/raw/master/img/markdig.png" alt="" width="50"/> | Markdig | Blogs are written in Markdown and converted to HTML |


## Documentation

```mermaid
classDiagram

    class BinaryFile{
        + FileMetaData MetaData
        + byte[] Bytes
    }
    <<Abstract>> BinaryFile

    class TextFile{
        + string Content
    }

    class FileMetaData{
    }

    class BasePage {
    }

    class PageMetaData {
    }

    class StaticContent {
    }

    class SiteMetaData {
        + Dictionary Data
        + BuildData BuildData
        + BasePage[] Items
    }

    class BuildData {
    }

    BinaryFile --> FileMetaData
    TextFile --|> BinaryFile
    SiteMetaData --> BasePage
    SiteMetaData --> BuildData
    PageMetaData --|> BasePage
    StaticContent --|> BasePage

    note for FileMetaData "Extends Dictionary(string, object?) 
    with some convenience properties"
```

## License

This repo (including Blog content) is licensed under the [MIT License](LICENSE)

<!-- https://giscus.app -->
<!-- https://github.com/giscus/giscus/blob/main/ADVANCED-USAGE.md -->

<!--
Brand Color
green: '#4cae50',
purple: '#55557b'
-->