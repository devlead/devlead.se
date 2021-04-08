## Include

Include content lets you fetch markdown from an external document. This is really useful as it makes the TabGroup more readable, but it also great for code reuse and i.e. if you want the same content on multiple tabs.

Path to the markdown file is specified using tab `include` property, either an absolute path or relative to the document,, by leading with dot slash (`./`) path will be relative to input root, which simplifies reasoning where document is located, regardless of where the document is included from.

If no `name` property specified, by convention the tab name will be the name of the external document **without** extension, title cased, and underscores will be replaced by a space.

### Example usage

<?# IncludeCode "./../includes/posts/2021/devlead-statiq/tabgroup/src/include.md" lang="yaml" /?>

### Result

<?# Include "./../includes/posts/2021/devlead-statiq/tabgroup/src/include.md" /?>
