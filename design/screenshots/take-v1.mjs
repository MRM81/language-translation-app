import { chromium } from 'playwright';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const outDir = path.join(__dirname, 'v1');
const BASE = 'http://localhost:5175';

const browser = await chromium.launch();

async function shot(page, filename) {
  await page.waitForTimeout(600);
  await page.screenshot({ path: path.join(outDir, filename), fullPage: true });
  console.log(`  ✓ ${filename}`);
}

// --- 1. Landing page (desktop) ---
{
  const ctx = await browser.newContext({ viewport: { width: 1280, height: 800 } });
  const page = await ctx.newPage();
  await page.goto(BASE, { waitUntil: 'networkidle', timeout: 15000 });
  await shot(page, 'landing-page.png');
  await ctx.close();
}

// --- 2. Translation mode — text (desktop, with language loaded) ---
{
  const ctx = await browser.newContext({ viewport: { width: 1280, height: 800 } });
  const page = await ctx.newPage();
  await page.goto(BASE, { waitUntil: 'networkidle', timeout: 15000 });
  await page.click('button:has-text("Start Translating")');
  await page.waitForSelector('.translation-tabs', { timeout: 8000 });
  await shot(page, 'translation-mode.png');
  await ctx.close();
}

// --- 3. Audio translation tab ---
{
  const ctx = await browser.newContext({ viewport: { width: 1280, height: 800 } });
  const page = await ctx.newPage();
  await page.goto(BASE, { waitUntil: 'networkidle', timeout: 15000 });
  await page.click('button:has-text("Start Translating")');
  await page.waitForSelector('.translation-tabs', { timeout: 8000 });
  await page.click('.translation-tab:has-text("Audio")');
  await page.waitForTimeout(400);
  await shot(page, 'audio-translation.png');
  await ctx.close();
}

// --- 4. Conversation mode ---
{
  const ctx = await browser.newContext({ viewport: { width: 1280, height: 800 } });
  const page = await ctx.newPage();
  await page.goto(BASE, { waitUntil: 'networkidle', timeout: 15000 });
  await page.click('button:has-text("Start Translating")');
  await page.waitForSelector('.mode-nav-btn', { timeout: 8000 });
  await page.click('.mode-nav-btn:has-text("Conversation")');
  await page.waitForSelector('.conversation-mode', { timeout: 8000 });
  await shot(page, 'conversation-mode.png');
  await ctx.close();
}

// --- 5. Push-to-Talk (record tab active in conversation input) ---
{
  const ctx = await browser.newContext({
    viewport: { width: 1280, height: 800 },
    permissions: [],
  });
  const page = await ctx.newPage();
  await page.goto(BASE, { waitUntil: 'networkidle', timeout: 15000 });
  await page.click('button:has-text("Start Translating")');
  await page.waitForSelector('.mode-nav-btn', { timeout: 8000 });
  await page.click('.mode-nav-btn:has-text("Conversation")');
  await page.waitForSelector('.audio-tab:has-text("Record")', { timeout: 8000 });
  await page.click('.audio-tab:has-text("Record")');
  await shot(page, 'push-to-talk.png');
  await ctx.close();
}

// --- 6. Conversation manager (multiple conversations shown) ---
{
  const ctx = await browser.newContext({ viewport: { width: 1280, height: 800 } });
  const page = await ctx.newPage();
  // Pre-seed localStorage with multiple conversations
  await page.goto(BASE, { waitUntil: 'networkidle', timeout: 15000 });
  await page.evaluate(() => {
    const store = {
      version: 1,
      activeConversationId: 'c1',
      conversations: {
        c1: { id: 'c1', title: 'English → Spanish trip', isAutoTitle: false, createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), languageA: 'en', languageB: 'es', messages: [{ id: 'm1', speaker: 'a', originalText: 'Hello, where is the pharmacy?', translatedText: 'Hola, ¿dónde está la farmacia?', inputType: 'text', timestamp: new Date().toISOString() }] },
        c2: { id: 'c2', title: 'French meeting prep', isAutoTitle: false, createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), languageA: 'en', languageB: 'fr', messages: [{ id: 'm2', speaker: 'a', originalText: 'What time is the meeting?', translatedText: 'À quelle heure est la réunion?', inputType: 'text', timestamp: new Date().toISOString() }] },
        c3: { id: 'c3', title: 'Japanese restaurant', isAutoTitle: false, createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), languageA: 'en', languageB: 'ja', messages: [{ id: 'm3', speaker: 'a', originalText: 'Can I see the menu?', translatedText: 'メニューを見せてください', inputType: 'text', timestamp: new Date().toISOString() }] },
      }
    };
    localStorage.setItem('my-translation-app-conversations', JSON.stringify(store));
  });
  await page.reload({ waitUntil: 'networkidle' });
  await page.click('button:has-text("Start Translating")');
  await page.waitForSelector('.mode-nav-btn', { timeout: 8000 });
  await page.click('.mode-nav-btn:has-text("Conversation")');
  await page.waitForSelector('.conv-manager-row', { timeout: 8000 });
  await shot(page, 'conversation-manager.png');
  await ctx.close();
}

// --- 7. Conversation search ---
{
  const ctx = await browser.newContext({ viewport: { width: 1280, height: 800 } });
  const page = await ctx.newPage();
  await page.goto(BASE, { waitUntil: 'networkidle', timeout: 15000 });
  await page.evaluate(() => {
    const store = {
      version: 1,
      activeConversationId: 'c1',
      conversations: {
        c1: { id: 'c1', title: 'English → Spanish trip', isAutoTitle: false, createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), languageA: 'en', languageB: 'es', messages: [{ id: 'm1', speaker: 'a', originalText: 'Hello, where is the pharmacy?', translatedText: 'Hola, ¿dónde está la farmacia?', inputType: 'text', timestamp: new Date().toISOString() }] },
        c2: { id: 'c2', title: 'French meeting prep', isAutoTitle: false, createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), languageA: 'en', languageB: 'fr', messages: [{ id: 'm2', speaker: 'a', originalText: 'What time is the meeting?', translatedText: 'À quelle heure est la réunion?', inputType: 'text', timestamp: new Date().toISOString() }] },
        c3: { id: 'c3', title: 'Japanese restaurant', isAutoTitle: false, createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), languageA: 'en', languageB: 'ja', messages: [{ id: 'm3', speaker: 'a', originalText: 'Can I see the menu?', translatedText: 'メニューを見せてください', inputType: 'text', timestamp: new Date().toISOString() }] },
      }
    };
    localStorage.setItem('my-translation-app-conversations', JSON.stringify(store));
  });
  await page.reload({ waitUntil: 'networkidle' });
  await page.click('button:has-text("Start Translating")');
  await page.waitForSelector('.mode-nav-btn', { timeout: 8000 });
  await page.click('.mode-nav-btn:has-text("Conversation")');
  await page.waitForSelector('.conv-search-input', { timeout: 8000 });
  await page.fill('.conv-search-input', 'French');
  await page.waitForTimeout(400);
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
console.log('\nAll screenshots saved to design/screenshots/v1/');
