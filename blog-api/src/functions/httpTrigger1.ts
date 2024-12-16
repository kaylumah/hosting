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
        enabled: false,
        permanent: true,
    },
];

export async function httpTrigger1(request: HttpRequest, context: InvocationContext): Promise<HttpResponseInit> {
    // Extract original URL from header
    const originalUrl = request.headers["x-ms-original-url"];
    if (!originalUrl) {
        console.log("No x-ms-original-url header found. Redirecting to 404.");
        return {
            status: 302,
            headers: { Location: "/404.html" },
        };
    }

    console.log(`Original Url: ${originalUrl}`);
    const url = new URL(originalUrl);
    const path = url.pathname;

    // Find matching redirect option
    const matchedOption = redirectOptions.find((option) => option.enabled && new RegExp(option.pattern).test(path));

    if (matchedOption) {
        const newPath = path.replace(new RegExp(matchedOption.pattern), matchedOption.rewrite);
        console.log(`Redirecting to: ${newPath}`);

        return {
            status: matchedOption.permanent ? 301 : 302,
            headers: { Location: newPath },
        };
    } else {
        console.log(`No redirect rule matched. Redirecting to 404 with originalUrl=${path}`);
        return {
            status: 302,
            headers: { Location: `/404.html?originalUrl=${encodeURIComponent(path)}` },
        };
    }
    /*
    context.log(`Http function processed request for url "${request.url}"`);

    const originalUrl = request.headers.get("x-ms-original-url");

    if (originalUrl) {
        context.log(`Original URL: ${originalUrl}`);
    }
    else {
        console.log("failed", request);
    }

    const name = request.query.get('name') || await request.text() || 'world';

    // https://stackoverflow.com/questions/42931965/azure-functions-redirect-header
    // return { body: `Hello, ${name}!` };
    return { status: 302, headers: { Location: 'https://www.google.com' } };
    */
};

app.http('httpTrigger1', {
    methods: ['GET', 'POST'],
    authLevel: 'anonymous',
    handler: httpTrigger1
});
