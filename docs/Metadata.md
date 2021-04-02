# Metadata

We have two editor versions. If you'd prefer to edit title and tags etc. as separate fields, switch to the "rich + markdown" option in Settings » UX. Otherwise, continue...

This editor is a markdown editor that uses Jekyll front matter.
Most of the time, you can write inline HTML directly into your posts.
We support native Liquid tags and created some fun custom ones, too! Trying embedding a Tweet or GitHub issue in your post.
Links to unpublished posts are shareable for feedback/review.
When you're ready to publish, set the published variable to true.
Front Matter
Custom variables set for each post, located between the triple-dashed lines in your editor. Here is a list of possibilities:

title: the title of your article
published: boolean that determines whether or not your article is published
description: description area in Twitter cards and open graph cards
tags: max of four tags, needs to be comma-separated
canonical_url: link for the canonical version of the content
cover_image: cover image for post, accepts a URL.
The best size is 1000 x 420.
series: post series name.

https://jekyllrb.com/docs/front-matter/

https://dev.to/p/publishing_from_rss_guide
- <title>: title
- <category>: tags, comma separated.
- <link>: canonical_url
The article content is prioritized in this order: <content>, <summary>, or <description>

---

https://tools.ietf.org/html/rfc4287
https://validator.w3.org/feed/docs/atom.html









https://developer.mozilla.org/en-US/docs/Web/HTML/Element/title
https://developer.mozilla.org/en-US/docs/Web/HTML/Element/meta
https://www.w3schools.com/tags/tag_meta.asp
https://www.w3schools.com/tags/tag_head.asp
https://www.w3schools.com/html/html_head.asp


















https://ogp.me/
- og:title - The title of your object as it should appear within the graph, e.g., "The Rock".
- og:type - The type of your object, e.g., "video.movie". Depending on the type you specify, other properties may also be required.
- og:image - An image URL which should represent your object within the graph.
- og:url - The canonical URL of your object that will be used as its permanent ID in the graph, e.g., "https://www.imdb.com/title/tt0117500/".

- og:audio - A URL to an audio file to accompany this object.
- og:determiner - The word that appears before this object's title in a sentence. An enum of (a, an, the, "", auto). If auto is chosen, the consumer of your data should chose between "a" or "an". Default is "" (blank).
- og:locale - The locale these tags are marked up in. Of the format language_TERRITORY. Default is en_US.
- og:locale:alternate - An array of other locales this page is available in.
- og:site_name - If your object is part of a larger web site, the name which should be displayed for the overall site. e.g., "IMDb".
- og:video - A URL to a video file that complements this object.
- og:description - A one to two sentence description of your object.

- og:image:url - Identical to og:image.
- og:image:secure_url - An alternate url to use if the webpage requires HTTPS.
- og:image:type - A MIME type for this image.
- og:image:width - The number of pixels wide.
- og:image:height - The number of pixels high.
- og:image:alt - A description of what is in the image (not a caption). If the page specifies an og:image it should specify og:image:alt.


<meta property="og:type" content="website" />

og:type values:

article - Namespace URI: https://ogp.me/ns/article#

article:published_time - datetime - When the article was first published.
article:modified_time - datetime - When the article was last changed.
article:expiration_time - datetime - When the article is out of date after.
article:author - profile array - Writers of the article.
article:section - string - A high-level section name. E.g. Technology
article:tag - string array - Tag words associated with this article.

