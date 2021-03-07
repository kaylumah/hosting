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