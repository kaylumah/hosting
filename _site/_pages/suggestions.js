document.addEventListener('DOMContentLoaded', function () {
    const maxAcceptableDistance = 3;
    const validUrls = [
    {% for page in site.pages %}
        "/{{ page.uri }}",
    {% endfor %}
    ];

    // Get the current URL path from the browser
    const requestedUrl = window.location.pathname;
    console.log(`Requested URL: '${requestedUrl}' maxAcceptableDistance: '${maxAcceptableDistance}'`);

    /**
     * Calculates the Levenshtein distance between two strings.
     * @param {string} source - The source string (the requested URL).
     * @param {string} target - The target string (a valid URL).
     * @returns {number} - The Levenshtein distance between the source and target strings.
     */
    function calculateLevenshteinDistance(source, target) {
        const distanceMatrix = [];

        for (let row = 0; row <= target.length; row++) {
            distanceMatrix[row] = [row];
        }
        for (let col = 0; col <= source.length; col++) {
            distanceMatrix[0][col] = col;
        }

        for (let row = 1; row <= target.length; row++) {
            for (let col = 1; col <= source.length; col++) {
                const cost = source.charAt(col - 1) === target.charAt(row - 1) ? 0 : 1;

                distanceMatrix[row][col] = Math.min(
                    distanceMatrix[row - 1][col] + 1,     // deletion
                    distanceMatrix[row][col - 1] + 1,     // insertion
                    distanceMatrix[row - 1][col - 1] + cost // substitution
                );
            }
        }

        return distanceMatrix[target.length][source.length];
    }

    /**
     *
     * @param {Array<string>} validUrls - The urls to use in calculating the best matches     * @returns {*}
     * @returns {Array<{distance: number,url: string}>}
     */
    function determineClosestMatch(url, validUrls) {
        
        if (!validUrls.length) {
            console.error('No valid URLs found.');
            return;
        }

        let endsWith = url.endsWith('.html');
        if (endsWith == false) {
            url = url + ".html";
        }

        // Calculate the Levenshtein distances
        const urlDistances = validUrls.map(validUrl => ({
            url: validUrl,
            distance: calculateLevenshteinDistance(url, validUrl)
        }));

        // Sort the URLs by their computed Levenshtein distance
        urlDistances.sort((a, b) => a.distance - b.distance);
        return urlDistances;
    }

    const urlDistances = determineClosestMatch(requestedUrl, validUrls);
    console.log('URL scores:', urlDistances);
    
    const tried = document.getElementById("tried");
    tried.innerHTML = requestedUrl;
    
    const nextAction = document.getElementById("nextAction");
        
    const suggestion = urlDistances[0];
    if (suggestion.distance <= maxAcceptableDistance) {
        nextAction.innerHTML = "";

        const textNode = document.createTextNode("We think you may have been looking for: ");
        nextAction.appendChild(textNode);
        
        const aTag = document.createElement('a');
        aTag.setAttribute('href', suggestion.url);
        aTag.innerText = suggestion.url;
        aTag.classList = "text-blue-500 underline";
        
        nextAction.appendChild(aTag);
    }
});