module.exports = {
	apps: [
		{
			name: 'stremio-jackett',
			script: 'npm start',
			cwd: '/app',
			watch: ['./dist/index.js'],
			autorestart: true,
			env: {
				...process.env,
			},
		},
	],
};
