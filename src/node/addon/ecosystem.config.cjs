module.exports = {
    apps: [
        {
            name: "knightcrawler",
            script: "npm start",
            cwd: "/app",
            watch: ["./dist/index.cjs"],
            autorestart: true,
            env: {
                ...process.env
            },
        },
    ],
};