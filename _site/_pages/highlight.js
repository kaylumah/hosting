(function() {
    console.log("loaded highlight.js");

    const blocks = document.querySelectorAll('code[class^="language-"]');
    if (!blocks.length) 
    {
        return;
    }

    console.log("code-blocks", blocks);
    
    let observer = new IntersectionObserver((entries) => {
        console.log("entries", entries);
    }, {threshold: 0.1});

    blocks.forEach(block => observer.observe(block));

})();