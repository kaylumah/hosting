module.exports = {
  content: [
    "**/*.html"
  ],
  theme: {
    extend: {
      fontFamily: {
        architect: "'Architects Daughter', cursive",
        roboto: "'Roboto', sans-serif",
        faduotone: "'Font Awesome 5 Duotone'"
      },
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
    require('@tailwindcss/typography'),
    require('@tailwindcss/line-clamp')
  ]
}
