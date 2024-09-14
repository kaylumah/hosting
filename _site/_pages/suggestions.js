document.addEventListener('DOMContentLoaded', function () {
    const suggestionsContainer = document.getElementById('suggestions');
    const maxAcceptableDistance = 5; // Maximum allowable distance for a suggestion
    const maxSuggestions = 3; // Maximum number of suggestions to display
    const pageData = {
        {% for page in site.pages %}
        "/{{ page.uri }}": {},
        {% endfor %}
    };

    // Get the current URL path from the browser
    const requestedUrl = window.location.pathname;
    console.log(`Requested URL: '${requestedUrl}' maxAcceptableDistance: '${maxAcceptableDistance}' maxSuggestions '${maxSuggestions}'`);

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
     * Process URLs
     * @param {Array<string>} validUrls - The urls to use in calculating the best matches
     */
    function determineClosestMatch(validUrls)
    {
        if (!validUrls.length) {
            console.error('No valid URLs found.');
            return;
        }

        // Calculate the Levenshtein distances
        const urlDistances = validUrls.map(validUrl => ({
            url: validUrl,
            distance: calculateLevenshteinDistance(requestedUrl, validUrl)
        }));

        // Sort the URLs by their computed Levenshtein distance
        urlDistances.sort((a, b) => a.distance - b.distance);

        console.log('URL scores:', urlDistances);

        let suggestionsCount = 0;
        for (let i = 0; i < urlDistances.length && suggestionsCount < maxSuggestions; i++) {
            if (urlDistances[i].distance <= maxAcceptableDistance) {

                CreateSuggestion(urlDistances[i].url);
                suggestionsCount++;
            }
        }
    }

    /**
     * Add suggestion to the markup
     * @param {string} url
     */
    function CreateSuggestion(url)
    {
        const suggestionsElement = document.getElementById('suggestions');
        const suggestionItem = document.createElement('li');
        suggestionItem.innerHTML = `<a href="${url}">${url}</a>`;
        suggestionsElement.appendChild(suggestionItem);
    }

    const validUrls = Object.keys(pageData);
    determineClosestMatch(validUrls);
});