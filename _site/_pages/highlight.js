(function() {
    console.log("loaded highlight.js");

    const blocks = document.querySelectorAll('code[class^="language-"]');
    if (!blocks.length) 
    {
        return;
    }

    console.log("code-blocks", blocks);
})();