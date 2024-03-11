import * as esbuild from 'esbuild'
import tsconfig from './tsconfig.json'

async function build(): Promise<void> {
    await esbuild.build({
        bundle: true,
        format: 'cjs',
        platform: 'node',
        logLevel: 'silent',
        outfile: 'dist/main.cjs',
        entryPoints: [`src/main.ts`],
        target: [tsconfig.compilerOptions.target],
        external: ['webtorrent'],
        keepNames: true,
    })
}

build()
    .then(() => console.log('Build complete'))
    .catch(() => process.exit(1));
