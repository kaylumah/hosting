---
type: 'Announcement'
title: Specflow has died; long live Reqnroll!
description: SpecFlow rebooted; please welcome Reqnroll!
# image: /assets/images/social_preview.png
tags:
  - csharp
  - testing
  - bdd
  - reqnroll
  - cucumber
  - gherkin
# publishedtime: '21:45'
commentid: '555' 
---
Today's post is a short one. My goal is to spread awareness inside the .NET community that on 2024-02-08 at 14:56:14Z, Specflow was [pronounced dead](https://github.com/SpecFlowOSS/SpecFlow/issues/2719#issuecomment-1934292742). The statement was made by none other than Gáspár Nagy, the original creator of Specflow.

For those who don't know, Specflow is (or I suppose was) the official Cucumber implementation for .NET, which has been open-source all the way since 2009. With Cucumber, you write tests using the Gherkin language. This allows for human readable tests and many more incredible advantages. One of the last things I did at my previous company was give a dev talk on all things Specflow and my experiences using it.

![Dev Days 2023 - ilionx - Specflow Presentation](/assets/images/posts/20240209/ilionx_devdays_2023_specflow.jpeg){width=2560 height=1707}

Somewhere in the past, Specflow was sold, and unfortunately, for the past two years, the project has seen no activity. Gáspár has made the decision [to fork and revive](https://reqnroll.net/news/2024/02/from-specflow-to-reqnroll-why-and-how/) it under the name of Reqnroll(pronounced as [reknroʊl]). He has done a lot of grunt work, and now it is up to him and the community to keep it alive.

A [migration guide](https://docs.reqnroll.net/latest/guides/migrating-from-specflow.html) was published, and it is as simple as changing a package and changing a few namespaces. I think the migration took me about 15 minutes, and it was done. There is even a package published for backwards compatibility, so even fewer changes are required. 

Most of the tests for the blog and static website generator you are reading this article on were written using Specflow (and now rewritten!). I was planning out a bunch of articles on this topic, but with a new job and everything, time got away from me.
I'd like to personally thank Gáspár for the effort to first revive Specflow and now for the process of starting the fork and a brand new community. I hope the efforts will not be in vain and we can make the project successful. I will make it a priority that upcoming content will make use of Reqnroll! Not only because I love this way of writing tests but also to show that Gáspár has my full support moving forward!