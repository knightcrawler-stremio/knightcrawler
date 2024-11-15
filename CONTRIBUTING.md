We use [Meaningful commit messages](https://reflectoring.io/meaningful-commit-messages/)

Tl;dr:
1. It should answer the question: “What happens if the changes are applied?".
2. Use the imperative, present tense. It is easier to read and scan quickly:
```
Right: Add feature to alert admin for new user registration
Wrong: Added feature ... (past tense)
```
3. The summary should always be able to complete the following sentence:
`If applied, this commit will… `

We use [git-cliff] for our changelog.

The breaking flag is set to true when the commit has an exclamation mark after the commit type and scope, e.g.:
`feat(scope)!: this is a breaking change`

Keywords (Commit messages should start with these):
```
# Added
add
support
# Removed
remove
delete
# Fixed
test
fix
```

Any other commits will fall under the `Changed` category


This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html)