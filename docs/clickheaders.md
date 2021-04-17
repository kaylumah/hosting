
<!-- <ul>
    {% for tag in site.tags %}
    <li>
        <a href="/tags?tag={{ tag.key | html.url_encode }}">{{ tag.key }} ({{ tag.value | array.size }})</a>
    </li>
    {% endfor %}
</ul> -->

<!-- <script>
    const urlParams = new URLSearchParams(window.location.search);
    const selectedTag = urlParams.get('tag');
    var tag = decodeURI(selectedTag);

    var sections = document.getElementsByTagName("section");
    for (var i = 0; i < sections.length; i++)
    {
        var section = sections[i];
        var id = section.id;
        if (id.startsWith("tag-"))
        {
            if (id === `tag-${tag}`)
            {
                section.style.display = 'block';
            }
            else
            {
                section.style.display = 'none';
            }
        }
    }

</script> -->