(function() {

    let commentId = window.COMMENT_ID || null;
    if (!commentId || typeof commentId !== "string") {
        return;
    }
    
    let commentSection = document.getElementById("comment-section");
    if (commentSection) {
        let observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    let script = document.createElement("script");
                    script.src = "https://giscus.app/client.js";
                    script.async = true;
                    script.setAttribute("data-repo", "kaylumah/hosting");
                    script.setAttribute("data-repo-id", "MDEwOlJlcG9zaXRvcnkzMzgyNzg4MzU=");
                    script.setAttribute("data-mapping", "number");
                    script.setAttribute("data-term", commentId);
                    script.setAttribute("data-reactions-enabled", "1");
                    script.setAttribute("data-emit-metadata", "0");
                    script.setAttribute("data-input-position", "top");
                    script.setAttribute("data-theme", "light_high_contrast");
                    script.setAttribute("data-lang", "en");
                    script.setAttribute("data-loading", "lazy");
                    script.setAttribute("crossorigin", "anonymous");
                    commentSection.appendChild(script);
                    observer.unobserve(commentSection);
                }
            });
        }, {threshold: 0.1});
        observer.observe(commentSection);
    }
})();