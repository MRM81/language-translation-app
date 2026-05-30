import { chromium } from 'playwright';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const outDir = path.join(__dirname, 'sprint-008-final');
const portfolioDir = path.join(__dirname, 'portfolio');

// Find the running Vite port (try 5173 then 5174)
let PORT = 5173;
try {
  const r = await fetch('http://localhost:5173', { signal: AbortSignal.timeout(1000) });
  if (!r.ok) throw new Error();
} catch {
  PORT = 5174;
}

const URL = `http://localhost:${PORT}`;
console.log(`Using dev server at ${URL}`);

const viewports = [
  { name: '1440', width: 1440, height: 900 },
  { name: '1024', width: 1024, height: 768 },
  { name: '768',  width: 768,  height: 1024 },
  { name: '375',  width: 375,  height: 812 },
  { name: '320',  width: 320,  height: 568 },
];

const browser = await chromium.launch();

// ── Empty state at all breakpoints ─────────────────────────────────────────
for (const vp of viewports) {
  const ctx = await browser.newContext({ viewport: { width: vp.width, height: vp.height } });
  const page = await ctx.newPage();
  await page.goto(URL, { waitUntil: 'networkidle', timeout: 15000 });
  await page.waitForTimeout(1200);
  await page.screenshot({
    path: path.join(outDir, `final-${vp.name}.png`),
    fullPage: true,
  });
  console.log(`  ✓ final-${vp.name}.png`);
  await ctx.close();
}

// ── Portfolio hero: desktop with language pair visible ──────────────────────
{
  const ctx = await browser.newContext({ viewport: { width: 1440, height: 900 } });
  const page = await ctx.newPage();
  await page.goto(URL, { waitUntil: 'networkidle', timeout: 15000 });
  await page.waitForTimeout(1200);
  await page.screenshot({
    path: path.join(portfolioDir, 'hero-desktop-1440.png'),
    fullPage: true,
  });
  console.log('  ✓ portfolio/hero-desktop-1440.png');
  await ctx.close();
}

// ── Portfolio hero: mobile ──────────────────────────────────────────────────
{
  const ctx = await browser.newContext({ viewport: { width: 375, height: 812 } });
  const page = await ctx.newPage();
  await page.goto(URL, { waitUntil: 'networkidle', timeout: 15000 });
  await page.waitForTimeout(1200);
  await page.screenshot({
    path: path.join(portfolioDir, 'hero-mobile-375.png'),
    fullPage: true,
  });
  console.log('  ✓ portfolio/hero-mobile-375.png');
  await ctx.close();
}

await browser.close();
console.log('Done.');
