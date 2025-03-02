module.exports = {
  content: [
    "**/*.html"
  ],
  theme: {
    screens: {
      'tablet': '640px',
      // => @media (min-width: 640px) { ... }

      'laptop': '1024px',
      // => @media (min-width: 1024px) { ... }

      'desktop': '1280px',
      // => @media (min-width: 1280px) { ... }
    },
    extend: {
      colors: {
        brand: {
          green: '#4cae50',
          purple: '#55557b'
        }
      },
      typography: {
        DEFAULT: {
          css: {
            a: {
              textDecoration: 'none'
            }
          },
        },
      }
    },
  },
  plugins: [
    require('@tailwindcss/typography')
  ]
}
