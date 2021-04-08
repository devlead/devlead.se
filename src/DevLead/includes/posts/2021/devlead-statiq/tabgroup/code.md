## Code

Code content lets you have fetch content from an external document into a markdown code fence. This is really useful as it makes the TabGroup more readable, but it also great for code reuse and i.e. keeping documentation in sync actual code used.

Path to code file is specified using tab `code` property, either absolute path or relative to document, by leading with dot slash (`./`) path will be relative to input root, which simplifies reasoning where document is located, regardless of where the document is included from.

If no `name` property specified the name of the document including extension (*with preserved casing*) will be used as tab name.

Code language used for code fence will be automatically inferred from file extension but you can override it using the `codeLang` property.

### Example usage

<?# IncludeCode "./../includes/posts/2021/devlead-statiq/tabgroup/src/code.md" lang="yaml" /?>

### Result

<?# Include "./../includes/posts/2021/devlead-statiq/tabgroup/src/code.md" /?>
