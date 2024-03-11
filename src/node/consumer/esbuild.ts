import * as esbuild from 'esbuild'
import tsconfig from './tsconfig.json'

async function build(): Promise<void> {
    await esbuild.build({
        bundle: true,
        format: 'cjs',
        platform: 'node',
        // minify: true,
        logLevel: 'silent',
        outdir: tsconfig.compilerOptions.outDir,
        entryPoints: [`src/main.ts`],
        target: [tsconfig.compilerOptions.target],
        external: ['webtorrent'],
        keepNames: true,
    })
}
build()
