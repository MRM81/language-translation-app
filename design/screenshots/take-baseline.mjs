import { chromium } from 'playwright';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const outDir = path.join(__dirname, 'sprint-008-baseline');
const URL = 'http://localhost:5174';

const viewports = [
  { name: '1440', width: 1440, height: 900 },
  { name: '1024', width: 1024, height: 768 },
  { name: '768',  width: 768,  height: 1024 },
  { name: '375',  width: 375,  height: 812 },
  { name: '320',  width: 320,  height: 568 },
];

const browser = await chromium.launch();

for (const vp of viewports) {
  const ctx = await browser.newContext({ viewport: { width: vp.width, height: vp.height } });
  const page = await ctx.newPage();
  await page.goto(URL, { waitUntil: 'networkidle', timeout: 15000 });
  // Wait a moment for any error banners to settle
  await page.waitForTimeout(1500);
  await page.screenshot({
    path: path.join(outDir, `baseline-${vp.name}.png`),
    fullPage: true,
  });
  console.log(`  ✓ baseline-${vp.name}.png`);
  await ctx.close();
}

await browser.close();
console.log('Done.');
