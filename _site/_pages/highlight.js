(function() {
    const blocks = document.querySelectorAll('code[class^="language-"]');
    if (!blocks.length) 
    {
        // No code blocks on the page, no need to continue
        return;
    }

    const PRISM_CORE_ID = "prism-core";
    const PRISM_CSS_ID = "prism-css";
    const PRISM_LANG_PREFIX = "prism-lang-";
    window.__loadedPrismLanguages = window.__loadedPrismLanguages || new Set();

    const loadPrismCore = () => {
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
    };
    
     const loadPrismLanguage = (lang) => {
         if (!lang || window.__loadedPrismLanguages.has(lang)) {
             return;
         }
         window.__loadedPrismLanguages.add(lang);

         const script = document.createElement("script");
         script.id = PRISM_LANG_PREFIX + lang;
         script.defer = true;
         script.src = `https://cdn.jsdelivr.net/npm/prismjs@1/components/prism-${lang}.min.js`;
         // script.onload = () => window.__loadedPrismLanguages.add(lang);
         document.head.appendChild(script);
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const code = entry.target;
                
                const langClass = [...code.classList].find(c => c.startsWith("language-"));
                const lang = langClass?.split("-")[1];
                loadPrismCore();
                if (lang) {
                    loadPrismLanguage(lang);
                }

                observer.unobserve(code);
            }
        });
    }, { threshold: 0.1 });
    
    blocks.forEach(block => observer.observe(block));
})();