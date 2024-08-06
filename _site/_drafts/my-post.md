---
title: "A"
---

Many static website generators support writing blogs in Markdown. Jekyll and several other generators organize content, such as blog posts, and assets, like images and CSS files, into separate directories.

My favorite editor for writing Markdown is VSCode. While this separation is useful for organization, it can be somewhat cumbersome when editing and previewing Markdown.

To illustrate this point, let’s look at an example. A typical directory structure looks like this:

```sh
# generated with the command tree -L 2
.
├── _posts
│   └── hello-world.md
├── assets
│   └── logo.svg
└── index.html
```

Using the Markdown preview feature of VSCode that would look like this:
<!-- code --profile "Blog" . -->
![Markdown preview from project root](/assets/images/drafts/markdown-preview/001.png)

If you’re like me, your project contains many more files than the few shown in the example. In such cases, I prefer working inside the _posts folder. Unfortunately, as the screenshot below shows, this breaks the image preview functionality.

![Markdown preview from subfolder](/assets/images/drafts/markdown-preview/002.png)

As you can see my logo is no longer displaying.
You might say, well there is no `assets` directory in the post directory. Change that to `../assets` and it will work.
From a file system perspective this is of course true

This workaround will not work for two reasons
1. VSCode does not allow due to [security concerns](https://github.com/Microsoft/vscode/issues/64685#issuecomment-446414622)
2. You can no longer open it in the root directory

![](/assets/images/drafts/markdown-preview/003.png)


ln -s ../assets assets 

## Resources

- https://jekyllrb.com/docs/posts/#including-images-and-resources
- https://jekyllrb.com/docs/structure/
- https://stackoverflow.com/questions/954560/how-does-git-handle-symbolic-links/18791647#18791647
- https://stackoverflow.com/questions/5917249/git-symbolic-links-in-windows/59761201#59761201
