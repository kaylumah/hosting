# Docs...

The `FileMetaData` class is a `Dictionary<string, object>` so that we don't force the data that is required while allowing the user to specifying custom data.

Some properties are accesible via `{get;set;}` since we preform logic on them.

| Prop | Desc |
| - | - |
| `Layout` | Layout is used to apply markup to a file if present. |
| `Permalink` | Permalink is the pattern to dermine output location. |
| `Uri` | URI is the output location of a file. |
| `Collection` | used for grouping data together |
| `Tags` | another way to group content |
| `Date` | Only for articles publication date |
| `Modified` | Only for articles, modififaction date |

Step #1 DetermineOutputLocation

1. Split filename and potential date.
    1. if date set metadata.date
2. Use Permalink with var substition and return it.

Step #2 Determine
