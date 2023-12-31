/** @type {import('tailwindcss').Config}*/
const config = {
  content: ["./src/**/*.{html,svelte}"],

  theme: {
    screens: {
      sm: "440px",
      md: "560px",
      lg: "660px",
    },
    extend: {
      keyframes: {
        "fade-in-down": {
          "0%": {
            opacity: "0",
            translate: "0 -100%",
          },
          "100%": {
            opacity: "1",
            translate: "0",
          },
        },
      },
      animation: {
        "fade-in-down": "fade-in-down 0.5s ease-out",
      },
      gridTemplateColumns: {
        lg: "1fr 1fr 3fr repeat(4,1fr)",
        md: "1fr 1fr 3fr repeat(3,1fr)",
        sm: "1fr 1fr 3fr repeat(2,1fr)",
        base: "1fr 1fr 3fr repeat(1,1fr)",
      },
    },
  },

  plugins: [],
};

module.exports = config;
