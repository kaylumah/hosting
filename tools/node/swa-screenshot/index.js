const puppeteer = require('puppeteer');
const fs = require('fs');
const path = require('path');

(async () => {
    const browser = await puppeteer.launch();
    const page = await browser.newPage();

    // Define breakpoints (including ultra-wide screens)
    const breakpoints = [
        320,    // Mobile (small)
        768,    // Tablet
        1024,   // Small Laptop
        1280,   // Standard Laptop
        1440,   // Large Laptop
        1920,   // Standard Desktop
        2560,   // QHD / Ultra-Wide Monitor
        3200,   // 3K Monitor
        3840    // 4K Display
    ];

    // Create a timestamped folder
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-'); // Format safe for filenames
    const folderPath = path.join(__dirname, `screenshots-${timestamp}`);
    fs.mkdirSync(folderPath, { recursive: true });

    console.log(`Saving screenshots in: ${folderPath}`);

    for (let width of breakpoints) {
        await page.setViewport({ width, height: 1080 });
        await page.goto('http://localhost:4280'); // Change to your local URL
        const screenshotPath = path.join(folderPath, `screenshot-${width}px.png`);
        await page.screenshot({ path: screenshotPath, fullPage: true });
        console.log(`Captured screenshot at ${width}px → ${screenshotPath}`);
    }

    await browser.close();
})();