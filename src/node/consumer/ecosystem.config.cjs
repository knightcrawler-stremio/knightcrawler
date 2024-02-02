module.exports = {
    apps: [
        {
            name: "consumer",
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