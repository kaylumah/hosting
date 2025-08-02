(function() {
    console.log("loaded highlight.js");

    const blocks = document.querySelectorAll('code[class^="language-"]');
    if (!blocks.length) 
    {
        return;
    }

    console.log("code-blocks", blocks);
    /*
    * const PRISM_CORE_ID = "prism-core";
    const PRISM_CSS_ID = "prism-css";
    const PRISM_LANG_PREFIX = "prism-lang-";

    // Track loaded languages
    window.__loadedPrismLanguages = window.__loadedPrismLanguages || new Set();
    if (window.__prismAlreadyLoaded) return;
    * */
    
    
    /*
    * 
    * const loadPrismCore = () => {
        if (!document.getElementById(PRISM_CSS_ID)) {
            const link = document.createElement("link");
            link.id = PRISM_CSS_ID;
            link.rel = "stylesheet";
            link.href = "https://cdn.jsdelivr.net/npm/prismjs@1/themes/prism.css";
            document.head.appendChild(link);
        }

        if (!document.getElementById(PRISM_CORE_ID)) {
            const core = document.createElement("script");
            core.id = PRISM_CORE_ID;
            core.defer = true;
            core.src = "https://cdn.jsdelivr.net/npm/prismjs@1/prism.min.js";
            document.head.appendChild(core);
        }

        window.__prismAlreadyLoaded = true;
    };
    * */
    
    /*
    * const loadPrismLanguage = (lang) => {
        if (!lang || window.__loadedPrismLanguages.has(lang)) return;

        const script = document.createElement("script");
        script.id = PRISM_LANG_PREFIX + lang;
        script.defer = true;
        script.src = `https://cdn.jsdelivr.net/npm/prismjs@1/components/prism-${lang}.min.js`;
        script.onload = () => window.__loadedPrismLanguages.add(lang);
        document.head.appendChild(script);
    };
    * */

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