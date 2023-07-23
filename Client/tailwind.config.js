/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.razor',
        './Components/**/*.razor',
        './Shared/**/*.razor',
        './App.razor'
    ],
    safelist: [
        'bg-red-600',
        'bg-green-600',
    ],
    theme: {
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
                'fade-in-down': 'fade-in-down 0.5s ease-out',
            }
        },
    },
    plugins: [],
}
