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
      // fontFamily: {
      //   headline: "Poppins, sans-serif" // font-headline
      // }
    },
  },
  variants: {
    extend: {},
  },
  plugins: [
    require('@tailwindcss/typography')
  ]
}
