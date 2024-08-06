---
title: Fixing VSCode Markdown preview with symbolic links!
description: Render images from everywhere inside VSCode's markdown preview
# image: /assets/images/posts/20240209/reqnroll/cover_image.png
tags:
  - vscode
  - markdown
# publishedtime: '21:00'
# commentid: '555' 
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
![Markdown preview from project root](/assets/images/drafts/markdown-preview/001.png){width=2272 height=1760}

## The issue

If you’re like me, your project contains many more files than the few shown in the example. In such cases, I prefer working inside the _posts folder. Unfortunately, as the screenshot below shows, this breaks the image preview functionality.

![Markdown preview from subfolder](/assets/images/drafts/markdown-preview/002.png){width=2272 height=1760}

Instead of displaying my logo, the preview now shows a broken image icon. Technically, this behavior is correct because, relative to our “hello-world.md” post, there is no “assets” directory. You might think that changing the path to “../assets/” would solve the issue, since that’s where the folder exists on disk. However, VSCode does not allow this due to [security concerns](https://github.com/Microsoft/vscode/issues/64685#issuecomment-446414622). Even if it did work, it would create the issue that the preview would no longer function correctly when opened from the root directory.

## The solution

To my knowledge, there is no built-in function in VSCode to address this issue. However, there is an operating system-level solution: using symbolic links.

We can create a symlink by running the following command inside the "_posts" directory

```sh
ln -s ../assets assets
```

From the filesystem perspective the "_posts" folder now has a subfolder called posts. If we now open it inside VSCode it renders the image correctly.

![Markdown preview with symlink](/assets/images/drafts/markdown-preview/003.png){width=2272 height=1760}

## To consider

Personally, I believe its a nice workaround for an issue that irritated me.
Before you leave I like to leave you with some final thoughts.

- This behavior is disabled by default, to prevent opening untrusted content. So don't blindly apply this solution everywhere
- If your blog is under source control via GIT you can [include those symlinks](https://stackoverflow.com/questions/954560/how-does-git-handle-symbolic-links/18791647#18791647) in GIT so your teammates have the same benefits. If you are using Git for Windows you may need [additional steps to support symlinks](https://stackoverflow.com/questions/5917249/git-symbolic-links-in-windows/59761201#59761201)
