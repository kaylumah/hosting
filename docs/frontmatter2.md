---
layout: page
title: Blog
permalink: /blog
---
{% assign posts = site.posts | where_exp: "post", "post.hidden != true" %}
{% for post in posts %}
{%- assign author = site.data.authors[post.author] -%}
{%- assign date_format = site.minima.date_format | default: "%b %-d, %Y" -%}
<a href="{{ post.url | relative_url }}">
{{ author.display_name }} &bull; {{ post.date | date: date_format }}

======================
{%- assign projects = site.data.projects | sort: 'from' | reverse -%}	
	
max:
  display_name: Max
  picture: /assets/avatar_background.svg
  twitter: maxhamulyak
  linkedin: maxhamulyak
  github: maxhamulyak
  medium: kaylumah
  devto: kaylumah
  stackoverflow: 1936600

# max:
#   name: max
#   display_name: max2
#   picture: /img/benbalter.png
#   twitter: jekyllrb

# Authors
# authors:
#   max:
#     name: Max Bla
#     email: max@kaylumah.nl
#     uri: https://kaylumah.nl
#     display_name: Max Hamulyák
#     linkedin_username: maxhamulyak
#     github_username: maxhamulyak
#     twitter_username: maxhamulyak


====================

pages:
    - title: Blog
      url: /blog