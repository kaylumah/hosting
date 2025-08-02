(function() {
    console.log("loaded highlight.js");

    const blocks = document.querySelectorAll('code[class^="language-"]');
    if (!blocks.length) 
    {
        return;
    }

    console.log("code-blocks", blocks);

    const observer = new IntersectionObserver((entries, obs) => {
        console.log("entries", entries);
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                console.log("intersecting", entry);
                // const code = entry.target;
                // const langClass = [...code.classList].find(c => c.startsWith("language-"));
                // const lang = langClass?.split("-")[1];

                // loadPrismCore();
                // if (lang) loadPrismLanguage(lang);

                obs.unobserve(code);
            } else {
                console.log("skipping", entry);
            }
        });
    }, { threshold: 0.1 });

    blocks.forEach(block => observer.observe(block));

})();