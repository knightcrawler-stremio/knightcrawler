module.exports = {
    apps: [
        {
            name: "torrentio-selfhostio",
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