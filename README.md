# SSG

![.NET (CI)](https://github.com/Kaylumah/SSG/workflows/.NET%20(CI)/badge.svg)

## Setup TailwindCSS

[docs](https://tailwindcss.com/docs/installation)

1. npm install tailwindcss@latest postcss@latest autoprefixer@latest
2. npm install postcss-cli
3. npm install @tailwindcss/typography
4. npx tailwindcss init -p
5. ./node_modules/.bin/postcss styles.css -o compiled.css (npm run dev)

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

- https://github.com/tailwindlabs/tailwindcss-typography

- https://medium.com/@mattront/the-complete-guide-to-customizing-a-tailwind-css-theme-ef423eccf863

- https://tjaddison.com/blog/2020/08/updating-to-tailwind-typography-to-style-markdown-posts/

- https://github.com/tailwindlabs/tailwindcss.com/blob/master/tailwind.config.js

- https://github.com/tailwindlabs/tailwindcss.com/blob/bb5eacf1778bb377b9bb190bccc2cac494cdfb56/src/css/utilities.css#L101

- https://github.com/tailwindlabs/tailwindcss.com/blob/bb5eacf1778bb377b9bb190bccc2cac494cdfb56/src/components/Heading.js

