const puppeteer = require('puppeteer');

(async () => {
    const urls = [];
    const browser = await puppeteer.launch({ args: ['--no-sandbox', '--disable-setuid-sandbox'] });
    for (var i = 0; i < urls.length; i++)
    {
        var url = urls[i];
        const page = await browser.newPage();
        await page.goto(url);
        await page.screenshot({ path: `screenshot-${i}.png` });
    }

    await browser.close();
})();