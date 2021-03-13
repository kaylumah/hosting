const puppeteer = require('puppeteer');

(async () => {
    const urls = [
        'https://www.nuget.org'
    ];
    const browser = await puppeteer.launch({ args: ['--no-sandbox', '--disable-setuid-sandbox'] });
    for (var i = 0; i < urls.length; i++)
    {
        var url = urls[i];
        const page = await browser.newPage();
        await page.goto(url);

        // // https://stackoverflow.com/questions/59618456/pupeteer-how-can-i-accept-cookie-consent-prompts-automatically-for-any-url
        // await page.evaluate(_ => {
        //     function xcc_contains(selector, text) {
        //         var elements = document.querySelectorAll(selector);
        //         return Array.prototype.filter.call(elements, function (element) {
        //             return RegExp(text, "i").test(element.textContent.trim());
        //         });
        //     }
        //     var _xcc;
        //     _xcc = xcc_contains('[id*=cookie] a, [class*=cookie] a, [id*=cookie] button, [class*=cookie] button', '^(Alle akzeptieren|Akzeptieren|Verstanden|Zustimmen|Okay|OK|Accept all)$');
        //     if (_xcc != null && _xcc.length != 0) { _xcc[0].click(); }
        // });

        await page.screenshot({ path: `screenshot-${i}.png` });
    }

    await browser.close();
})();