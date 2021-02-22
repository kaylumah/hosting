# SSG

![.NET (CI)](https://github.com/Kaylumah/SSG/workflows/.NET%20(CI)/badge.svg)

## Setup TailwindCSS

[docs](https://tailwindcss.com/docs/installation)

1. npm install tailwindcss@latest postcss@latest autoprefixer@latest
2. npm install postcss-cli
3. npm install @tailwindcss/typography
4. npx tailwindcss init -p
5. ./node_modules/.bin/postcss styles.css -o compiled.css (npm run dev)

npx tailwindcss init tailwind-full.config.js --full
posscss 
tailwindcss: {
    config: './tailwind-full.config.js'
}

const colors = require('tailwindcss/colors')


Create css file

```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

## Design System

### Header

* Background-color is `bg-gray-900`
* Text-color is `text-gray-100`
* Anchor hover is `text-gray-400`

### Footer

* Background-color is `bg-gray-900`
* Text-color is `text-gray-100`
* Anchor hover is `text-gray-400`

## MSBuild Extensions

https://www.hanselman.com/blog/adding-a-git-commit-hash-and-azure-devops-build-number-and-build-id-to-an-aspnet-website 

* `SourceRevisionId`
* `BuildNumber` or `BuildId`

## Reading list

https://tailwindcss.com/docs/optimizing-for-production#enabling-manually

https://refactoringui.com/book/

https://v1.tailwindcss.com/resources

http://www.zondicons.com/

https://simpleicons.org/

https://heroicons.com/

https://github.com/tailwindlabs/blog.tailwindcss.com


Taftse02/16/2021
The Logos come from https://simpleicons.org/ as far as I know

I think they only used simple Icons for the brand logos because they are not part of heroicons

They could also be part of http://www.zondicons.com/ which is also made by Steve as far as I know





- https://github.com/tailwindlabs/tailwindcss-typography

- https://medium.com/@mattront/the-complete-guide-to-customizing-a-tailwind-css-theme-ef423eccf863

- https://tjaddison.com/blog/2020/08/updating-to-tailwind-typography-to-style-markdown-posts/

- https://github.com/tailwindlabs/tailwindcss.com/blob/master/tailwind.config.js

- https://github.com/tailwindlabs/tailwindcss.com/blob/bb5eacf1778bb377b9bb190bccc2cac494cdfb56/src/css/utilities.css#L101

- https://github.com/tailwindlabs/tailwindcss.com/blob/bb5eacf1778bb377b9bb190bccc2cac494cdfb56/src/components/Heading.js


## Docs

- Code coverage
https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=linux

dotnet test --collect:"XPlat Code Coverage"
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

dotnet reportgenerator "-reports:test/Unit/TestResults/2a416370-74ff-4c37-9418-b31e88d736a0/coverage.cobertura.xml" "-targetdir:coveragereport" -reporttypes:Html
