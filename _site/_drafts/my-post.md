---
title: "A"
---

The first incarnation of my blog was written using Jekyll, since then I moved to a custom written static website generator. In Jekyll Pages/Posts are separated from the assets directory, I chose to keep the same separation from my code.

For the sake of argument let's say that our directory structure looks like this:

```sh
# generated with the command tree -L 2
.
├── _posts
│   └── hello-world.md
├── assets
│   └── logo.svg
└── index.html
```

> **note**: to ensure no installed extensions can interfere with this demo I am opening VSCode in profile mode (i.e. `code --profile "Blog" .`)

![](/assets/images/drafts/markdown-preview/001.png)

However, in my case there are a lot more files in the root directory. Sometimes I just want to experience just editing the posts.

Let's open the `_posts` directory to see what it looks like.

![](/assets/images/drafts/markdown-preview/002.png)

As you can see my logo is no longer displaying.
You might say, well there is no `assets` directory in the post directory. Change that to `../assets` and it will work.
From a file system perspective this is of course true

This workaround will not work for two reasons
1. VSCode does not allow due to [security concerns](https://github.com/Microsoft/vscode/issues/64685#issuecomment-446414622)
2. You can no longer open it in the root directory

![](/assets/images/drafts/markdown-preview/003.png)


## Resources

- https://jekyllrb.com/docs/posts/#including-images-and-resources
- https://jekyllrb.com/docs/structure/
- https://stackoverflow.com/questions/954560/how-does-git-handle-symbolic-links/18791647#18791647
- https://stackoverflow.com/questions/5917249/git-symbolic-links-in-windows/59761201#59761201
