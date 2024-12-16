import { app, HttpRequest, HttpResponseInit, InvocationContext } from "@azure/functions";

interface RedirectOption {
    pattern: string;
    rewrite: string;
    enabled: boolean;
    permanent: boolean;
}

const redirectOptions: RedirectOption[] = [
    {
        pattern: "^/2023/04/14/csharp-client-for-openapi-revistted.html$",
        rewrite: "/2023/04/14/csharp-client-for-openapi-revisited.html",
        enabled: true,
        permanent: true,
    },
    {
        pattern: "^\\/(?<year>\\d{4})\\/(?<month>\\d{2})\\/(?<day>\\d{2})\\/(?<rest>[\\w-]*?)(?<ext>\\.\\w+)?$",
        rewrite: "/articles/${year}/${month}/${day}/${rest}.html",
        enabled: true,
        permanent: true,
    },
];

export async function httpTrigger1(request: HttpRequest, context: InvocationContext): Promise<HttpResponseInit> {
    const originalUrl = request.headers.get("x-ms-original-url");
    
    if (!originalUrl) {
        console.log("No x-ms-original-url header found. Redirecting to 404.");
        return {
            status: 302,
            headers: { Location: "/404.html" },
        };
    }

    const url = new URL(originalUrl);
    const path = url.pathname;
    console.log("Original Url", originalUrl, path);

    // Find matching redirect option
    const matchedOption = redirectOptions.find((option) => option.enabled && new RegExp(option.pattern).test(path));

    if (!matchedOption) {
        console.log(`No redirect rule matched. Redirecting to 404 with originalUrl=${path}`);
        return {
            status: 302,
            headers: { Location: `/404.html?originalUrl=${encodeURIComponent(path)}` },
        };
    } else {
        const regex = new RegExp(matchedOption.pattern);
        
        const match = regex.exec(path);
        console.log("match", match);

        let newPath = matchedOption.rewrite;
        if (match && match.groups) {
            // Replace named capture groups dynamically
            newPath = Object.keys(match.groups).reduce((result, groupName) => {
                let newValue = match.groups![groupName];
                console.log(`${result} ${groupName} ${newValue}`);
                return result.replace(`\${${groupName}}`, newValue);
            }, matchedOption.rewrite);
        }

        console.log(`Redirecting to: ${newPath}`);
        return {
            status: matchedOption.permanent ? 301 : 302,
            headers: { Location: newPath },
        };
    }
};

app.http('fallback', {
    methods: ['GET', 'POST'],
    authLevel: 'anonymous',
    handler: httpTrigger1
});
