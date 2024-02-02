/** @type {import("eslint").ESLint.Options} */
module.exports = {
    env: {
        es2024: true,
        node: true,
    },
    globals: {
        Atomics: "readonly",
        SharedArrayBuffer: "readonly",
    },
    parserOptions: {
        sourceType: "module",
    },
    plugins: ["import", "import-helpers"],
    rules: {
        "default-case": "off",
        "import/no-duplicates": "off",
        "import/no-extraneous-dependencies": ["off", { devDependencies: ["backend", "frontend", "mobile"] }],
        "import/order": "off",
        "import-helpers/order-imports": [
            "warn",
            {
                alphabetize: {
                    order: "asc",
                },
            },
        ],
        "lines-between-class-members": ["error", "always", { exceptAfterSingleLine: true }],
        "no-continue": "off",
        "no-param-reassign": "off",
        "no-plusplus": ["error", { allowForLoopAfterthoughts: true }],
        "no-restricted-syntax": "off",
        "no-unused-expressions": ["off", { allowShortCircuit: true }],
        "no-unused-vars": "off",
        "no-use-before-define": "off",
        "one-var": ["error", { uninitialized: "consecutive" }],
        "prefer-destructuring": "warn",
    },
};