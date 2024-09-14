document.addEventListener('DOMContentLoaded', function () {
    const maxAcceptableDistance = 5; // Maximum allowable distance for a suggestion
    const maxSuggestions = 3; // Maximum number of suggestions to display
    const pageData = {
        {% for page in site.pages %}
        "/{{ page.uri }}": { "title": "{{ page.title }}" },
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
     *
     * @param {Array<string>} validUrls - The urls to use in calculating the best matches     * @returns {*}
     * @returns {Array<{distance: number,url: string}>}
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
        return urlDistances;
    }
    
    const suggestionsList = document.getElementById('suggestions');
    const validUrls = Object.keys(pageData);
    const suggestions = determineClosestMatch(validUrls);
    console.log('URL scores:', suggestions);

    setTimeout(function() {
        // Clear any placeholder content
        suggestionsList.innerHTML = '';

        // Dynamically create list items with Tailwind styling
        suggestions.forEach(suggestion => {
            const url = suggestion.url;
            const item = pageData[url];
            const listItem = document.createElement('li');
            listItem.classList.add('relative', 'py-6', 'flex', 'items-start', 'space-x-4');

            // Icon container
            const iconContainer = document.createElement('div');
            iconContainer.classList.add('flex-shrink-0');
            iconContainer.innerHTML = `
                <span class="flex items-center justify-center h-12 w-12 rounded-lg bg-indigo-50">
                    <svg class="h-6 w-6 text-indigo-700" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" aria-hidden="true">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 5c7.18 0 13 5.82 13 13M6 11a7 7 0 017 7m-6 0a1 1 0 11-2 0 1 1 0 012 0z"/>
                    </svg>
                </span>
            `;

            // Content container
            const contentContainer = document.createElement('div');
            contentContainer.classList.add('min-w-0', 'flex-1');
            contentContainer.innerHTML = `
                <h3 class="text-base font-medium text-gray-900">
                    <a href="${url}" class="focus:outline-none text-indigo-600 hover:text-indigo-800">
                        ${item.title}
                    </a>
                </h3>
                <p class="text-base text-gray-500">${item.description}</p>
            `;

            // Chevron icon
            const chevronContainer = document.createElement('div');
            chevronContainer.classList.add('flex-shrink-0', 'self-center');
            chevronContainer.innerHTML = `
                <svg class="h-5 w-5 text-gray-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                    <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd"/>
                </svg>
            `;

            // Append icon, content, and chevron to the list item
            listItem.appendChild(iconContainer);
            listItem.appendChild(contentContainer);
            listItem.appendChild(chevronContainer);

            // Append the list item to the suggestions list
            suggestionsList.appendChild(listItem);
        });
    }, 1000);

    /*
    let suggestionsCount = 0;
    for (let i = 0; i < urlDistances.length && suggestionsCount < maxSuggestions; i++) {
        if (urlDistances[i].distance <= maxAcceptableDistance) {

            CreateSuggestion(urlDistances[i].url);
            suggestionsCount++;
        }
    }*/
});