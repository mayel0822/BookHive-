/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Views/**/*.cshtml',
    './Pages/**/*.cshtml',
  ],
  theme: {
    extend: {
      colors: {
        navy:    '#1B2B5E',
        byellow: '#F5C518',
      }
    }
  },
  plugins: [],
}