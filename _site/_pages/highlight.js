(async function () {
    const blocks = document.querySelectorAll('code[class^="language-"]');
    if (!blocks.length) {
        console.warn("[Prism] No code blocks found.");
        return;
    }

    const loadCSS = () =>
        new Promise((res) => {
            if (document.getElementById('prism-css')) {
                console.log("[Prism] CSS already loaded.");
                return res();
            }
            const link = document.createElement('link');
            link.id = 'prism-css';
            link.rel = 'stylesheet';
            link.href = 'https://cdn.jsdelivr.net/npm/prismjs@1/themes/prism.css';
            link.onload = () => {
                console.log("[Prism] CSS loaded.");
                res();
            };
            link.onerror = () => {
                console.error("[Prism] Failed to load CSS.");
                res();
            };
            document.head.appendChild(link);
        });

    const loadCore = () =>
        new Promise((res) => {
            if (document.getElementById('prism-core')) {
                console.log("[Prism] Core JS already loaded.");
                return res();
            }
            const script = document.createElement('script');
            script.id = 'prism-core';
            script.defer = true;
            script.src = 'https://cdn.jsdelivr.net/npm/prismjs@1/prism.min.js';
            script.onload = () => {
                console.log("[Prism] Core loaded.");
                res();
            };
            script.onerror = () => {
                console.error("[Prism] Failed to load core JS.");
                res();
            };
            document.head.appendChild(script);
        });

    const loadLang = (lang) => {
        return new Promise((res) => {
            if (!lang) {
                console.warn("[Prism] Skipping empty language.");
                return res();
            }

            if (window.__loadedPrismLanguages.has(lang)) {
                console.log(`[Prism] '${lang}' already available.`);
                return res();
            }

            const script = document.createElement('script');
            script.src = `https://cdn.jsdelivr.net/npm/prismjs@1/components/prism-${lang}.min.js`;
            script.defer = true;
            script.onload = () => {
                console.log(`[Prism] Language '${lang}' loaded.`);
                window.__loadedPrismLanguages.add(lang);
                res();
            };
            script.onerror = () => {
                console.error(`[Prism] Failed to load language: ${lang}`);
                res();
            };
            document.head.appendChild(script);
        });
    };

    await loadCSS();
    await loadCore();

    if (!window.Prism || !Prism.languages) {
        console.error("[Prism] Core did not initialize correctly.");
        return;
    }

    window.__loadedPrismLanguages = new Set(Object.keys(Prism.languages));

    const LANG_ALIASES = {
        shell: 'bash',
        sh: 'bash'
    };
    
    for (const block of blocks) {
        const langClass = [...block.classList].find((cls) => cls.startsWith('language-'));
        const rawLang = langClass?.split('-')[1] || 'markup';

        const canonical = LANG_ALIASES[rawLang] || rawLang;
        if (canonical !== rawLang) {
            console.warn(`[Prism] Aliased '${rawLang}' → '${canonical}'`);
            block.classList.remove(`language-${rawLang}`);
            block.classList.add(`language-${canonical}`);
        }

        const lang = canonical;
        await loadLang(lang);
        try {
            Prism.highlightElement(block);
            console.log(`[Prism] Highlighted block with language: ${lang}`);
        } catch (err) {
            console.error(`[Prism] Failed to highlight: ${lang}`, err);
        }
    }
})();