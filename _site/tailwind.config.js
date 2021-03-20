module.exports = {
  purge: [
    "**/*.html"
  ],
  darkMode: false, // or 'media' or 'class'
  theme: {
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
            },
            'code::before': {
              content: '""',
            },
            'code::after': {
              content: '""',
            }
          },
        },
      }
      // fontFamily: {
      //   headline: "Poppins, sans-serif" // font-headline
      // }
    },
  },
  variants: {
    extend: {},
  },
  plugins: [
    // https://github.com/tailwindlabs/tailwindcss-typography#responsive-variants
    // https://github.com/tailwindlabs/tailwindcss-typography/blob/master/src/styles.js
    require('@tailwindcss/typography')
  ]
}
