import { chromium } from 'playwright';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const outDir = path.join(__dirname, 'v1');
const BASE = 'http://localhost:5175';

const browser = await chromium.launch();

async function shot(page, filename) {
  await page.waitForTimeout(800);
  await page.screenshot({ path: path.join(outDir, filename), fullPage: true });
  console.log(`  ✓ ${filename}`);
}

function makeStore() {
  const now = new Date().toISOString();
  return {
    version: 1,
    activeConversationId: 'c1',
    conversations: {
      c1: { id: 'c1', version: 1, title: 'English → Spanish trip', isAutoTitle: false, createdAt: now, updatedAt: now, languageA: 'en', languageB: 'es', messages: [{ id: 'm1', speaker: 'A', originalText: 'Hello, where is the pharmacy?', translatedText: 'Hola, ¿dónde está la farmacia?', sourceLanguage: 'en', targetLanguage: 'es', inputType: 'text', timestamp: now }] },
      c2: { id: 'c2', version: 1, title: 'French meeting prep', isAutoTitle: false, createdAt: now, updatedAt: now, languageA: 'en', languageB: 'fr', messages: [{ id: 'm2', speaker: 'A', originalText: 'What time is the meeting?', translatedText: 'À quelle heure est la réunion?', sourceLanguage: 'en', targetLanguage: 'fr', inputType: 'text', timestamp: now }] },
      c3: { id: 'c3', version: 1, title: 'Japanese restaurant', isAutoTitle: false, createdAt: now, updatedAt: now, languageA: 'en', languageB: 'ja', messages: [{ id: 'm3', speaker: 'A', originalText: 'Can I see the menu?', translatedText: 'メニューを見せてください', sourceLanguage: 'en', targetLanguage: 'ja', inputType: 'text', timestamp: now }] },
    }
  };
}

// --- 7. Conversation search ---
{
  const ctx = await browser.newContext({ viewport: { width: 1280, height: 800 } });
  const page = await ctx.newPage();
  await page.goto(BASE, { waitUntil: 'networkidle', timeout: 15000 });
  await page.evaluate((store) => {
    localStorage.setItem('my-translation-app-conversations', JSON.stringify(store));
  }, makeStore());
  await page.reload({ waitUntil: 'networkidle' });
  await page.click('button:has-text("Start Translating")');
  await page.waitForSelector('.mode-nav-btn', { timeout: 10000 });
  await page.click('.mode-nav-btn:has-text("Conversation")');
  await page.waitForSelector('.conv-search-input', { timeout: 12000 });
  await page.fill('.conv-search-input', 'French');
  await shot(page, 'conversation-search.png');
  await ctx.close();
}

// --- 8. Mobile view — landing page ---
{
  const ctx = await browser.newContext({ viewport: { width: 375, height: 812 } });
  const page = await ctx.newPage();
  await page.goto(BASE, { waitUntil: 'networkidle', timeout: 15000 });
  await shot(page, 'mobile-view.png');
  await ctx.close();
}

await browser.close();
console.log('\nRemaining screenshots saved to design/screenshots/v1/');
