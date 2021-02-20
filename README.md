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